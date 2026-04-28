import { transacoesApi } from './client';

export const simularTransacao = (dto) => transacoesApi.post('/transacoes', dto);
export const listarTransacoes = () => transacoesApi.get('/transacoes');
