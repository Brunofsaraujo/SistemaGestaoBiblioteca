namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Emprestimo : BaseEntity
    {
        public Emprestimo(
            Livro livro,
            Usuario usuario,
            DateTime dataEmprestimo)
        {
            Livro = livro;
            Usuario = usuario;
            DataEmprestimo = dataEmprestimo;

            ValidaEmprestimo();
        }

        public Livro Livro { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime DataEmprestimo { get; set; }

        private void ValidaEmprestimo()
        {

        }
    }
}
