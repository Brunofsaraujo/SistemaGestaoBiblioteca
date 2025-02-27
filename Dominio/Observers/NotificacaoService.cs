using SistemaGestaoBiblioteca.Dominio.Interface;

namespace SistemaGestaoBiblioteca.Dominio.Observers
{
    public class NotificacaoService : IObserver
    {
        public void Update(string mensagem)
        {
            Console.WriteLine("Notificação: " + mensagem);
        }
    }
}
