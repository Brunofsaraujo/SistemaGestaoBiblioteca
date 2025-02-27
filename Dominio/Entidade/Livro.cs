using Flunt.Validations;
using SistemaGestaoBiblioteca.Dominio.Enum;
using SistemaGestaoBiblioteca.Dominio.ValueObjects;

namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Livro : BaseEntity
    {
        private Livro(string titulo, string autor, string isbn)
        {
            Titulo = titulo;
            Autor = autor;
            ISBN = NormalizadorISBN.NormalizarISBN(isbn);
            ValidaCadastroLivro();
        }

        public string Titulo { get; }
        public string Autor { get; }
        public string ISBN { get; }
        public StatusLivro Status { get; private set; } = StatusLivro.Disponivel;
        public string IsbnNormalizado => NormalizadorISBN.NormalizarISBN(ISBN);

        private void ValidaCadastroLivro()
        {
            var contrato = new Contract<Livro>()
                .Requires()
                .IsNotNullOrEmpty(Titulo, nameof(Titulo), "O Título do livro deve ser preenchido.")
                .IsNotNullOrEmpty(Autor, nameof(Autor), "O Autor do livro deve ser preenchido.")
                .IsNotNullOrEmpty(ISBN, nameof(ISBN), "ISBN do livro deve ser preenchido.");

            if (!string.IsNullOrWhiteSpace(ISBN) && !ValidadorISBN.EhRegistroValidoISBN(ISBN))
                contrato.AddNotification(nameof(ISBN), $"ISBN '{ISBN}' inválido para registro do livro. Verifique as regras de ISBN.");

            AddNotifications(contrato);
        }

        public static Result<Livro> Criar(string titulo, string autor, string isbn)
        {
            var livro = new Livro(titulo, autor, isbn);

            return livro.IsValid
                ? Result<Livro>.Success(livro)
                : Result<Livro>.Failure([.. livro.Notifications.Select(notificacao => notificacao.Message)]);
        }

        public void AlterarStatusLivro(StatusLivro statusLivro) => Status = statusLivro;        
    }
}
