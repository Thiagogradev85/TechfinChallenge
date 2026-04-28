import { useState } from 'react';
import toast from 'react-hot-toast';
import useTransacoes from '../hooks/useTransacoes';
import { useClientesContext } from '../context/ClientesContext';
import Table from '../components/ui/Table';
import Card from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import TransacaoForm from '../components/forms/TransacaoForm';
import { formatCurrency } from '../utils/formatters';

export default function TransacoesPage() {
  const { transacoes, loading, simular } = useTransacoes();
  const { clientes, reload: recarregarClientes } = useClientesContext();
  const [simulating, setSimulating] = useState(false);
  const [lastResult, setLastResult] = useState(null);

  const handleSimular = async (dto) => {
    setSimulating(true);
    setLastResult(null);
    try {
      const data = await simular(dto);
      setLastResult(data);
      if (data.status === 'APROVADO') {
        toast.success(`Aprovada! ID: ${data.idTransacao?.slice(0, 8)}…`);
        setTimeout(recarregarClientes, 800);
      } else {
        toast.error('Transação negada — limite insuficiente.');
      }
    } catch {
      toast.error('Erro ao simular transação.');
    } finally {
      setSimulating(false);
    }
  };

  const clienteNome = (clienteId) =>
    clientes.find((c) => c.id === clienteId)?.nome ?? clienteId?.slice(0, 8) + '…';

  const columns = [
    {
      key: 'id',
      label: 'ID',
      render: (r) => (
        <span className="text-xs font-mono text-gray-400">{r.id?.slice(0, 8)}…</span>
      ),
    },
    {
      key: 'clienteId',
      label: 'Cliente',
      render: (r) => <span className="text-sm">{clienteNome(r.clienteId)}</span>,
    },
    {
      key: 'valor',
      label: 'Valor',
      render: (r) => (
        <span className="font-semibold text-gray-800">{formatCurrency(r.valor)}</span>
      ),
    },
    {
      key: 'status',
      label: 'Status',
      render: (r) => (
        <Badge variant={r.status === 'aprovado' ? 'success' : 'danger'}>
          {r.status?.toUpperCase()}
        </Badge>
      ),
    },
  ];

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
      <div className="space-y-4">
        <Card>
          <h3 className="text-sm font-semibold text-gray-800 mb-4">
            Simular transação
          </h3>
          <TransacaoForm
            clientes={clientes}
            onSubmit={handleSimular}
            loading={simulating}
          />
        </Card>

        {lastResult && (
          <Card>
            <h3 className="text-sm font-semibold text-gray-800 mb-3">
              Último resultado
            </h3>
            <div className="space-y-2">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-500">Status</span>
                <Badge
                  variant={lastResult.status === 'APROVADO' ? 'success' : 'danger'}
                >
                  {lastResult.status}
                </Badge>
              </div>
              {lastResult.idTransacao && (
                <div className="flex items-center justify-between text-sm">
                  <span className="text-gray-500">ID</span>
                  <span className="font-mono text-xs text-gray-600">
                    {lastResult.idTransacao?.slice(0, 12)}…
                  </span>
                </div>
              )}
            </div>
          </Card>
        )}
      </div>

      <div className="lg:col-span-2">
        <Card padding={false}>
          <div className="px-4 py-3 border-b border-gray-100">
            <h3 className="text-sm font-semibold text-gray-800">
              Histórico de transações
            </h3>
          </div>
          <Table
            columns={columns}
            data={transacoes}
            loading={loading}
            keyExtractor={(r) => r.id}
            emptyTitle="Nenhuma transação ainda"
            emptyDescription="Simule uma transação para ver o histórico aqui."
          />
        </Card>
      </div>
    </div>
  );
}
