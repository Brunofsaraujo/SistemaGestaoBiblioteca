using SistemaGestaoBiblioteca.Dominio.Entidade;

namespace SistemaGestaoBiblioteca.Dominio.DTO
{
    public readonly struct UsuarioDto(
        int identificacao,
        string nome)
    {
        public int Identificacao { get; } = identificacao;
        public string Nome { get; } = nome;

        public static explicit operator Result<Usuario>(UsuarioDto usuarioDto) =>
            Usuario.Criar(usuarioDto.Identificacao, usuarioDto.Nome);
    }
}
