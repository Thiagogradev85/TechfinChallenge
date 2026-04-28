import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import toast from 'react-hot-toast';
import {
  listarClientes,
  criarCliente,
  atualizarCliente,
  deletarCliente,
} from '../api/clientes';

const ClientesContext = createContext(null);

export function ClientesProvider({ children }) {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);

  const reload = useCallback(async () => {
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
    reload();
  }, [reload]);

  const criar = async (dto) => {
    const { data } = await criarCliente(dto);
    await reload();
    return data;
  };

  const atualizar = async (id, dto) => {
    await atualizarCliente(id, dto);
    await reload();
  };

  const deletar = async (id) => {
    await deletarCliente(id);
    await reload();
  };

  return (
    <ClientesContext.Provider value={{ clientes, loading, reload, criar, atualizar, deletar }}>
      {children}
    </ClientesContext.Provider>
  );
}

export function useClientesContext() {
  return useContext(ClientesContext);
}
