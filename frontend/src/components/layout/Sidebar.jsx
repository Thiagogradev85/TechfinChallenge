import { useEffect, useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  ArrowLeftRight,
  Settings,
  LogOut,
  Zap,
  FlaskConical,
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import Badge from '../ui/Badge';
import { getBroker } from '../../api/settings';

const links = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/clientes', label: 'Clientes', icon: Users },
  { to: '/transacoes', label: 'Transações', icon: ArrowLeftRight },
  { to: '/settings', label: 'Configurações', icon: Settings },
  { to: '/tests', label: 'Testes', icon: FlaskConical },
];

export default function Sidebar() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [broker, setBroker] = useState(null);

  useEffect(() => {
    getBroker()
      .then(setBroker)
      .catch(() => {});
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <aside className="w-60 bg-zinc-900 flex flex-col shrink-0">
      <div className="px-5 py-6 border-b border-zinc-800">
        <div className="flex items-center gap-2">
          <div className="p-1.5 bg-indigo-600 rounded-lg">
            <Zap size={16} className="text-white" />
          </div>
          <div>
            <h1 className="text-white font-bold text-sm leading-tight">TechfinChallenge</h1>
            <p className="text-zinc-500 text-xs">Admin Panel</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 p-3 space-y-0.5">
        {links.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                isActive
                  ? 'bg-indigo-600 text-white'
                  : 'text-zinc-400 hover:bg-zinc-800 hover:text-zinc-100'
              }`
            }
          >
            <Icon size={17} />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="p-3 border-t border-zinc-800 space-y-1">
        {broker && (
          <div className="px-3 pb-1">
            <Badge variant={broker === 'Kafka' ? 'kafka' : 'rabbitmq'}>
              {broker}
            </Badge>
          </div>
        )}
        <button
          onClick={handleLogout}
          className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium
            text-zinc-400 hover:bg-zinc-800 hover:text-zinc-100 transition-colors w-full"
        >
          <LogOut size={17} />
          Sair
        </button>
      </div>
    </aside>
  );
}
