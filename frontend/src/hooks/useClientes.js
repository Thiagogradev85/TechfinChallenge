import { useState, useEffect, useCallback } from 'react';
import toast from 'react-hot-toast';
import {
  listarClientes,
  criarCliente,
  atualizarCliente,
  deletarCliente,
} from '../api/clientes';

export default function useClientes() {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    try {
      setLoading(true);
      const { data } = await listarClientes();
      setClientes(data);
    } catch {
      toast.error('Erro ao carregar clientes.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregar();
  }, [carregar]);

  const criar = async (dto) => {
    const { data } = await criarCliente(dto);
    await carregar();
    return data;
  };

  const atualizar = async (id, dto) => {
    await atualizarCliente(id, dto);
    await carregar();
  };

  const deletar = async (id) => {
    await deletarCliente(id);
    await carregar();
  };

  return { clientes, loading, reload: carregar, criar, atualizar, deletar };
}
