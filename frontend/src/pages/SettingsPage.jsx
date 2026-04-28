import { useEffect, useState } from 'react';
import { AlertTriangle, RefreshCw } from 'lucide-react';
import toast from 'react-hot-toast';
import Card from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import Spinner from '../components/ui/Spinner';
import { getBroker, setBroker } from '../api/settings';

const BROKERS = [
  {
    id: 'RabbitMQ',
    description: 'Mensageria clássica com filas duráveis e suporte a DLQ.',
    active: 'border-orange-500 bg-orange-50 text-orange-800',
    badge: 'rabbitmq',
  },
  {
    id: 'Kafka',
    description: 'Streaming distribuído de alta performance com offsets.',
    active: 'border-purple-500 bg-purple-50 text-purple-800',
    badge: 'kafka',
  },
];

export default function SettingsPage() {
  const [current, setCurrent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getBroker()
      .then(setCurrent)
      .catch(() => toast.error('Erro ao carregar configurações.'))
      .finally(() => setLoading(false));
  }, []);

  const handleSwitch = async (broker) => {
    if (broker === current) return;
    setSaving(true);
    try {
      await setBroker(broker);
      setCurrent(broker);
      toast.success(`Broker alterado para ${broker}. Reinicie as APIs para aplicar.`);
    } catch {
      toast.error('Erro ao trocar o broker.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-xl space-y-5">
      <Card>
        <div className="flex items-center justify-between mb-5">
          <div>
            <h3 className="text-sm font-semibold text-gray-900">Message Broker</h3>
            <p className="text-xs text-gray-500 mt-0.5">
              Alterne entre RabbitMQ e Kafka
            </p>
          </div>
          {loading ? (
            <Spinner size="sm" />
          ) : (
            current && (
              <Badge variant={current === 'Kafka' ? 'kafka' : 'rabbitmq'}>
                {current} ativo
              </Badge>
            )
          )}
        </div>

        <div className="grid grid-cols-2 gap-3 mb-5">
          {BROKERS.map((b) => (
            <button
              key={b.id}
              disabled={saving || loading}
              onClick={() => handleSwitch(b.id)}
              className={`p-4 rounded-xl border-2 text-left transition-all
                disabled:opacity-60 disabled:cursor-not-allowed
                ${
                  current === b.id
                    ? b.active
                    : 'border-gray-200 text-gray-500 hover:border-gray-300 hover:bg-gray-50'
                }`}
            >
              <div className="flex items-center justify-between mb-1">
                <span className="text-sm font-semibold">{b.id}</span>
                {saving && current !== b.id && (
                  <RefreshCw size={12} className="animate-spin opacity-50" />
                )}
                {current === b.id && (
                  <span className="text-xs opacity-70">ativo</span>
                )}
              </div>
              <p className="text-xs opacity-70 leading-snug">{b.description}</p>
            </button>
          ))}
        </div>

        <div className="flex items-start gap-2.5 p-3 bg-amber-50 rounded-lg text-xs text-amber-800 border border-amber-100">
          <AlertTriangle size={14} className="mt-0.5 shrink-0" />
          <span>
            Após trocar o broker, reinicie a <strong>Clientes.Api</strong> e a{' '}
            <strong>Transacao.Api</strong> para aplicar a mudança no DI.
          </span>
        </div>
      </Card>

      <Card>
        <h3 className="text-sm font-semibold text-gray-900 mb-4">
          Portas das APIs
        </h3>
        <div className="space-y-2">
          {[
            { label: 'Auth API', url: import.meta.env.VITE_AUTH_API },
            { label: 'Clientes API', url: import.meta.env.VITE_CLIENTES_API },
            { label: 'Transações API', url: import.meta.env.VITE_TRANSACOES_API },
          ].map(({ label, url }) => (
            <div key={label} className="flex items-center justify-between text-sm">
              <span className="text-gray-500">{label}</span>
              <code className="text-xs bg-gray-100 px-2 py-0.5 rounded font-mono text-gray-700">
                {url}
              </code>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
