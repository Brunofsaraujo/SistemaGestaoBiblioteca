using SistemaGestaoBiblioteca.Aplicacao;
using SistemaGestaoBiblioteca.Dominio.DTO;
using SistemaGestaoBiblioteca.Dominio.Enum;
using SistemaGestaoBiblioteca.Dominio.ValueObjects;

namespace SistemaGestaoBiblioteca.Apresentacao
{
    public class Menu
    {
        private readonly BibliotecaService _bibliotecaService;

        public Menu(BibliotecaService bibliotecaService)
        {
            _bibliotecaService = bibliotecaService;
            Exibir();
        }

        public void Exibir()
        {
            try
            {
                bool sair = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("=== Sistema de Gestão de Biblioteca ===\n");
                    Console.WriteLine("1. Cadastrar Livro");
                    Console.WriteLine("2. Cadastrar Usuário");
                    Console.WriteLine("3. Emprestar Livro");
                    Console.WriteLine("4. Devolver Livro");
                    Console.WriteLine("5. Listar Livros Disponíveis");
                    Console.WriteLine("6. Obter histórico de empréstimos");
                    Console.WriteLine("7. Sair");
                    Console.Write("\nSelecione uma opção: ");

                    _ = Enum.TryParse(Console.ReadLine(), out OpcaoMenu opcao);
                    switch (opcao)
                    {
                        case OpcaoMenu.CadastrarLivro:
                            CadastrarLivro();
                            break;
                        case OpcaoMenu.CadastrarUsuario:
                            CadastrarUsuario();
                            break;
                        case OpcaoMenu.EmprestarLivro:
                            EmprestarLivro();
                            break;
                        case OpcaoMenu.DevolverLivro:
                            DevolverLivro();
                            break;
                        case OpcaoMenu.ListarLivrosDisponiveis:
                            ListarLivrosDisponiveis();
                            break;
                        case OpcaoMenu.ObterHistoricoEmprestimos:
                            ObterHistoricoEmprestimos();
                            break;
                        case OpcaoMenu.Sair:
                            sair = true;
                            break;
                        default:
                            Console.WriteLine("\nOpção inválida. Pressione qualquer tecla para tentar novamente.");
                            Console.ReadKey();
                            break;
                    }
                } while (!sair);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao selecionar uma opção do menu: Retorno. {ex.Message}");
            }
        }

        private void CadastrarLivro()
        {
            try
            {
                Console.WriteLine("\n=== Cadastro de Livro ===\n");
                string titulo = LerEntradaTexto("Título: ");
                string autor = LerEntradaTexto("Autor: ");
                string isbn = LerEntradaTexto("ISBN: ");

                var livro = new LivroDto(
                    titulo: titulo,
                    autor: autor,
                    isbn: NormalizadorISBN.NormalizarISBN(isbn)
                );

                _bibliotecaService.CadastrarLivro(livro);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao cadastrar livro: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private void CadastrarUsuario()
        {
            try
            {
                Console.WriteLine("\n=== Cadastro de Usuário ===\n");
                int identificacao = LerEntradaInteira("Número identificação: ");
                string nome = LerEntradaTexto("Nome: ");

                var usuario = new UsuarioDto(
                    identificacao: identificacao,
                    nome: nome
                );

                _bibliotecaService.CadastrarUsuario(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao cadastrar usuário: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private void EmprestarLivro()
        {
            try
            {
                Console.WriteLine("\n=== Empréstimo de Livro ===\n");
                string isbn = LerEntradaTexto("Informe o ISBN do livro: ");
                int identificacao = LerEntradaInteira("Informe o ID do usuário: ");

                var emprestimo = new EmprestimoDto(
                    isbn: NormalizadorISBN.NormalizarISBN(isbn),
                    identificacao: identificacao
                );

                _bibliotecaService.EmprestarLivro(emprestimo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao realizar empréstimo: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private void DevolverLivro()
        {
            try
            {
                Console.WriteLine("\n=== Devolução de Livro ===\n");
                string isbn = LerEntradaTexto("Informe o ISBN do livro: ");

                _bibliotecaService.DevolverLivro(NormalizadorISBN.NormalizarISBN(isbn));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao devolver livro: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private void ListarLivrosDisponiveis()
        {
            try
            {
                Console.WriteLine("\n=== Livros Disponíveis para empréstimo ===\n");

                _bibliotecaService.ListarLivrosDisponiveis();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao listar livros disponíveis para empréstimo: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private void ObterHistoricoEmprestimos()
        {
            try
            {
                Console.WriteLine("\n=== Histórico de empréstimos ===\n");

                _bibliotecaService.ObterHistoricoEmprestimos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao obter histórico de empréstimos: Retorno. {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para retornar ao menu.");
            Console.ReadKey();
        }

        private static string LerEntradaTexto(string mensagem)
        {
            Console.Write(mensagem);
            return Console.ReadLine() ?? string.Empty;
        }

        private static int LerEntradaInteira(string mensagem)
        {
            Console.Write(mensagem);
            string entrada = Console.ReadLine() ?? string.Empty;
            _ = int.TryParse(entrada, out int valor);
            return valor;
        }
    }
}
