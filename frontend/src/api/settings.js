import { clientesApi, transacoesApi } from './client';

export const getBroker = () =>
  clientesApi.get('/settings').then((r) => r.data.messageBroker);

export const setBroker = (broker) =>
  Promise.all([
    clientesApi.post('/settings/broker', { broker }),
    transacoesApi.post('/settings/broker', { broker }),
  ]);
