namespace SistemaGestaoBiblioteca.Dominio.Interface
{
    public interface IObservable
    {
        void RegistrarObserver(IObserver observer);
        void RemoverObserver(IObserver observer);
        void NotificarObservers(string mensagem);
    }
}
