namespace SistemaGestaoBiblioteca.Dominio.DTO
{
    public readonly struct EmprestimoDto(string isbn, int identificacao)
    {
        public string ISBN { get; } = isbn;
        public int Identificacao { get; } = identificacao;

        //public static explicit operator Result<Emprestimo>(EmprestimoDto emprestimoDto) =>
        //    Emprestimo.Criar(emprestimoDto.ISBN, emprestimoDto.Identificacao);
    }
}
