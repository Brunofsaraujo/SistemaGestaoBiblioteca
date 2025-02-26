using SistemaGestaoBiblioteca.Dominio.Enum;

namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Livro : BaseEntity
    {
        public Livro(
            string titulo,
            string autor,
            string iSBN)
        {
            Titulo = titulo;
            Autor = autor;
            ISBN = iSBN;

            ValidaCadastroLivro();
        }

        public string Titulo { get; }
        public string Autor { get; }
        public string ISBN { get; }
        public StatusLivro Status { get; private set; } = StatusLivro.Disponivel;

        private void ValidaCadastroLivro()
        {

        }
    }
}
