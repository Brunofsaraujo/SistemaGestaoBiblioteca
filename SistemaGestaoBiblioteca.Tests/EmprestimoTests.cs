using SistemaGestaoBiblioteca.Dominio.Entidade;
using SistemaGestaoBiblioteca.Dominio.Enum;

namespace SistemaGestaoBiblioteca.Tests
{
    public class EmprestimoTests
    {
        private static readonly DateTime _dataEmprestimoValida = DateTime.Now;

        private static Livro CriarLivroValido()
        {
            var resultado = Livro.Criar("Clean Code", "Robert C. Martin", "9780132350884");
            Assert.True(resultado.IsSuccess, "Falha ao criar o livro para teste.");
            return resultado.Value!;
        }

        private static Usuario CriarUsuarioValido()
        {
            var resultado = Usuario.Criar(12345, "João Silva");
            Assert.True(resultado.IsSuccess, "Falha ao criar o usuário para teste.");
            return resultado.Value!;
        }

        private static Emprestimo CriarEmprestimoValido()
        {
            var livro = CriarLivroValido();
            var usuario = CriarUsuarioValido();
            var resultado = Emprestimo.Criar(livro, usuario, _dataEmprestimoValida);
            Assert.True(resultado.IsSuccess, "Falha ao criar o empréstimo para teste.");
            return resultado.Value!;
        }

        [Fact]
        public void Criar_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var livro = CriarLivroValido();
            var usuario = CriarUsuarioValido();

            // Act
            var resultado = Emprestimo.Criar(livro, usuario, _dataEmprestimoValida);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.NotNull(resultado.Value);
            Assert.Equal(livro, resultado.Value.Livro);
            Assert.Equal(usuario, resultado.Value.Usuario);
            Assert.Equal(_dataEmprestimoValida, resultado.Value.DataEmprestimo);
            Assert.Null(resultado.Value.DataDevolucao);
        }

        [Fact]
        public void Criar_DeveAlterarStatusLivroParaEmprestado_QuandoDadosValidos()
        {
            // Arrange
            var livro = CriarLivroValido();
            var usuario = CriarUsuarioValido();

            // Act
            var resultado = Emprestimo.Criar(livro, usuario, _dataEmprestimoValida);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(StatusLivro.Emprestado, livro.Status);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoLivroNulo()
        {
            // Arrange
            Livro? livro = null;
            var usuario = CriarUsuarioValido();

            // Act
            var resultado = Emprestimo.Criar(livro!, usuario, _dataEmprestimoValida);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("Deve ser informado um livro para empréstimo.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoLivroIndisponivel()
        {
            // Arrange
            var livro = CriarLivroValido();
            var usuario = CriarUsuarioValido();
            livro.AlterarStatusLivro(StatusLivro.Emprestado);

            // Act
            var resultado = Emprestimo.Criar(livro, usuario, _dataEmprestimoValida);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains($"Livro '{livro.Titulo}' está indisponível para empréstimo.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoUsuarioNulo()
        {
            // Arrange
            var livro = CriarLivroValido();
            Usuario? usuario = null;

            // Act
            var resultado = Emprestimo.Criar(livro, usuario!, _dataEmprestimoValida);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("Deve ser informado um usuário para empréstimo de livro.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoDataEmprestimoDefault()
        {
            // Arrange
            var livro = CriarLivroValido();
            var usuario = CriarUsuarioValido();
            var dataEmprestimo = default(DateTime);

            // Act
            var resultado = Emprestimo.Criar(livro, usuario, dataEmprestimo);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("Deve ser informada uma data de empréstimo do livro.", resultado.Errors);
        }

        [Fact]
        public void DevolverLivro_DeveAlterarStatusLivroEDefinirDataDevolucao_QuandoLivroEmprestado()
        {
            // Arrange
            var emprestimo = CriarEmprestimoValido();
            var livro = emprestimo.Livro;
            Assert.Equal(StatusLivro.Emprestado, livro.Status);

            // Act
            emprestimo.DevolverLivro(livro);

            // Assert
            Assert.Equal(StatusLivro.Disponivel, livro.Status);
            Assert.NotNull(emprestimo.DataDevolucao);
        }

        [Fact]
        public void DevolverLivro_DeveAdicionarNotificacao_QuandoLivroJaEstaDisponivel()
        {
            // Arrange
            var emprestimo = CriarEmprestimoValido();
            var livro = emprestimo.Livro;
            livro.AlterarStatusLivro(StatusLivro.Disponivel);

            // Act
            emprestimo.DevolverLivro(livro);

            // Assert
            Assert.Contains($"Livro {livro.Titulo} não pode ser devolvido, pois está disponível para empréstimo.",
                emprestimo.Notifications.Select(n => n.Message));
            Assert.Null(emprestimo.DataDevolucao);
        }
    }
}
