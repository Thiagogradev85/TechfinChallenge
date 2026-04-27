import { authApi } from './client';

export const login = (email, senha) =>
  authApi.post('/auth/login', { email, senha });

export const register = (email, senha) =>
  authApi.post('/auth/register', { email, senha });
