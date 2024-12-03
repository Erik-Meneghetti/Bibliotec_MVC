using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        // Criando um obj da classe Contex:
        Context context = new Context();

        //
        public IActionResult Index()
        {
            //Pegar  as informações da session que sao necessarias para que aparece os detalhes do meu usuario

            int id = int.Parse(HttpContext.Session.GetString("UsuarioID")!);
            ViewBag.admin = HttpContext.Session.GetString("Admin")!;

            // Busquei o usuario que esta logado (ERIK)
            Usuario usuarioEncontrado = context.Usuario.FirstOrDefault(usuario => usuario. UsuarioID == id)!; 
            
            //se nao for encontrado ninguem
            if(usuarioEncontrado == null){
                return NotFound();
            }

            //Procurar o curso que meu usuario esta cadastrado
            Curso cursoEncontrado = context.Curso.FirstOrDefault(curso => curso.CursoID == usuarioEncontrado.CursoID) !;

            //Verificar se o usuario possui ou nao o curso
            if(cursoEncontrado == null){

                //Preciso que voce mande essa mensagem para view:
                ViewBag.Curso = "O usuario nao possui curso cadastrado";
            }else{
                //Preciso que voce mande para nome do curso para View:
                ViewBag.Curso = cursoEncontrado.Nome;
            }

            ViewBag.nome = usuarioEncontrado.Nome;
             ViewBag.nome = usuarioEncontrado.Email;
              ViewBag.nome = usuarioEncontrado.Contato;
              ViewBag.DtNasc = usuarioEncontrado.DtNascimento.ToString("dd/MM/yyyy");


            return View();
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}