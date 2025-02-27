using SistemaGestaoBiblioteca.Dominio.Entidade;

namespace SistemaGestaoBiblioteca.Dominio.DTO
{
    public readonly struct LivroDto(
        string titulo,
        string autor,
        string isbn)
    {
        public string Titulo { get; } = titulo;
        public string Autor { get; } = autor;
        public string ISBN { get; } = isbn;

        public static explicit operator Result<Livro>(LivroDto livroDto) =>
            Livro.Criar(livroDto.Titulo, livroDto.Autor, livroDto.ISBN);
    }
}
