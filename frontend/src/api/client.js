import axios from 'axios';

function createApiClient(baseURL) {
  const client = axios.create({ baseURL });

  client.interceptors.request.use((config) => {
    const token = localStorage.getItem('techfin_token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  });

  return client;
}

export const authApi = createApiClient(import.meta.env.VITE_AUTH_API);
export const clientesApi = createApiClient(import.meta.env.VITE_CLIENTES_API);
export const transacoesApi = createApiClient(import.meta.env.VITE_TRANSACOES_API);
