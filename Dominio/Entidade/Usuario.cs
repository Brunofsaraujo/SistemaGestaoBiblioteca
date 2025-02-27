using Flunt.Validations;

namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Usuario : BaseEntity
    {
        private Usuario(int identificacao, string nome)
        {
            Identificacao = identificacao;
            Nome = nome;

            ValidaCadastroUsuario();
        }

        public int Identificacao { get; }
        public string Nome { get; }

        private void ValidaCadastroUsuario()
        {
            AddNotifications(new Contract<Usuario>()
                .Requires()
                .IsGreaterThan(Identificacao, default, nameof(Nome), "O usuário deve possuir um número de identificação.")
                .IsNotNullOrEmpty(Nome, nameof(Nome), "O Nome do usuário deve ser preenchido.")
            );
        }

        public static Result<Usuario> Criar(int identificacao, string nome)
        {
            var usuario = new Usuario(identificacao, nome);

            return usuario.IsValid
                ? Result<Usuario>.Success(usuario)
                : Result<Usuario>.Failure([.. usuario.Notifications.Select(notificacao => notificacao.Message)]);
        }
    }
}
