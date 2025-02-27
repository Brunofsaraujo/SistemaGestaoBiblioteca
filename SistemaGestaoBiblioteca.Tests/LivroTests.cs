using SistemaGestaoBiblioteca.Dominio.Entidade;
using SistemaGestaoBiblioteca.Dominio.Enum;

namespace SistemaGestaoBiblioteca.Tests
{
    public class LivroTests
    {
        private const string _tituloValido = "Clean Code";
        private const string _autorValido = "Robert C. Martin";
        private const string _isbnValido = "9780132350884";

        private static Livro CriarLivroValido()
        {
            var resultado = Livro.Criar(_tituloValido, _autorValido, _isbnValido);
            Assert.True(resultado.IsSuccess, "Falha ao criar o livro para teste.");
            return resultado.Value!;
        }

        [Fact]
        public void Criar_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange & Act
            var livro = CriarLivroValido();

            // Assert
            Assert.Equal(_tituloValido, livro.Titulo);
            Assert.Equal(_autorValido, livro.Autor);
            Assert.Equal(_isbnValido, livro.ISBN);
        }

        [Theory]
        [InlineData(StatusLivro.Disponivel, StatusLivro.Emprestado)]
        [InlineData(StatusLivro.Emprestado, StatusLivro.Disponivel)]
        public void AlterarStatusLivro_DeveAlterarStatus_QuandoTransicaoValida(StatusLivro statusInicial, StatusLivro statusFinal)
        {
            // Arrange
            var livro = CriarLivroValido();
            livro.AlterarStatusLivro(statusInicial);

            // Act
            livro.AlterarStatusLivro(statusFinal);

            // Assert
            Assert.Equal(statusFinal, livro.Status);
        }

        [Fact]
        public void Criar_DeveIniciarComStatusDisponivel_QuandoDadosValidos()
        {
            // Arrange & Act
            var livro = CriarLivroValido();

            // Assert
            Assert.Equal(StatusLivro.Disponivel, livro.Status);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoTituloVazio()
        {
            // Arrange
            var titulo = "";
            var autor = "Robert C. Martin";
            var isbn = "9780132350884";

            // Act
            var resultado = Livro.Criar(titulo, autor, isbn);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O Título do livro deve ser preenchido.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoAutorVazio()
        {
            // Arrange
            var titulo = "Clean Code";
            var autor = "";
            var isbn = "9780132350884";

            // Act
            var resultado = Livro.Criar(titulo, autor, isbn);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O Autor do livro deve ser preenchido.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoISBNVazio()
        {
            // Arrange
            var titulo = "Clean Code";
            var autor = "Robert C. Martin";
            var isbn = "";

            // Act
            var resultado = Livro.Criar(titulo, autor, isbn);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("ISBN do livro deve ser preenchido.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoISBNInvalido()
        {
            // Arrange
            var titulo = "Clean Code";
            var autor = "Robert C. Martin";
            var isbn = "123";

            // Act
            var resultado = Livro.Criar(titulo, autor, isbn);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains($"ISBN '{isbn}' inválido para registro do livro. Verifique as regras de ISBN.", resultado.Errors);
        }
    }
}
