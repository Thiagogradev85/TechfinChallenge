import { useState } from 'react';
import { Plus, Pencil, Trash2, Search } from 'lucide-react';
import toast from 'react-hot-toast';
import useClientes from '../hooks/useClientes';
import Table from '../components/ui/Table';
import Button from '../components/ui/Button';
import Modal from '../components/ui/Modal';
import ConfirmDialog from '../components/ui/ConfirmDialog';
import ClienteForm from '../components/forms/ClienteForm';
import Card from '../components/ui/Card';
import { formatCPF, formatCurrency } from '../utils/formatters';

export default function ClientesPage() {
  const { clientes, loading, criar, atualizar, deletar } = useClientes();
  const [search, setSearch] = useState('');
  const [modal, setModal] = useState({ type: null, cliente: null });
  const [saving, setSaving] = useState(false);

  const filtered = clientes.filter(
    (c) =>
      c.nome.toLowerCase().includes(search.toLowerCase()) ||
      c.cpf.includes(search.replace(/\D/g, ''))
  );

  const close = () => setModal({ type: null, cliente: null });

  const handleCriar = async (dto) => {
    setSaving(true);
    try {
      await criar(dto);
      toast.success('Cliente cadastrado com sucesso!');
      close();
    } catch (err) {
      toast.error(err.response?.data?.detalheErro ?? 'Erro ao cadastrar cliente.');
    } finally {
      setSaving(false);
    }
  };

  const handleAtualizar = async (dto) => {
    setSaving(true);
    try {
      await atualizar(modal.cliente.id, dto);
      toast.success('Cliente atualizado!');
      close();
    } catch (err) {
      toast.error(err.response?.data?.detalheErro ?? 'Erro ao atualizar.');
    } finally {
      setSaving(false);
    }
  };

  const handleDeletar = async () => {
    setSaving(true);
    try {
      await deletar(modal.cliente.id);
      toast.success('Cliente removido.');
      close();
    } catch (err) {
      toast.error(err.response?.data?.detalheErro ?? 'Erro ao remover.');
    } finally {
      setSaving(false);
    }
  };

  const columns = [
    { key: 'nome', label: 'Nome' },
    {
      key: 'cpf',
      label: 'CPF',
      render: (r) => (
        <span className="font-mono text-xs">{formatCPF(r.cpf)}</span>
      ),
    },
    {
      key: 'valorLimite',
      label: 'Limite',
      render: (r) => (
        <span className="font-semibold text-indigo-700">
          {formatCurrency(r.valorLimite)}
        </span>
      ),
    },
    {
      key: 'acoes',
      label: '',
      render: (r) => (
        <div className="flex gap-1 justify-end">
          <Button
            variant="ghost"
            size="sm"
            leftIcon={<Pencil size={13} />}
            onClick={() => setModal({ type: 'edit', cliente: r })}
          >
            Editar
          </Button>
          <Button
            variant="ghost"
            size="sm"
            leftIcon={<Trash2 size={13} />}
            onClick={() => setModal({ type: 'delete', cliente: r })}
            className="text-red-500 hover:bg-red-50 hover:text-red-600"
          >
            Remover
          </Button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <Card padding={false}>
        <div className="flex items-center justify-between px-4 py-3 border-b border-gray-100">
          <div className="relative">
            <Search
              size={15}
              className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
            />
            <input
              className="pl-8 pr-4 py-2 text-sm border border-gray-200 rounded-lg
                focus:outline-none focus:ring-2 focus:ring-indigo-500 w-64"
              placeholder="Buscar por nome ou CPF…"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <Button
            leftIcon={<Plus size={15} />}
            onClick={() => setModal({ type: 'create', cliente: null })}
          >
            Novo cliente
          </Button>
        </div>

        <Table
          columns={columns}
          data={filtered}
          loading={loading}
          keyExtractor={(r) => r.id}
          emptyTitle="Nenhum cliente encontrado"
          emptyDescription="Cadastre o primeiro cliente usando o botão acima."
        />
      </Card>

      <Modal
        isOpen={modal.type === 'create'}
        onClose={close}
        title="Cadastrar novo cliente"
      >
        <ClienteForm onSubmit={handleCriar} onCancel={close} loading={saving} />
      </Modal>

      <Modal
        isOpen={modal.type === 'edit'}
        onClose={close}
        title="Editar cliente"
      >
        <ClienteForm
          initialData={modal.cliente ?? {}}
          onSubmit={handleAtualizar}
          onCancel={close}
          loading={saving}
        />
      </Modal>

      <ConfirmDialog
        isOpen={modal.type === 'delete'}
        onClose={close}
        onConfirm={handleDeletar}
        title="Remover cliente"
        message={`Tem certeza que deseja remover "${modal.cliente?.nome}"? Esta ação não pode ser desfeita.`}
        loading={saving}
      />
    </div>
  );
}
