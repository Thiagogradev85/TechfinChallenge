import { useState } from 'react';
import Input from '../ui/Input';
import Button from '../ui/Button';

export default function ClienteForm({ initialData = {}, onSubmit, onCancel, loading }) {
  const [form, setForm] = useState({
    nome: initialData.nome ?? '',
    cpf: initialData.cpf ?? '',
    valorLimite: initialData.valorLimite ?? '',
  });
  const [errors, setErrors] = useState({});

  const set = (field) => (e) => {
    setForm((p) => ({ ...p, [field]: e.target.value }));
    setErrors((p) => ({ ...p, [field]: '' }));
  };

  const validate = () => {
    const e = {};
    if (!form.nome.trim()) e.nome = 'Nome é obrigatório.';
    const cpfNum = form.cpf.replace(/\D/g, '');
    if (!cpfNum) e.cpf = 'CPF é obrigatório.';
    else if (cpfNum.length !== 11) e.cpf = 'CPF deve ter 11 dígitos.';
    if (form.valorLimite === '' || form.valorLimite === null) e.valorLimite = 'Valor limite é obrigatório.';
    else if (Number(form.valorLimite) < 0) e.valorLimite = 'Valor deve ser ≥ 0.';
    return e;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length) { setErrors(errs); return; }
    onSubmit({
      nome: form.nome.trim(),
      cpf: form.cpf.replace(/\D/g, ''),
      valorLimite: Number(form.valorLimite),
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Nome completo"
        value={form.nome}
        onChange={set('nome')}
        error={errors.nome}
        placeholder="João da Silva"
      />
      <Input
        label="CPF (apenas números)"
        value={form.cpf}
        onChange={set('cpf')}
        error={errors.cpf}
        placeholder="00000000000"
        maxLength={14}
      />
      <Input
        label="Valor Limite (R$)"
        type="number"
        min="0"
        step="0.01"
        value={form.valorLimite}
        onChange={set('valorLimite')}
        error={errors.valorLimite}
        placeholder="1000.00"
      />
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
          Cancelar
        </Button>
        <Button type="submit" loading={loading}>
          {initialData.id ? 'Salvar alterações' : 'Cadastrar'}
        </Button>
      </div>
    </form>
  );
}
