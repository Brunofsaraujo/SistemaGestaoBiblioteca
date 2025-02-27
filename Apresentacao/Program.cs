using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoBiblioteca.Aplicacao;
using SistemaGestaoBiblioteca.Dominio.Observers;
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
            var notificacaoService = new NotificacaoService();
            bibliotecaService.AdicionarObservador(notificacaoService);
            _ = new Menu(bibliotecaService);
        }
    }
}