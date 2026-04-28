import { useEffect, useState } from 'react';
import { Users, ArrowLeftRight, DollarSign, TrendingUp } from 'lucide-react';
import Stat from '../components/ui/Stat';
import Card from '../components/ui/Card';
import { useClientesContext } from '../context/ClientesContext';
import { listarTransacoes } from '../api/transacoes';
import { formatCurrency } from '../utils/formatters';

export default function DashboardPage() {
  const { clientes, loading: loadingClientes } = useClientesContext();
  const [transacoes, setTransacoes] = useState([]);
  const [loadingT, setLoadingT] = useState(true);

  useEffect(() => {
    listarTransacoes()
      .then((res) => setTransacoes(res.data ?? []))
      .catch(() => {})
      .finally(() => setLoadingT(false));
  }, []);

  const loading = loadingClientes || loadingT;
  const data = {
    totalClientes: clientes.length,
    totalTransacoes: transacoes.length,
    limiteTotal: clientes.reduce((s, c) => s + (c.valorLimite ?? 0), 0),
    totalTransacionado: transacoes.reduce((s, t) => s + (t.valor ?? 0), 0),
  };

  const val = (v) => (loading ? '...' : v);

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        <Stat
          label="Total de Clientes"
          value={val(data?.totalClientes ?? 0)}
          icon={Users}
          color="indigo"
        />
        <Stat
          label="Transações Realizadas"
          value={val(data?.totalTransacoes ?? 0)}
          icon={ArrowLeftRight}
          color="emerald"
        />
        <Stat
          label="Limite Total Cadastrado"
          value={val(formatCurrency(data?.limiteTotal ?? 0))}
          icon={DollarSign}
          color="amber"
        />
        <Stat
          label="Total Transacionado"
          value={val(formatCurrency(data?.totalTransacionado ?? 0))}
          icon={TrendingUp}
          color="red"
        />
      </div>

      <Card>
        <h3 className="text-sm font-semibold text-gray-800 mb-2">Acesso rápido</h3>
        <p className="text-sm text-gray-500">
          Use a barra lateral para gerenciar clientes, simular transações e
          configurar o broker de mensageria (RabbitMQ ↔ Kafka).
        </p>
      </Card>
    </div>
  );
}
