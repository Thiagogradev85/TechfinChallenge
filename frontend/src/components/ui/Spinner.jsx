const sizes = { sm: 'h-4 w-4 border-2', md: 'h-7 w-7 border-2', lg: 'h-12 w-12 border-4' };

export default function Spinner({ size = 'md' }) {
  return (
    <div
      className={`${sizes[size]} animate-spin rounded-full border-gray-200 border-t-indigo-600`}
    />
  );
}
