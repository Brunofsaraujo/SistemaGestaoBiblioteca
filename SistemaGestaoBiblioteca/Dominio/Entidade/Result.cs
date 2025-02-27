namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public List<string> Errors { get; }

        private Result(T valor)
        {
            IsSuccess = true;
            Value = valor;
            Errors = [];
        }

        private Result(List<string> erros)
        {
            IsSuccess = false;
            Errors = erros;
        }

        public static Result<T> Success(T valor) => new(valor);
        public static Result<T> Failure(params string[] erros) => new([.. erros]);
    }

}
