import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Zap } from 'lucide-react';
import toast from 'react-hot-toast';
import { useAuth } from '../context/AuthContext';
import { login as loginApi, register as registerApi } from '../api/auth';
import Input from '../components/ui/Input';
import Button from '../components/ui/Button';

export default function LoginPage() {
  const [tab, setTab] = useState('login');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const { data } = await loginApi(email, senha);
      login(data.token, { email });
      navigate('/dashboard');
    } catch {
      toast.error('Email ou senha inválidos.');
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await registerApi(email, senha);
      toast.success('Conta criada! Faça login agora.');
      setTab('login');
    } catch (err) {
      toast.error(err.response?.data?.detalheErro ?? 'Erro ao criar conta.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-zinc-950 via-indigo-950 to-zinc-950 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
        <div className="bg-gradient-to-r from-indigo-600 to-indigo-700 px-8 py-7">
          <div className="flex items-center gap-3 mb-1">
            <div className="p-2 bg-white/20 rounded-lg">
              <Zap size={20} className="text-white" />
            </div>
            <h1 className="text-xl font-bold text-white">TechfinChallenge</h1>
          </div>
          <p className="text-indigo-200 text-sm">Plataforma de gestão financeira</p>
        </div>

        <div className="p-8">
          <div className="flex bg-gray-100 rounded-lg p-1 mb-6">
            {[
              { key: 'login', label: 'Entrar' },
              { key: 'register', label: 'Criar conta' },
            ].map(({ key, label }) => (
              <button
                key={key}
                onClick={() => setTab(key)}
                className={`flex-1 py-2 text-sm font-medium rounded-md transition-all ${
                  tab === key
                    ? 'bg-white text-indigo-600 shadow-sm'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                {label}
              </button>
            ))}
          </div>

          <form
            onSubmit={tab === 'login' ? handleLogin : handleRegister}
            className="space-y-4"
          >
            <Input
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="voce@email.com"
              required
            />
            <Input
              label="Senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="••••••••"
              required
            />
            <Button type="submit" loading={loading} className="w-full" size="lg">
              {tab === 'login' ? 'Entrar' : 'Criar conta'}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
