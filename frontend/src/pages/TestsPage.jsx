import { useState } from 'react';
import { Play, PlayCircle, CheckCircle, XCircle, Clock, FlaskConical } from 'lucide-react';
import toast from 'react-hot-toast';
import { runAllTests, runOneTest } from '../api/tests';
import Card from '../components/ui/Card';

const STATUS = { idle: 'idle', running: 'running', done: 'done' };

function classNames(base, results, nome) {
  if (!results[nome]) return '';
  return results[nome].outcome === 'Passed' ? 'bg-green-50' : 'bg-red-50';
}

export default function TestsPage() {
  const [results, setResults] = useState({});
  const [status, setStatus] = useState(STATUS.idle);
  const [runningOne, setRunningOne] = useState(null);

  const allTests = Object.values(results);
  const passed = allTests.filter(r => r.outcome === 'Passed').length;
  const failed = allTests.filter(r => r.outcome === 'Failed').length;

  const handleRunAll = async () => {
    setStatus(STATUS.running);
    setResults({});
    try {
      const { data } = await runAllTests();
      console.log('[TESTS LOG]', data.log);
      const map = {};
      data.results.forEach(r => { map[r.name] = r; });
      setResults(map);
      const f = data.results.filter(r => r.outcome === 'Failed').length;
      if (f === 0) toast.success(`Todos os ${data.results.length} testes passaram!`);
      else toast.error(`${f} teste(s) falharam.`);
    } catch {
      toast.error('Erro ao rodar testes.');
    } finally {
      setStatus(STATUS.done);
    }
  };

  const handleRunOne = async (nome) => {
    setRunningOne(nome);
    try {
      const { data } = await runOneTest(nome);
      console.log('[TESTS LOG]', data.log);
      if (data.results.length > 0) {
        setResults(prev => ({ ...prev, [nome]: data.results[0] }));
        if (data.results[0].outcome === 'Passed') toast.success(`${nome} passou!`);
        else toast.error(`${nome} falhou.`);
      }
    } catch {
      toast.error('Erro ao rodar teste.');
    } finally {
      setRunningOne(null);
    }
  };

  const groups = Object.values(results).reduce((acc, r) => {
    if (!acc[r.className]) acc[r.className] = [];
    acc[r.className].push(r);
    return acc;
  }, {});

  const testList = [
    { className: 'TransacaoServiceTests', tests: [
      'AutorizarAsync_DeveRetornarErro_QuandoValorZero',
      'AutorizarAsync_DeveRetornarErro_QuandoClienteIdVazio',
      'AutorizarAsync_DeveRetornarErro_QuandoClienteNaoEncontrado',
      'AutorizarAsync_DeveRetornarErro_QuandoLimiteInsuficiente',
      'AutorizarAsync_DeveRetornarTransacao_QuandoDebitoAprovado',
      'AutorizarAsync_DeveAprovarCredito_SemVerificarLimite',
      'AutorizarAsync_DeveRetornarErro_QuandoTipoInvalido',
    ]},
    { className: 'TransacaoEventHandlerTests', tests: [
      'HandleAsync_DeveDebitarLimite_QuandoTipoDebito',
      'HandleAsync_DeveCreditarLimite_QuandoTipoCredito',
      'HandleAsync_DeveIgnorar_QuandoClienteNaoEncontrado',
      'HandleAsync_DeveLancarExcecao_QuandoServicoFalha',
    ]},
    { className: 'KafkaConsumerRetryTests', tests: [
      'ProcessWithRetry_DeveChamarHandlerMaxRetries_QuandoHandlerSempreLanca',
      'ProcessWithRetry_NaoDeveChamarDlq_QuandoSegundaTentativaSucede',
      'ProcessWithRetry_NaoDeveChamarDlq_QuandoPrimeiraTentativaSucede',
    ]},
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
            <FlaskConical size={20} className="text-indigo-500" />
            Testes unitários
          </h2>
          <p className="text-sm text-gray-500 mt-0.5">
            {allTests.length > 0
              ? `${passed} passaram · ${failed} falharam · ${allTests.length} total`
              : 'Clique em Rodar todos para executar os testes'}
          </p>
        </div>
        <button
          onClick={handleRunAll}
          disabled={status === STATUS.running}
          className="flex items-center gap-2 px-4 py-2 bg-indigo-600 text-white text-sm font-medium
            rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Play size={15} />
          {status === STATUS.running ? 'Rodando...' : 'Rodar todos'}
        </button>
      </div>

      {/* Resumo */}
      {allTests.length > 0 && (
        <div className="grid grid-cols-3 gap-4">
          <Card>
            <div className="flex items-center gap-3">
              <CheckCircle size={24} className="text-green-500" />
              <div>
                <div className="text-2xl font-bold text-gray-800">{passed}</div>
                <div className="text-xs text-gray-500">Passaram</div>
              </div>
            </div>
          </Card>
          <Card>
            <div className="flex items-center gap-3">
              <XCircle size={24} className="text-red-500" />
              <div>
                <div className="text-2xl font-bold text-gray-800">{failed}</div>
                <div className="text-xs text-gray-500">Falharam</div>
              </div>
            </div>
          </Card>
          <Card>
            <div className="flex items-center gap-3">
              <Clock size={24} className="text-gray-400" />
              <div>
                <div className="text-2xl font-bold text-gray-800">{allTests.length}</div>
                <div className="text-xs text-gray-500">Total</div>
              </div>
            </div>
          </Card>
        </div>
      )}

      {/* Tabela de testes */}
      {testList.map(({ className, tests }) => (
        <Card key={className} padding={false}>
          <div className="px-4 py-3 border-b border-gray-100 flex items-center justify-between">
            <h3 className="text-sm font-semibold text-gray-700 font-mono">{className}</h3>
            <span className="text-xs text-gray-400">{tests.length} testes</span>
          </div>
          <div className="divide-y divide-gray-50">
            {tests.map((nome) => {
              const r = results[nome];
              const isRunning = runningOne === nome;
              return (
                <div
                  key={nome}
                  className={`flex items-center justify-between px-4 py-3 transition-colors
                    ${r?.outcome === 'Passed' ? 'bg-green-50' : r?.outcome === 'Failed' ? 'bg-red-50' : 'bg-white'}`}
                >
                  <div className="flex items-center gap-3 min-w-0">
                    {!r && <div className="w-4 h-4 rounded-full border-2 border-gray-200 flex-shrink-0" />}
                    {r?.outcome === 'Passed' && <CheckCircle size={16} className="text-green-500 flex-shrink-0" />}
                    {r?.outcome === 'Failed' && <XCircle size={16} className="text-red-500 flex-shrink-0" />}
                    <div className="min-w-0">
                      <p className="text-sm font-mono text-gray-700 truncate">{nome}</p>
                      {r?.outcome === 'Failed' && r.errorMessage && (
                        <p className="text-xs text-red-600 mt-0.5 truncate">{r.errorMessage}</p>
                      )}
                      {r?.outcome === 'Passed' && (
                        <p className="text-xs text-gray-400 mt-0.5">{r.durationMs.toFixed(0)}ms</p>
                      )}
                    </div>
                  </div>
                  <button
                    onClick={() => handleRunOne(nome)}
                    disabled={isRunning || status === STATUS.running}
                    className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-indigo-600
                      hover:bg-indigo-50 rounded-lg transition-colors disabled:opacity-40 flex-shrink-0 ml-4"
                  >
                    <PlayCircle size={13} />
                    {isRunning ? 'Rodando...' : 'Rodar'}
                  </button>
                </div>
              );
            })}
          </div>
        </Card>
      ))}
    </div>
  );
}
