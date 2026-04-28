import { useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { User } from 'lucide-react';

const titles = {
  '/dashboard': 'Dashboard',
  '/clientes': 'Clientes',
  '/transacoes': 'Transações',
  '/settings': 'Configurações',
};

export default function Navbar() {
  const { pathname } = useLocation();
  const { user } = useAuth();

  return (
    <header className="bg-white border-b border-gray-100 px-6 h-14 flex items-center justify-between shrink-0">
      <h2 className="text-base font-semibold text-gray-900">
        {titles[pathname] ?? 'TechfinChallenge'}
      </h2>
      <div className="flex items-center gap-2 text-sm text-gray-500">
        <div className="h-7 w-7 rounded-full bg-indigo-100 flex items-center justify-center">
          <User size={14} className="text-indigo-600" />
        </div>
        <span className="text-xs">{user?.email ?? ''}</span>
      </div>
    </header>
  );
}
