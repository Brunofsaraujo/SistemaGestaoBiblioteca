namespace SistemaGestaoBiblioteca.Infra
{
    public class InMemoryRepository<T> where T : class
    {
        private readonly List<T> _itens = [];

        public void Adicionar(T item) => _itens.Add(item);

        public IEnumerable<T> ObterTodos() => _itens.AsReadOnly();

        public T Obter(Func<T, bool> predicate) =>
            _itens.FirstOrDefault(predicate) ?? throw new InvalidOperationException("Item não encontrado.");
    }
}
