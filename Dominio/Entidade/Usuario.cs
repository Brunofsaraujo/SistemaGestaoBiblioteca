namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public class Usuario : BaseEntity
    {
        public Usuario(
            string nome)
        {
            Id = Guid.NewGuid();
            Nome = nome;

            ValidaCadastroUsuario();
        }

        public Guid Id { get; }
        public string Nome { get; }

        private void ValidaCadastroUsuario()
        {

        }
    }
}
