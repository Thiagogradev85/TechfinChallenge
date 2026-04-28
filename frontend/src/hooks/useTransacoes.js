import { useState, useEffect, useCallback } from 'react';
import toast from 'react-hot-toast';
import { listarTransacoes, simularTransacao } from '../api/transacoes';

export default function useTransacoes() {
  const [transacoes, setTransacoes] = useState([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    try {
      setLoading(true);
      const { data } = await listarTransacoes();
      setTransacoes(data);
    } catch {
      toast.error('Erro ao carregar transações.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregar();
  }, [carregar]);

  const simular = async (dto) => {
    const { data } = await simularTransacao(dto);
    await carregar();
    return data;
  };

  return { transacoes, loading, reload: carregar, simular };
}
