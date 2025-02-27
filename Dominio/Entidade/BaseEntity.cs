using Flunt.Notifications;

namespace SistemaGestaoBiblioteca.Dominio.Entidade
{
    public abstract class BaseEntity : Notifiable<Notification>
    {
        public Guid Id { get; protected set; }
        public DateTime DataCriacao { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            DataCriacao = DateTime.Now;
        }
    }
}
