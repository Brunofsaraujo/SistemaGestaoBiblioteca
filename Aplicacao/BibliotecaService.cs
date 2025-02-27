using SistemaGestaoBiblioteca.Dominio.DTO;
using SistemaGestaoBiblioteca.Dominio.Entidade;
using SistemaGestaoBiblioteca.Dominio.Enum;
using SistemaGestaoBiblioteca.Dominio.Interface;
using SistemaGestaoBiblioteca.Dominio.ValueObjects;
using SistemaGestaoBiblioteca.Infra;

namespace SistemaGestaoBiblioteca.Aplicacao
{
    public class BibliotecaService(
        InMemoryRepository<Livro> repositorioLivros,
        InMemoryRepository<Usuario> repositorioUsuarios,
        InMemoryRepository<Emprestimo> repositorioEmprestimos) : IObservable
    {
        private readonly InMemoryRepository<Livro> _repositorioLivros = repositorioLivros;
        private readonly InMemoryRepository<Usuario> _repositorioUsuarios = repositorioUsuarios;
        private readonly InMemoryRepository<Emprestimo> _repositorioEmprestimos = repositorioEmprestimos;
        private readonly List<IObserver> _observers = [];

        public void CadastrarLivro(LivroDto livroDto)
        {
            var resultado = Livro.Criar(livroDto.Titulo, livroDto.Autor, livroDto.ISBN);

            if (!resultado.IsSuccess)
            {
                Console.WriteLine($"\nErro ao adicionar livro {livroDto.Titulo}:");

                foreach (var erro in resultado.Errors)
                    Console.WriteLine($"--> {erro}");

                return;
            }

            var livro = resultado.Value!;

            if (_repositorioLivros.ObterTodos().Any(livroCadastrado => livroCadastrado.IsbnNormalizado == livro.IsbnNormalizado))
            {
                Console.WriteLine($"\nErro: O livro com ISBN '{livro.ISBN}' já está cadastrado.");
                return;
            }

            _repositorioLivros.Adicionar(livro);
            Console.WriteLine("\nLivro cadastrado com sucesso!");
        }

        public void CadastrarUsuario(UsuarioDto usuarioDto)
        {
            var resultado = Usuario.Criar(usuarioDto.Identificacao, usuarioDto.Nome);

            if (!resultado.IsSuccess)
            {
                Console.WriteLine($"\nErro ao adicionar usuário:");

                foreach (var erro in resultado.Errors)
                    Console.WriteLine($"--> {erro}");

                return;
            }

            var usuario = resultado.Value!;
            var usuariosCadastrados = _repositorioUsuarios.ObterTodos();

            if (usuariosCadastrados.Any(usuarioCadastrado => usuarioCadastrado.Identificacao == usuario.Identificacao))
            {
                Console.WriteLine($"\nErro ao adicionar usuário:");
                string mensagem = $"\nIdentificação '{usuario.Identificacao}' de usuário já cadastrado.";
                Console.WriteLine(mensagem);
                return;
            }

            if (usuariosCadastrados.Any(usuarioCadastrado => usuarioCadastrado.Nome == usuario.Nome))
            {
                Console.WriteLine($"\nErro ao adicionar usuário:");
                string mensagem = $"\nUsuário '{usuario.Nome}' já cadastrado.";
                Console.WriteLine(mensagem);
                return;
            }

            _repositorioUsuarios.Adicionar(usuario);
            Console.WriteLine("\nUsuário cadastrado com sucesso!");
        }

        public void EmprestarLivro(EmprestimoDto emprestimoDto)
        {
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(emprestimoDto.ISBN))
            {
                Console.WriteLine($"\nErro ao realizar empréstimo de livro:");
                mensagem = "--> Deve ser informado o ISBN do livro.";
                Console.WriteLine(mensagem);
                return;
            }

            if (emprestimoDto.Identificacao <= 0)
            {
                Console.WriteLine($"\nErro ao realizar empréstimo de livro:");
                mensagem = "--> Deve ser informada a identificação do usuário.";
                Console.WriteLine(mensagem);
                return;
            }

            var livro = _repositorioLivros.ObterTodos().FirstOrDefault(livro => livro.ISBN == emprestimoDto.ISBN && livro.Status == StatusLivro.Disponivel);
            if (livro is null)
            {
                Console.WriteLine($"\nErro ao realizar empréstimo de livro:");
                mensagem = $"--> Livro com ISBN: {emprestimoDto.ISBN} indisponível ou não encontrado para empréstimo.";
                Console.WriteLine(mensagem);
                return;
            }

            var usuario = _repositorioUsuarios.ObterTodos().FirstOrDefault(usuario => usuario.Identificacao == emprestimoDto.Identificacao);
            if (usuario is null)
            {
                Console.WriteLine($"\nErro ao realizar empréstimo de livro:");
                mensagem = $"--> Usuário com identificação: {emprestimoDto.Identificacao} não encontrado.";
                Console.WriteLine(mensagem);
                return;
            }

            var usuarioEmPosseLivro = _repositorioEmprestimos.ObterTodos().FirstOrDefault(emprestimo => emprestimo.Usuario.Identificacao == emprestimoDto.Identificacao && emprestimo.DataDevolucao is null);
            if (usuarioEmPosseLivro != null)
            {
                Console.WriteLine($"\nErro ao realizar empréstimo de livro:");
                mensagem = $"--> Usuário com identificação: {usuarioEmPosseLivro.Usuario.Identificacao} ({usuarioEmPosseLivro.Usuario.Nome}) em posse do livro {usuarioEmPosseLivro.Livro.ISBN} ({usuarioEmPosseLivro.Livro.Titulo}).\n    É permitido apenas 1 empréstimo simultâneo por usuário.";
                Console.WriteLine(mensagem);
                return;
            }

            var resultado = Emprestimo.Criar(
                livro: livro,
                usuario: usuario,
                dataEmprestimo: DateTime.Now);

            if (!resultado.IsSuccess)
            {
                Console.WriteLine($"\nErro ao realizar empréstimo:");

                foreach (var erro in resultado.Errors)
                    Console.WriteLine($"--> {erro}");

                return;
            }

            var emprestimo = resultado.Value!;
            _repositorioEmprestimos.Adicionar(emprestimo);
            Console.WriteLine("\nEmpréstimo realizado com sucesso!");
            //NotificarObservers($"O livro '{livro.Titulo}' foi emprestado para {usuario.Nome}.");
        }

        public void DevolverLivro(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                Console.WriteLine("ISBN deve ser obrigatóriamente preenchido.");
                return;
            }

            string isbnNormalizado = NormalizadorISBN.NormalizarISBN(isbn);

            var livro = _repositorioLivros.ObterTodos().FirstOrDefault(livro => livro.ISBN == isbnNormalizado);
            if (livro is null)
            {
                Console.WriteLine($"Livro com ISBN: {isbnNormalizado} não cadastrado.");
                return;
            }
            else if (livro.Status == StatusLivro.Disponivel)
            {
                Console.WriteLine($"{livro.Titulo} ({livro.ISBN}) não pode ser devolvido, pois não está emprestado.");
                return;
            }

            var emprestimo = _repositorioEmprestimos.ObterTodos().FirstOrDefault(emprestimo => emprestimo.Livro.ISBN == isbnNormalizado && emprestimo.DataDevolucao is null);
            if (emprestimo is null)
            {
                Console.WriteLine($"Livro: {livro.Titulo} ({livro.ISBN}) encontra-se emprestado.");
                return;
            }

            emprestimo.DevolverLivro(livro);
            Console.WriteLine("\nLivro devolvido com sucesso!");
            //NotificarObservers($"O livro '{livro.Titulo}' foi devolvido e está disponível.");
        }

        public void ListarLivrosDisponiveis()
        {
            var livrosDisponiveis = _repositorioLivros.ObterTodos().Where(l => l.Status == StatusLivro.Disponivel);

            if (!livrosDisponiveis.Any())
                Console.WriteLine("Não há livros disponíveis para empréstimo.\n");
            else
                for (int indice = 0; indice < livrosDisponiveis.Count(); indice++)
                {
                    var livro = livrosDisponiveis.ElementAt(indice);
                    Console.WriteLine($"{indice + 1}. Título: {livro.Titulo} | Autor: {livro.Autor} | ISBN: {livro.ISBN}");
                }
        }

        public void ObterHistoricoEmprestimos()
        {
            var emprestimos = _repositorioEmprestimos.ObterTodos();
            if (!emprestimos.Any())
                Console.WriteLine("Não há empréstimos realizados até o momento.\n");
            else
                for (int indice = 0; indice < emprestimos.Count(); indice++)
                {
                    var emprestimo = emprestimos.ElementAt(indice);
                    Console.WriteLine(
                        $"{indice + 1}. Nome usuário: {emprestimo.Usuario.Nome} | Data empréstimo: {emprestimo.DataEmprestimo:dd-MM-yyyy} | Data devolução: {emprestimo.DataDevolucao?.ToString("dd-MM-yyyy") ?? "-"} | Nome do livro: {emprestimo.Livro.Titulo} | ISBN: {emprestimo.Livro.ISBN}");
                }
        }

        public void RegistrarObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoverObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotificarObservers(string mensagem)
        {
            foreach (var observer in _observers)
            {
                observer.Update(mensagem);
            }
        }
    }
}
