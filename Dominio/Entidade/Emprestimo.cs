using Flunt.Validations;
using SistemaGestaoBiblioteca.Dominio.Enum;

namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Emprestimo : BaseEntity
    {
        private Emprestimo(
            Livro livro,
            Usuario usuario,
            DateTime dataEmprestimo)
        {
            Livro = livro;
            Usuario = usuario;
            DataEmprestimo = dataEmprestimo;

            ValidaEmprestimo();

            if (IsValid)
                Livro.AlterarStatusLivro(StatusLivro.Emprestado);
        }

        public Livro Livro { get; }
        public Usuario Usuario { get; }
        public DateTime DataEmprestimo { get; }
        public DateTime? DataDevolucao { get; private set; }

        private void ValidaEmprestimo()
        {
            AddNotifications(new Contract<Usuario>()
                .Requires()
                .IsNotNull(Livro, nameof(Livro), "Deve ser informado um livro para empréstimo.")
                .IsTrue(ValidaLivroDisponivelEmprestimo(), nameof(Livro), $"Livro '{Livro.Titulo}' está indisponível para empréstimo.")
                .IsNotNull(Usuario, nameof(Usuario), "Deve ser informado um usuário para empréstimo de livro.")
                .AreNotEquals(DataEmprestimo, default, nameof(DataEmprestimo), "Deve ser informada uma data de empréstimo do livro.")
            );
        }

        public static Result<Emprestimo> Criar(Livro livro, Usuario usuario, DateTime dataEmprestimo)
        {
            var emprestimo = new Emprestimo(livro, usuario, dataEmprestimo);

            return emprestimo.IsValid
                ? Result<Emprestimo>.Success(emprestimo)
                : Result<Emprestimo>.Failure([.. emprestimo.Notifications.Select(notificacao => notificacao.Message)]);
        }

        private bool ValidaLivroDisponivelEmprestimo() =>
            Livro.Status == StatusLivro.Disponivel;

        public void DevolverLivro(Livro livro)
        {
            if (livro.Status == StatusLivro.Disponivel)
            {
                AddNotification(nameof(Livro), $"Livro {Livro.Titulo} não pode ser devolvido, pois está disponível para empréstimo.");
                return;
            }

            Livro.AlterarStatusLivro(StatusLivro.Disponivel);
            AtribuirDataDevolucao();
        }

        private void AtribuirDataDevolucao() => DataDevolucao = DateTime.Now;
    }
}
