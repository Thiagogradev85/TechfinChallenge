import Spinner from './Spinner';
import EmptyState from './EmptyState';

export default function Table({
  columns,
  data,
  loading,
  keyExtractor,
  emptyTitle = 'Nenhum item encontrado',
  emptyDescription = '',
}) {
  if (loading) {
    return (
      <div className="flex justify-center py-14">
        <Spinner />
      </div>
    );
  }

  if (!data || data.length === 0) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b border-gray-100">
            {columns.map((col) => (
              <th
                key={col.key}
                className="text-left px-4 py-3 text-xs font-semibold text-gray-400 uppercase tracking-wide"
              >
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map((row, idx) => (
            <tr
              key={keyExtractor ? keyExtractor(row) : idx}
              className="border-b border-gray-50 hover:bg-gray-50 transition-colors"
            >
              {columns.map((col) => (
                <td key={col.key} className="px-4 py-3 text-gray-700">
                  {col.render ? col.render(row) : row[col.key]}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
