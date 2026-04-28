import { useState } from 'react';
import Input from '../ui/Input';
import Select from '../ui/Select';
import Button from '../ui/Button';
import { formatCPF } from '../../utils/formatters';

export default function TransacaoForm({ clientes = [], onSubmit, loading }) {
  const [form, setForm] = useState({ idCliente: '', valorSimulacao: '', tipo: 'debito' });
  const [errors, setErrors] = useState({});

  const set = (field) => (e) => {
    setForm((p) => ({ ...p, [field]: e.target.value }));
    setErrors((p) => ({ ...p, [field]: '' }));
  };

  const validate = () => {
    const e = {};
    if (!form.idCliente) e.idCliente = 'Selecione um cliente.';
    if (!form.valorSimulacao) e.valorSimulacao = 'Informe o valor.';
    else if (Number(form.valorSimulacao) <= 0) e.valorSimulacao = 'Valor deve ser > 0.';
    return e;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length) { setErrors(errs); return; }
    onSubmit({ idCliente: form.idCliente, valorSimulacao: Number(form.valorSimulacao), tipo: form.tipo });
  };

  const clienteOptions = [
    { value: '', label: 'Selecione um cliente...' },
    ...clientes.map((c) => ({
      value: c.id,
      label: `${c.nome} — ${formatCPF(c.cpf)}`,
    })),
  ];

  const tipoOptions = [
    { value: 'debito', label: '↓ Débito (subtrai limite)' },
    { value: 'credito', label: '↑ Crédito (adiciona limite)' },
  ];

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Select
        label="Cliente"
        value={form.idCliente}
        onChange={set('idCliente')}
        options={clienteOptions}
        error={errors.idCliente}
      />
      <Select
        label="Tipo"
        value={form.tipo}
        onChange={set('tipo')}
        options={tipoOptions}
      />
      <Input
        label="Valor (R$)"
        type="number"
        min="0.01"
        step="0.01"
        value={form.valorSimulacao}
        onChange={set('valorSimulacao')}
        error={errors.valorSimulacao}
        placeholder="500.00"
      />
      <Button
        type="submit"
        loading={loading}
        className="w-full"
        variant={form.tipo === 'credito' ? 'secondary' : 'primary'}
      >
        {form.tipo === 'credito' ? '↑ Adicionar crédito' : '↓ Simular débito'}
      </Button>
    </form>
  );
}
