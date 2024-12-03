using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class LivroController : Controller
    {
        private readonly ILogger<LivroController> _logger;

        public LivroController(ILogger<LivroController> logger)
        {
            _logger = logger;
        }
        
            Context context = new Context();
        public IActionResult Index()
        {
             ViewBag.Admin = HttpContext.Session.GetString("Admin")!;
            
            //Criar uma lista de livros
            List<Livro> listaLivros = context.Livro.ToList();

            //Verificar se o livro tem reserva ou nao
            var livrosReservados = context.LivroReserva.ToDictionary(Livro => Livro.LivroID, livror => livror.DtReserva);
            
            ViewBag.Livros = listaLivros;
            ViewBag.LivrosComReserva = livrosReservados;
            return View();

        }

        [Route("Cadastro")]
        //Metodo que retorna a tela de cadastro:
        public IActionResult  Cadastro(){
                    
             ViewBag.Admin = HttpContext.Session.GetString("Admin")!;
          
            ViewBag.Categorias = context.Categoria.ToList();
            //Retorna a view de cadastro:
            return View();
        }

        //metodo para cadastrar um livro:
        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form){
            //PRIMEIRA PARTE: CADASTRAR UM LIVRO NA TABELA LIVRO
            Livro novoLivro = new Livro();
            
            //O que meu usuario escrever no formulario sera atribuido ao novoLivro

            novoLivro.Nome =form ["Nome"].ToString();
            novoLivro.Descricao = form["Descricao"].ToString();
            novoLivro.Editora=form["Editora"].ToString();
            novoLivro.Escritor=form["Escritor"].ToString();
            novoLivro.Idioma=form["Idioma"].ToString();
            //Trabalhar com imagens:
            if(form.Files.Count > 0){
                //Primeiro passo:
                // Armazenaremos o arquivo enviado pelo usuario
                var arquivo = form.Files[0];

                //Segundo passo:
                //Criar variavel do caminho da minha pasta para colocar as fotos dos livros
                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Livros");
                //Validaremos se a pasta que sera armazenada as imagens, existe. Caso nao exista, criaremos uma nova pasta.
                if(!Directory.Exists(pasta)){
                        //Criar a pasta:
                        Directory.CreateDirectory(pasta);
                }   
                //Terceiro passo:
                //Criar a variavel para armazenar o caminho em que meu arquivo estara, alem do nome dele
                var caminho = Path.Combine(pasta,arquivo.FileName);
                
                using(var stream = new FileStream(caminho, FileMode.Create)){
                    //Copiou o arquivo para o meu diretorio
                    arquivo.CopyTo(stream);
                }

                novoLivro.Imagem = arquivo.FileName;
            }else{
                novoLivro.Imagem = "padrao.png";
            }

            context.Livro.Add(novoLivro);
            context.SaveChanges();

            // SEGUNDA PARTE: E ADICIONAR  DENTRO DE LIVROCATEGORIA A CATEGORIA QUE PERTENCE AO NOVOLIVRO 
            //Lista as categorias:
            List<LivroCategoria> ListaLivroCategorias = new List<LivroCategoria>(); //Lista as categorias:

            //Array que possui as categorias selecionadas pelo usuario
            string[] categoriasSelecionadas = form["Categoria"].ToString().Split(',');
            //acao, terror, suspense

            foreach(string categoria in categoriasSelecionadas){
                //string categoria possui a informacao do id da categoria Atual selecionada.

                LivroCategoria livroCategoria = new LivroCategoria();
                livroCategoria.CategoriaID = int.Parse(categoria);
                livroCategoria.LivroID = novoLivro.LivroID;

                ListaLivroCategorias.Add(livroCategoria);

            }

            context.LivroCategoria.AddRange(ListaLivroCategorias);

            context.SaveChanges();

            return LocalRedirect("/Cadastro");
        }
        

        

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
} 