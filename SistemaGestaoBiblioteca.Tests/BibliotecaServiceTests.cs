using SistemaGestaoBiblioteca.Aplicacao;
using SistemaGestaoBiblioteca.Dominio.DTO;
using SistemaGestaoBiblioteca.Dominio.Entidade;
using SistemaGestaoBiblioteca.Dominio.Enum;
using SistemaGestaoBiblioteca.Dominio.Interface;
using SistemaGestaoBiblioteca.Infra;

namespace SistemaGestaoBiblioteca.Tests
{
    public class BibliotecaServiceTests
    {
        private readonly InMemoryRepository<Livro> _repositorioLivros;
        private readonly InMemoryRepository<Usuario> _repositorioUsuarios;
        private readonly InMemoryRepository<Emprestimo> _repositorioEmprestimos;
        private readonly BibliotecaService _bibliotecaService;
        private readonly MockConsoleOutput _consoleOutput;
        private readonly MockObserver _mockObserver;

        public BibliotecaServiceTests()
        {
            _repositorioLivros = new InMemoryRepository<Livro>();
            _repositorioUsuarios = new InMemoryRepository<Usuario>();
            _repositorioEmprestimos = new InMemoryRepository<Emprestimo>();
            _bibliotecaService = new BibliotecaService(_repositorioLivros, _repositorioUsuarios, _repositorioEmprestimos);
            _consoleOutput = new MockConsoleOutput();
            _mockObserver = new MockObserver();
            _bibliotecaService.AdicionarObservador(_mockObserver);
        }

        #region CadastrarLivro

        [Fact]
        public void CadastrarLivro_DeveAdicionarLivro_QuandoDadosValidos()
        {
            // Arrange
            var livroDto = new LivroDto
            (
                titulo: "Clean Code",
                autor: "Robert C. Martin",
                isbn: "9780132350884"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarLivro(livroDto);
            }

            // Assert
            var livros = _repositorioLivros.ObterTodos();
            Assert.Single(livros);
            Assert.Equal(livroDto.Titulo, livros.First().Titulo);
            Assert.Equal(livroDto.Autor, livros.First().Autor);
            Assert.Equal(livroDto.ISBN.ToUpper(), livros.First().ISBN);
            Assert.Contains("Livro cadastrado com sucesso!", _consoleOutput.GetOutput());
        }

        [Fact]
        public void CadastrarLivro_NaoDeveAdicionarLivro_QuandoDadosInvalidos()
        {
            // Arrange
            var livroDto = new LivroDto
            (
                titulo: "",
                autor: "Robert C. Martin",
                isbn: "9780132350884"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarLivro(livroDto);
            }

            // Assert
            Assert.Empty(_repositorioLivros.ObterTodos());
            Assert.Contains("Erro ao adicionar livro", _consoleOutput.GetOutput());
            Assert.Contains("O Título do livro deve ser preenchido", _consoleOutput.GetOutput());
        }

        [Fact]
        public void CadastrarLivro_NaoDeveAdicionarLivro_QuandoISBNJaCadastrado()
        {
            // Arrange
            var livroDto1 = new LivroDto
            (
                titulo: "Clean Code",
                autor: "Robert C. Martin",
                isbn: "9780132350884"
            );

            var livroDto2 = new LivroDto
            (
                titulo: "Outro Livro",
                autor: "Outro Autor",
                isbn: "9780132350884"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarLivro(livroDto1);
                _bibliotecaService.CadastrarLivro(livroDto2);
            }

            // Assert
            var livros = _repositorioLivros.ObterTodos();
            Assert.Single(livros);
            Assert.Equal(livroDto1.Titulo, livros.First().Titulo);
            Assert.Contains("O livro com ISBN '9780132350884' já está cadastrado", _consoleOutput.GetOutput());
        }

        #endregion

        #region CadastrarUsuario

        [Fact]
        public void CadastrarUsuario_DeveAdicionarUsuario_QuandoDadosValidos()
        {
            // Arrange
            var usuarioDto = new UsuarioDto
            (
                identificacao: 12345,
                nome: "João Silva"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarUsuario(usuarioDto);
            }

            // Assert
            var usuarios = _repositorioUsuarios.ObterTodos();
            Assert.Single(usuarios);
            Assert.Equal(usuarioDto.Identificacao, usuarios.First().Identificacao);
            Assert.Equal(usuarioDto.Nome, usuarios.First().Nome);
            Assert.Contains("Usuário cadastrado com sucesso!", _consoleOutput.GetOutput());
        }

        [Fact]
        public void CadastrarUsuario_NaoDeveAdicionarUsuario_QuandoDadosInvalidos()
        {
            // Arrange
            var usuarioDto = new UsuarioDto
            (
                identificacao: 12345,
                nome: ""
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarUsuario(usuarioDto);
            }

            // Assert
            Assert.Empty(_repositorioUsuarios.ObterTodos());
            Assert.Contains("Erro ao adicionar usuário", _consoleOutput.GetOutput());
            Assert.Contains("O Nome do usuário deve ser preenchido", _consoleOutput.GetOutput());
        }

        [Fact]
        public void CadastrarUsuario_NaoDeveAdicionarUsuario_QuandoIdentificacaoJaCadastrada()
        {
            // Arrange
            var usuarioDto1 = new UsuarioDto
            (
                identificacao: 12345,
                nome: "João Silva"
            );

            var usuarioDto2 = new UsuarioDto
            (
                identificacao: 12345,
                nome: "Maria Souza"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarUsuario(usuarioDto1);
                _bibliotecaService.CadastrarUsuario(usuarioDto2);
            }

            // Assert
            var usuarios = _repositorioUsuarios.ObterTodos();
            Assert.Single(usuarios);
            Assert.Equal(usuarioDto1.Nome, usuarios.First().Nome);
            Assert.Contains("Identificação '12345' de usuário já cadastrado", _consoleOutput.GetOutput());
        }

        [Fact]
        public void CadastrarUsuario_NaoDeveAdicionarUsuario_QuandoNomeJaCadastrado()
        {
            // Arrange
            var usuarioDto1 = new UsuarioDto
            (
                identificacao: 12345,
                nome: "João Silva"
            );

            var usuarioDto2 = new UsuarioDto
            (
                identificacao: 54321,
                nome: "João Silva"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarUsuario(usuarioDto1);
                _bibliotecaService.CadastrarUsuario(usuarioDto2);
            }

            // Assert
            var usuarios = _repositorioUsuarios.ObterTodos();
            Assert.Single(usuarios);
            Assert.Equal(usuarioDto1.Identificacao, usuarios.First().Identificacao);
            Assert.Contains("Usuário 'João Silva' já cadastrado", _consoleOutput.GetOutput());
        }

        #endregion

        #region EmprestarLivro

        [Fact]
        public void EmprestarLivro_DeveRealizarEmprestimo_QuandoDadosValidos()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            var emprestimos = _repositorioEmprestimos.ObterTodos();
            Assert.Single(emprestimos);
            Assert.Equal("9780132350884", emprestimos.First().Livro.ISBN);
            Assert.Equal(12345, emprestimos.First().Usuario.Identificacao);
            Assert.Null(emprestimos.First().DataDevolucao);
            Assert.Contains("Empréstimo realizado com sucesso!", _consoleOutput.GetOutput());
            Assert.Equal("O livro 'Clean Code' foi emprestado para João Silva.", _mockObserver.UltimaMensagem);
        }

        [Fact]
        public void EmprestarLivro_NaoDeveRealizarEmprestimo_QuandoISBNVazio()
        {
            // Arrange
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            Assert.Empty(_repositorioEmprestimos.ObterTodos());
            Assert.Contains("Deve ser informado o ISBN do livro", _consoleOutput.GetOutput());
        }

        [Fact]
        public void EmprestarLivro_NaoDeveRealizarEmprestimo_QuandoIdentificacaoInvalida()
        {
            // Arrange
            CadastrarLivroValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 0
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            Assert.Empty(_repositorioEmprestimos.ObterTodos());
            Assert.Contains("Deve ser informada a identificação do usuário", _consoleOutput.GetOutput());
        }

        [Fact]
        public void EmprestarLivro_NaoDeveRealizarEmprestimo_QuandoLivroNaoEncontrado()
        {
            // Arrange
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            Assert.Empty(_repositorioEmprestimos.ObterTodos());
            Assert.Contains("Livro com ISBN: 9780132350884 indisponível ou não encontrado para empréstimo", _consoleOutput.GetOutput());
        }

        [Fact]
        public void EmprestarLivro_NaoDeveRealizarEmprestimo_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            CadastrarLivroValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            Assert.Empty(_repositorioEmprestimos.ObterTodos());
            Assert.Contains("Usuário com identificação: 12345 não encontrado", _consoleOutput.GetOutput());
        }

        [Fact]
        public void EmprestarLivro_NaoDeveRealizarEmprestimo_QuandoUsuarioJaPossuiEmprestimo()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();

            // Cadastrar segundo livro
            var segundoLivroDto = new LivroDto
            (
                titulo: "Domain-Driven Design",
                autor: "Eric Evans",
                isbn: "0321125215"
            );

            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarLivro(segundoLivroDto);

                // Realizar primeiro empréstimo
                var primeiroEmprestimoDto = new EmprestimoDto
                (
                    isbn: "9780132350884",
                    identificacao: 12345
                );

                //Tentar realizar segundo empréstimo
                var segundoEmprestimoDto = new EmprestimoDto
                (
                    isbn: "0321125215",
                    identificacao: 12345
                );

                // Act
                _bibliotecaService.EmprestarLivro(primeiroEmprestimoDto);
                _bibliotecaService.EmprestarLivro(segundoEmprestimoDto);
            }

            // Assert
            var emprestimos = _repositorioEmprestimos.ObterTodos();
            Assert.Single(emprestimos);
            Assert.Equal("9780132350884", emprestimos.First().Livro.ISBN);
            Assert.Contains("É permitido apenas 1 empréstimo simultâneo por usuário", _consoleOutput.GetOutput());
        }

        #endregion

        #region DevolverLivro

        [Fact]
        public void DevolverLivro_DeveRealizarDevolucao_QuandoLivroEmprestado()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
                _bibliotecaService.DevolverLivro("9780132350884");
            }

            // Assert
            var emprestimos = _repositorioEmprestimos.ObterTodos();
            Assert.NotNull(emprestimos.First().DataDevolucao);
            var livro = _repositorioLivros.ObterTodos().First();
            Assert.Equal(StatusLivro.Disponivel, livro.Status);
            Assert.Contains("Livro devolvido com sucesso!", _consoleOutput.GetOutput());
            Assert.Equal("O livro 'Clean Code' foi devolvido.", _mockObserver.UltimaMensagem);
        }

        [Fact]
        public void DevolverLivro_NaoDeveRealizarDevolucao_QuandoISBNVazio()
        {
            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.DevolverLivro("");
            }

            // Assert
            Assert.Contains("ISBN deve ser obrigatóriamente preenchido", _consoleOutput.GetOutput());
        }

        [Fact]
        public void DevolverLivro_NaoDeveRealizarDevolucao_QuandoLivroNaoEncontrado()
        {
            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.DevolverLivro("9780132350884");
            }

            // Assert
            Assert.Contains("Livro com ISBN: 9780132350884 não cadastrado", _consoleOutput.GetOutput());
        }

        [Fact]
        public void DevolverLivro_NaoDeveRealizarDevolucao_QuandoLivroNaoEstaEmprestado()
        {
            // Arrange
            CadastrarLivroValido();

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.DevolverLivro("9780132350884");
            }

            // Assert
            Assert.Contains("não pode ser devolvido, pois não está emprestado", _consoleOutput.GetOutput());
        }

        #endregion

        #region ListarLivrosDisponiveis

        [Fact]
        public void ListarLivrosDisponiveis_DeveExibirMensagem_QuandoNaoHaLivrosDisponiveis()
        {
            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.ListarLivrosDisponiveis();
            }

            // Assert
            Assert.Contains("Não há livros disponíveis para empréstimo", _consoleOutput.GetOutput());
        }

        [Fact]
        public void ListarLivrosDisponiveis_DeveListarLivrosDisponiveis_QuandoExistemLivros()
        {
            // Arrange
            CadastrarLivroValido();

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.ListarLivrosDisponiveis();
            }

            // Assert
            Assert.Contains("1. Título: Clean Code | Autor: Robert C. Martin | ISBN: 9780132350884", _consoleOutput.GetOutput());
        }

        [Fact]
        public void ListarLivrosDisponiveis_DeveListarApenasLivrosDisponiveis_QuandoExistemLivrosEmprestados()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();
            
            // Cadastrar segundo livro
            var segundoLivroDto = new LivroDto
            (
                titulo: "Domain-Driven Design",
                autor: "Eric Evans",
                isbn: "0321125215"
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.CadastrarLivro(segundoLivroDto);

                // Emprestar primeiro livro
                var emprestimoDto = new EmprestimoDto
                (
                    isbn: "9780132350884",
                    identificacao: 12345
                );

                _bibliotecaService.EmprestarLivro(emprestimoDto);
                _bibliotecaService.ListarLivrosDisponiveis();
            }

            // Assert
            var output = _consoleOutput.GetOutput();
            Assert.DoesNotContain("Clean Code", output);
            Assert.Contains("1. Título: Domain-Driven Design | Autor: Eric Evans | ISBN: 0321125215", output);
        }

        #endregion

        #region ObterHistoricoEmprestimos

        [Fact]
        public void ObterHistoricoEmprestimos_DeveExibirMensagem_QuandoNaoHaEmprestimos()
        {
            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.ObterHistoricoEmprestimos();
            }

            // Assert
            Assert.Contains("Não há empréstimos realizados até o momento", _consoleOutput.GetOutput());
        }

        [Fact]
        public void ObterHistoricoEmprestimos_DeveListarEmprestimos_QuandoExistemEmprestimos()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
                _bibliotecaService.ObterHistoricoEmprestimos();
            }

            // Assert
            var output = _consoleOutput.GetOutput();
            Assert.Contains("1. Usuário: João Silva | Data empréstimo:", output);
            Assert.Contains("| Data devolução: - | Livro: Clean Code | ISBN: 9780132350884", output);
        }

        [Fact]
        public void ObterHistoricoEmprestimos_DeveListarEmprestimosComDevolucao_QuandoExistemEmprestimosDevolvidos()
        {
            // Arrange
            CadastrarLivroValido();
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
                _bibliotecaService.DevolverLivro("9780132350884");
                _bibliotecaService.ObterHistoricoEmprestimos();
            }

            // Assert
            var output = _consoleOutput.GetOutput();
            Assert.Contains("1. Usuário: João Silva | Data empréstimo:", output);
            Assert.DoesNotContain("| Data devolução: - |", output);
            Assert.Contains("| Livro: Clean Code | ISBN: 9780132350884", output);
        }

        #endregion

        #region Observadores

        [Fact]
        public void AdicionarObservador_DeveNotificarObservadores_QuandoEmprestimoRealizado()
        {
            // Arrange
            var observador = new MockObserver();
            _bibliotecaService.AdicionarObservador(observador);

            CadastrarLivroValido();
            CadastrarUsuarioValido();

            var emprestimoDto = new EmprestimoDto
            (
                isbn: "9780132350884",
                identificacao: 12345
            );

            // Act
            using (_consoleOutput.Capture())
            {
                _bibliotecaService.EmprestarLivro(emprestimoDto);
            }

            // Assert
            Assert.Equal("O livro 'Clean Code' foi emprestado para João Silva.", observador.UltimaMensagem);
        }

        [Fact]
        public void AdicionarObservador_DeveNotificarObservadores_QuandoDevolucaoRealizada()
        {
            // Arrange
            var observador = new MockObserver();
            _bibliotecaService.AdicionarObservador(observador);

            using (_consoleOutput.Capture())
            {
                CadastrarLivroValido();
                CadastrarUsuarioValido();

                var emprestimoDto = new EmprestimoDto
                (
                    isbn: "9780132350884",
                    identificacao: 12345
                );

                // Act
                _bibliotecaService.EmprestarLivro(emprestimoDto);
                _bibliotecaService.DevolverLivro("9780132350884");
            }

            // Assert
            Assert.Equal("O livro 'Clean Code' foi devolvido.", observador.UltimaMensagem);
        }

        #endregion

        #region Métodos Auxiliares

        private void CadastrarLivroValido()
        {
            var livroDto = new LivroDto
            (
                titulo: "Clean Code",
                autor: "Robert C. Martin",
                isbn: "9780132350884"
            );

            _bibliotecaService.CadastrarLivro(livroDto);
        }

        private void CadastrarUsuarioValido()
        {
            var usuarioDto = new UsuarioDto
            (
                identificacao: 12345,
                nome: "João Silva"
            );

            _bibliotecaService.CadastrarUsuario(usuarioDto);
        }

        #endregion
    }

    public class MockConsoleOutput : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;

        public MockConsoleOutput()
        {
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
        }

        public IDisposable Capture()
        {
            Console.SetOut(_stringWriter);
            return this;
        }

        public string GetOutput()
        {
            return _stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            _stringWriter.Dispose();
        }
    }

    public class MockObserver : IObserver
    {
        public string UltimaMensagem { get; private set; } = string.Empty;

        public void Atualizar(string mensagem)
        {
            UltimaMensagem = mensagem;
        }
    }
}