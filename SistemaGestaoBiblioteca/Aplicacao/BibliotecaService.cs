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
        private readonly List<IObserver> _observadores = [];

        public void CadastrarLivro(LivroDto livroDto)
        {
            var resultado = Livro.Criar(livroDto.Titulo, livroDto.Autor, livroDto.ISBN.ToUpper());

            if (!resultado.IsSuccess)
            {
                Console.WriteLine($"\nErro ao adicionar livro {livroDto.Titulo}:");

                foreach (var erro in resultado.Errors)
                    Console.WriteLine($"--> {erro}");

                return;
            }

            var livro = resultado.Value!;

            if (_repositorioLivros.ObterTodos().Any(livroCadastrado => livroCadastrado.IsbnNormalizado.Equals(livro.IsbnNormalizado, StringComparison.CurrentCultureIgnoreCase)))
            {
                Console.WriteLine($"\nErro ao adicionar livro {livroDto.Titulo}:");
                Console.WriteLine($"--> O livro com ISBN '{livro.ISBN}' já está cadastrado.");
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
                string mensagem = $"--> Identificação '{usuario.Identificacao}' de usuário já cadastrado.";
                Console.WriteLine(mensagem);
                return;
            }

            if (usuariosCadastrados.Any(usuarioCadastrado => usuarioCadastrado.Nome.Equals(usuario.Nome, StringComparison.CurrentCultureIgnoreCase)))
            {
                Console.WriteLine($"\nErro ao adicionar usuário:");
                string mensagem = $"--> Usuário '{usuario.Nome}' já cadastrado.";
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

            var livro = _repositorioLivros.ObterTodos().FirstOrDefault(livro => livro.ISBN.Equals(emprestimoDto.ISBN, StringComparison.CurrentCultureIgnoreCase) && livro.Status == StatusLivro.Disponivel);
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
                mensagem = $"--> Usuário com identificação: {usuarioEmPosseLivro.Usuario.Identificacao} ({usuarioEmPosseLivro.Usuario.Nome}) em posse do livro com ISBN: {usuarioEmPosseLivro.Livro.ISBN} ({usuarioEmPosseLivro.Livro.Titulo}).\n    É permitido apenas 1 empréstimo simultâneo por usuário.";
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
            NotificarObservadores($"O livro '{livro.Titulo}' foi emprestado para {usuario.Nome}.");
        }

        public void DevolverLivro(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                Console.WriteLine("--> ISBN deve ser obrigatóriamente preenchido.");
                return;
            }

            string isbnNormalizado = NormalizadorISBN.NormalizarISBN(isbn);

            var livro = _repositorioLivros.ObterTodos().FirstOrDefault(livro => livro.ISBN.Equals(isbnNormalizado, StringComparison.CurrentCultureIgnoreCase));
            if (livro is null)
            {
                Console.WriteLine($"--> Livro com ISBN: {isbnNormalizado} não cadastrado.");
                return;
            }
            else if (livro.Status == StatusLivro.Disponivel)
            {
                Console.WriteLine($"{livro.Titulo} ({livro.ISBN}) não pode ser devolvido, pois não está emprestado.");
                return;
            }

            var emprestimo = _repositorioEmprestimos.ObterTodos().FirstOrDefault(emprestimo => emprestimo.Livro.ISBN.Equals(isbnNormalizado, StringComparison.CurrentCultureIgnoreCase) && emprestimo.DataDevolucao is null);
            if (emprestimo is null)
            {
                Console.WriteLine($"--> Livro: {livro.Titulo} ({livro.ISBN}) encontra-se emprestado.");
                return;
            }

            emprestimo.DevolverLivro(livro);
            Console.WriteLine("\nLivro devolvido com sucesso!");
            NotificarObservadores($"O livro '{livro.Titulo}' foi devolvido.");
        }

        public void ListarLivrosDisponiveis()
        {
            var livrosDisponiveis = _repositorioLivros.ObterTodos().Where(l => l.Status == StatusLivro.Disponivel);

            if (!livrosDisponiveis.Any())
                Console.WriteLine("Não há livros disponíveis para empréstimo.");
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
                Console.WriteLine("Não há empréstimos realizados até o momento.");
            else
                for (int indice = 0; indice < emprestimos.Count(); indice++)
                {
                    var emprestimo = emprestimos.ElementAt(indice);
                    Console.WriteLine(
                        $"{indice + 1}. Usuário: {emprestimo.Usuario.Nome} | Data empréstimo: {emprestimo.DataEmprestimo:dd-MM-yyyy} | Data devolução: {emprestimo.DataDevolucao?.ToString("dd-MM-yyyy") ?? "-"} | Livro: {emprestimo.Livro.Titulo} | ISBN: {emprestimo.Livro.ISBN}");
                }
        }

        public void AdicionarObservador(IObserver observador) => 
            _observadores.Add(observador);

        public void NotificarObservadores(string mensagem)
        {
            foreach (var observador in _observadores)
                observador.Atualizar(mensagem);
        }
    }
}
