import { useState } from 'react';
import { Play, PlayCircle, CheckCircle, XCircle, Clock, FlaskConical } from 'lucide-react';
import toast from 'react-hot-toast';
import { runAllTests, runOneTest } from '../api/tests';
import Card from '../components/ui/Card';

const STATUS = { idle: 'idle', running: 'running', done: 'done' };

const testList = [
  {
    className: 'TransacaoServiceTests',
    descricao: 'Validações e regras de negócio da autorização de transações',
    tests: [
      { nome: 'AutorizarAsync_DeveRetornarErro_QuandoValorZero',            descricao: 'Transação com valor R$0 deve ser recusada' },
      { nome: 'AutorizarAsync_DeveRetornarErro_QuandoClienteIdVazio',       descricao: 'Transação sem ClienteId deve ser recusada' },
      { nome: 'AutorizarAsync_DeveRetornarErro_QuandoClienteNaoEncontrado', descricao: 'ClienteId inexistente retorna erro' },
      { nome: 'AutorizarAsync_DeveRetornarErro_QuandoLimiteInsuficiente',   descricao: 'Débito de R$200 com limite R$50 deve ser recusado' },
      { nome: 'AutorizarAsync_DeveRetornarTransacao_QuandoDebitoAprovado',  descricao: 'Débito de R$200 com limite R$1000 deve ser aprovado' },
      { nome: 'AutorizarAsync_DeveAprovarCredito_SemVerificarLimite',       descricao: 'Crédito é aprovado mesmo com limite zero' },
      { nome: 'AutorizarAsync_DeveRetornarErro_QuandoTipoInvalido',         descricao: 'Tipo "transferencia" é rejeitado — só débito ou crédito' },
    ],
  },
  {
    className: 'TransacaoEventHandlerTests',
    descricao: 'Processamento do evento Kafka após transação aprovada',
    tests: [
      { nome: 'HandleAsync_DeveDebitarLimite_QuandoTipoDebito',        descricao: 'Débito de R$300 reduz limite de R$1000 → R$700' },
      { nome: 'HandleAsync_DeveCreditarLimite_QuandoTipoCredito',      descricao: 'Crédito de R$500 aumenta limite de R$1000 → R$1500' },
      { nome: 'HandleAsync_DeveIgnorar_QuandoClienteNaoEncontrado',    descricao: 'Evento de cliente inexistente é ignorado sem erro' },
      { nome: 'HandleAsync_DeveLancarExcecao_QuandoServicoFalha',      descricao: 'Falha no serviço lança exceção — ativa retry no Kafka' },
    ],
  },
  {
    className: 'KafkaConsumerRetryTests',
    descricao: 'Lógica de retry e Dead Letter Queue do KafkaConsumer',
    tests: [
      { nome: 'ProcessWithRetry_DeveChamarHandlerMaxRetries_QuandoHandlerSempreLanca', descricao: 'Handler sempre falha: tenta 3x e manda para a DLQ' },
      { nome: 'ProcessWithRetry_NaoDeveChamarDlq_QuandoSegundaTentativaSucede',        descricao: 'Falha na 1ª tentativa, sucesso na 2ª: DLQ não é chamado' },
      { nome: 'ProcessWithRetry_NaoDeveChamarDlq_QuandoPrimeiraTentativaSucede',       descricao: 'Sucesso na 1ª tentativa: handler chamado 1x, sem DLQ' },
    ],
  },
];

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
        if (data.results[0].outcome === 'Passed') toast.success(`Passou!`);
        else toast.error(`Falhou.`);
      }
    } catch {
      toast.error('Erro ao rodar teste.');
    } finally {
      setRunningOne(null);
    }
  };

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

      {/* Grupos de testes */}
      {testList.map(({ className, descricao: descricaoGrupo, tests }) => (
        <Card key={className} padding={false}>
          {/* Cabeçalho do grupo */}
          <div className="px-4 py-3 border-b border-gray-100">
            <div className="flex items-center justify-between">
              <h3 className="text-sm font-semibold text-gray-700 font-mono">{className}</h3>
              <span className="text-xs text-gray-400">{tests.length} testes</span>
            </div>
            <p className="text-xs text-gray-400 mt-0.5">{descricaoGrupo}</p>
          </div>

          {/* Lista de testes */}
          <div className="divide-y divide-gray-50">
            {tests.map(({ nome, descricao }) => {
              const r = results[nome];
              const isRunning = runningOne === nome;
              return (
                <div
                  key={nome}
                  className={`flex items-start justify-between px-4 py-3 transition-colors gap-4
                    ${r?.outcome === 'Passed' ? 'bg-green-50' : r?.outcome === 'Failed' ? 'bg-red-50' : 'bg-white'}`}
                >
                  <div className="flex items-start gap-3 min-w-0">
                    {/* Ícone de status */}
                    <div className="mt-0.5 flex-shrink-0">
                      {!r && <div className="w-4 h-4 rounded-full border-2 border-gray-200" />}
                      {r?.outcome === 'Passed' && <CheckCircle size={16} className="text-green-500" />}
                      {r?.outcome === 'Failed' && <XCircle size={16} className="text-red-500" />}
                    </div>

                    {/* Nome + descrição + resultado */}
                    <div className="min-w-0">
                      <p className="text-xs font-mono text-gray-500 truncate">{nome}</p>
                      <p className="text-sm text-gray-700 mt-0.5">{descricao}</p>
                      {r?.outcome === 'Passed' && (
                        <p className="text-xs text-green-600 mt-0.5">{r.durationMs.toFixed(0)}ms</p>
                      )}
                      {r?.outcome === 'Failed' && r.errorMessage && (
                        <p className="text-xs text-red-600 mt-0.5 line-clamp-2">{r.errorMessage}</p>
                      )}
                    </div>
                  </div>

                  {/* Botão rodar */}
                  <button
                    onClick={() => handleRunOne(nome)}
                    disabled={isRunning || status === STATUS.running}
                    className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-indigo-600
                      hover:bg-indigo-50 rounded-lg transition-colors disabled:opacity-40 flex-shrink-0"
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
