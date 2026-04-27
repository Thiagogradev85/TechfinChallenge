import { clientesApi } from './client';

export const listarClientes = () => clientesApi.get('/clientes');
export const buscarCliente = (id) => clientesApi.get(`/clientes/${id}`);
export const criarCliente = (dto) => clientesApi.post('/clientes', dto);
export const atualizarCliente = (id, dto) => clientesApi.put(`/clientes/${id}`, dto);
export const deletarCliente = (id) => clientesApi.delete(`/clientes/${id}`);
