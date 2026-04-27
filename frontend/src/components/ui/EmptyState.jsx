import { Inbox } from 'lucide-react';

export default function EmptyState({
  title,
  description,
  icon: Icon = Inbox,
  action,
}) {
  return (
    <div className="flex flex-col items-center justify-center py-14 text-center px-4">
      <Icon className="h-10 w-10 text-gray-300 mb-3" strokeWidth={1.5} />
      <p className="text-sm font-medium text-gray-500">{title}</p>
      {description && (
        <p className="text-xs text-gray-400 mt-1 max-w-xs">{description}</p>
      )}
      {action && <div className="mt-4">{action}</div>}
    </div>
  );
}
