# Sistema de Gestão de Biblioteca

Este projeto é um sistema de gestão de biblioteca desenvolvido em C#. Ele permite o cadastro de livros e usuários, empréstimo e devolução de livros, além de fornecer funcionalidades para listar livros disponíveis e consultar o histórico de empréstimos.

## Funcionalidades

- **Cadastro de Livros**: Permite cadastrar novos livros com título, autor e ISBN.
- **Cadastro de Usuários**: Permite cadastrar novos usuários com identificação e nome.
- **Empréstimo de Livros**: Permite emprestar livros para usuários cadastrados.
- **Devolução de Livros**: Permite devolver livros que foram emprestados.
- **Listagem de Livros Disponíveis**: Lista todos os livros disponíveis para empréstimo.
- **Histórico de Empréstimos**: Exibe o histórico de todos os empréstimos realizados.

## Estrutura do Projeto

O projeto é dividido em várias camadas:

- **Apresentação**: Contém a interface do usuário (UI) e o menu de interação.
- **Aplicação**: Contém a lógica de negócios e serviços.
- **Domínio**: Contém as entidades, value objects, enums e interfaces.
- **Infra**: Contém a implementação do repositório em memória.

### Principais Classes

- **BibliotecaService**: Gerencia as operações de cadastro, empréstimo e devolução de livros.
- **Menu**: Interface de linha de comando para interação com o usuário.
- **Emprestimo**: Representa um empréstimo de livro.
- **Usuario**: Representa um usuário da biblioteca.
- **Livro**: Representa um livro cadastrado na biblioteca.
- **NotificacaoService**: Serviço de notificação que exibe mensagens ao usuário.

## Como Executar o Projeto

### Pré-requisitos

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- Um ambiente de desenvolvimento como [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### Passos para Execução

1. **Clone o repositório**: git clone https://github.com/seu-usuario/SistemaGestaoBiblioteca.git
2. **Navegue até o diretório do projeto**: cd SistemaGestaoBiblioteca
3. **Restaurar pacotes NuGet**: dotnet restore
4. **Compile o projeto**: dotnet build
5. **Execute o projeto**: dotnet run --project SistemaGestaoBiblioteca.csproj

### Como Usar

1. **Cadastrar Livro**:
   - Selecione a opção "Cadastrar Livro" no menu.
   - Insira o título, autor e ISBN do livro.

2. **Cadastrar Usuário**:
   - Selecione a opção "Cadastrar Usuário" no menu.
   - Insira a identificação e o nome do usuário.

3. **Emprestar Livro**:
   - Selecione a opção "Emprestar Livro" no menu.
   - Insira o ISBN do livro e a identificação do usuário.

4. **Devolver Livro**:
   - Selecione a opção "Devolver Livro" no menu.
   - Insira o ISBN do livro a ser devolvido.

5. **Listar Livros Disponíveis**:
   - Selecione a opção "Listar Livros Disponíveis" no menu.
   - Veja a lista de livros disponíveis para empréstimo.

6. **Histórico de Empréstimos**:
   - Selecione a opção "Obter histórico de empréstimos" no menu.
   - Veja o histórico de todos os empréstimos realizados.

### Tecnologias Utilizadas

- **.NET 9.0**: Framework utilizado para desenvolvimento.
- **Flunt**: Biblioteca para validação de entidades.
- **Microsoft.Extensions.DependencyInjection**: Para injeção de dependências.

### Licença
Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.