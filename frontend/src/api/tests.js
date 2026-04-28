import { clientesApi as api } from './client';

export const runAllTests = () => api.post('/tests/run');
export const runOneTest = (nome) => api.post(`/tests/run/${nome}`);
