using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoBiblioteca.Aplicacao;
using SistemaGestaoBiblioteca.Infra;

namespace SistemaGestaoBiblioteca.Apresentacao
{
    internal class Program
    {
        static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(InMemoryRepository<>))
                .AddSingleton<BibliotecaService>()
                .BuildServiceProvider();

            var bibliotecaService = serviceProvider.GetRequiredService<BibliotecaService>();
            _ = new Menu(bibliotecaService);

            //var notificacaoService = new NotificacaoService();
            //biblioteca.RegistrarObserver(notificacaoService);
        }
    }
}