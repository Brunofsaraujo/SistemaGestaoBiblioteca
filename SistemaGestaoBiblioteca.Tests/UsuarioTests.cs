using SistemaGestaoBiblioteca.Dominio.Entidade;

namespace SistemaGestaoBiblioteca.Tests
{
    public class UsuarioTests
    {
        private const int _identificacaoValida = 12345;
        private const string _nomeValido = "João Silva";

        private static Usuario CriarUsuarioValido()
        {
            var resultado = Usuario.Criar(_identificacaoValida, _nomeValido);
            Assert.True(resultado.IsSuccess, "Falha ao criar o usuário para teste.");
            return resultado.Value!;
        }

        [Fact]
        public void Criar_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange & Act
            var usuario = CriarUsuarioValido();

            // Assert
            Assert.Equal(_identificacaoValida, usuario.Identificacao);
            Assert.Equal(_nomeValido, usuario.Nome);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoIdentificacaoZero()
        {
            // Arrange
            var identificacao = 0;
            var nome = "João Silva";

            // Act
            var resultado = Usuario.Criar(identificacao, nome);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O usuário deve possuir um número de identificação.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoIdentificacaoNegativa()
        {
            // Arrange
            var identificacao = -1;
            var nome = "João Silva";

            // Act
            var resultado = Usuario.Criar(identificacao, nome);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O usuário deve possuir um número de identificação.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoNomeVazio()
        {
            // Arrange
            var identificacao = 12345;
            var nome = "";

            // Act
            var resultado = Usuario.Criar(identificacao, nome);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O Nome do usuário deve ser preenchido.", resultado.Errors);
        }

        [Fact]
        public void Criar_DeveRetornarFalha_QuandoNomeNull()
        {
            // Arrange
            var identificacao = 12345;
            string? nome = null;

            // Act
            var resultado = Usuario.Criar(identificacao, nome!);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O Nome do usuário deve ser preenchido.", resultado.Errors);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Criar_DeveRetornarFalha_QuandoNomeApenasEspacos(string nome)
        {
            // Arrange
            var identificacao = 12345;

            // Act
            var resultado = Usuario.Criar(identificacao, nome);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Contains("O Nome do usuário deve ser preenchido.", resultado.Errors);
        }
    }
}
