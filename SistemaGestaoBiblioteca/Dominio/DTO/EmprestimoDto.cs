namespace SistemaGestaoBiblioteca.Dominio.DTO
{
    public readonly struct EmprestimoDto(string isbn, int identificacao)
    {
        public string ISBN { get; } = isbn;
        public int Identificacao { get; } = identificacao;
    }
}
