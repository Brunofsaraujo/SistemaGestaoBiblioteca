using SistemaGestaoBiblioteca.Dominio.Interface;

namespace SistemaGestaoBiblioteca.Dominio.Observers
{
    public class NotificacaoService : IObserver
    {
        public void Atualizar(string mensagem) =>
            MessageBox.Show(mensagem, "Notificação da Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
