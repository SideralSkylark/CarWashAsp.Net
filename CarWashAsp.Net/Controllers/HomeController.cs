using CarWashAsp.Net.Models;
using CarWashAsp.Net.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarWashAsp.Net.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsuarioService _usuarioService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _usuarioService = new UsuarioService();
        }

        // GET: Dashboard
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(Usuario usuario)
        {
            
            var usuarioLogado = _usuarioService.Login(usuario.Email, usuario.Senha);

            if (usuarioLogado == null)
            {
                // Login falhou, redireciona de volta à página de login
                ViewBag.ErrorMessage = "Email ou senha inválidos.";
                return View("Index");
            } else
            {
                return RedirectToAction("Geral", "Dashboard");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
