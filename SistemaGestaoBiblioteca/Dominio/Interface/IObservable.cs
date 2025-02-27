namespace SistemaGestaoBiblioteca.Dominio.Interface
{
    public interface IObservable
    {
        void AdicionarObservador(IObserver observer);
        void NotificarObservadores(string mensagem);
    }
}
