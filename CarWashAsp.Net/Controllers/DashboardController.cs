using CarWashAsp.Net.Data;
using CarWashAsp.Net.Models;
using CarWashAsp.Net.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarWashAsp.Net.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AgendamentoService _agendamentoService;
        private readonly ProdutoService _produtoService; // Adicione esta linha

        public DashboardController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _agendamentoService = new AgendamentoService(new AgendamentoRepository(), new TabelaPrecos());
            _produtoService = new ProdutoService(new ProdutoRepository()); // Inicialize o serviço de produtos
        }

        // GET: Dashboard/Geral
        public IActionResult Geral(string status = "")
        {
            var agendamentos = _agendamentoService.ObterAgendamentos();

            if (status == "Pendente")
            {
                agendamentos = _agendamentoService.ObterAgendamentosPendentes();
            }
            else if (status == "Concluido") // Corrigido "Concludo" para "Concluido"
            {
                agendamentos = _agendamentoService.ObterAgendamentosConcluidos();
            }

            return View(agendamentos);
        }

        [HttpGet]
        public IActionResult AbrirAdicionarAgendamento()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdicionarAgendamento(Agendamento agendamento)
        {
            try
            {
                Console.WriteLine(agendamento.DataAgendamento);
                _agendamentoService.AdicionarAgendamento(agendamento.NomeCliente, agendamento.PlacaCarro, agendamento.DataAgendamento, agendamento.Plano, agendamento.Servico);
                return RedirectToAction("Geral");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o agendamento");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet]
        public IActionResult AbrirEditarAgendamento(int idAgendamento)
        {
            var agendamentos = _agendamentoService.ObterAgendamentos();
            Agendamento alvo = agendamentos.FirstOrDefault(a => a.Id == idAgendamento);

            if (alvo == null)
            {
                return NotFound("Agendamento não encontrado.");
            }

            return View(alvo);
        }

        [HttpPost]
        public IActionResult EditarAgendamento(Agendamento agendamento)
        {
            try
            {
                _agendamentoService.EditarAgendamento(agendamento.Id, agendamento.NomeCliente, agendamento.PlacaCarro, agendamento.DataAgendamento, agendamento.Plano, agendamento.Servico);
                return RedirectToAction("Geral");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "erro ao editar agendamento com plano: " + plano + ", e servico: " + servico + ", id: " + id + ", nomeCliente: " + nomeCliente + ", placa: " + placaCarro + ", data: " + dataAgendamento);
                return StatusCode(500, "Erro interno de servidor");
            }
        }

        public IActionResult RemoverAgendamento(int id)
        {
            _agendamentoService.RemoverAgendamento(id);
            return RedirectToAction("Geral");
        }

        public IActionResult MarcarConcluido(int id)
        {
            _agendamentoService.MarcarConcluido(id);
            return RedirectToAction("Geral");
        }

        // Métodos para Produtos

        // GET: Dashboard/Produtos
        public IActionResult Produtos()
        {
            var produtos = _produtoService.ObterProdutos();
            return View(produtos);
        }

        [HttpPost]
        public IActionResult AdicionarProduto(string tipoProduto, int quantidade)
        {
            try
            {
                if (Enum.TryParse<TipoProduto>(tipoProduto, out var tipo))
                {
                    var produto = new Produto
                    {
                        TipoProduto = tipo,
                        Quantia = quantidade
                    };
                    _produtoService.AdicionarProduto(produto);

                    // Retorna o produto adicionado em JSON
                    return Json(new { id = produto.Id, tipoProduto = produto.TipoProduto.ToString(), quantidade = produto.Quantia });
                }
                else
                {
                    return BadRequest("Tipo de produto inválido.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o produto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
        [HttpPost]
        public IActionResult EditarProduto(int id, string tipoProduto, int quantidade)
        {
            try
            {
                if (Enum.TryParse<TipoProduto>(tipoProduto, out var tipo))
                {
                    var produto = new Produto
                    {
                        Id = id,
                        TipoProduto = tipo,
                        Quantia = quantidade
                    };
                    _produtoService.EditarProduto(produto);

                    // Retorna sucesso em JSON
                    return Json(new { success = true });
                }
                else
                {
                    return BadRequest("Tipo de produto inválido.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao editar produto. ID: {id}, Tipo: {tipoProduto}, Quantidade: {quantidade}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        [HttpPost]
        public IActionResult RemoverProduto(int id)
        {
            try 
            {
                _produtoService.RemoverProduto(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao remover o produto. ID: {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
        [HttpGet]
        public IActionResult ListarProdutos()
        {
            var produtos = _produtoService.ObterProdutos();
            var produtosJson = produtos.Select(p => new { id = p.Id, tipoProduto = p.TipoProduto.ToString(), quantidade = p.Quantia });
            return Json(produtosJson);
        }

        [HttpGet]
        public JsonResult ObterPreco(string tipoServico, Plano plano)
        {
            if (Enum.TryParse<Servico>(tipoServico, out var servico))
            {
                var tabelaPrecos = new TabelaPrecos();
                decimal preco = tabelaPrecos.ObterPreco(plano, servico);
                return Json(new { preco });
            }
            return Json(new { preco = 0 }); // Retorna 0 ou outra lógica de erro
        }

        // GET: Dashboard/Relatorio
        public IActionResult Relatorio(string periodo = "")
        {
            var agendamentosConfirmados = _agendamentoService.ObterAgendamentosConcluidos();
            DateTime hoje = DateTime.Today;
            IEnumerable<Agendamento> agendamentosFiltrados = agendamentosConfirmados;

            switch (periodo)
            {
                case "Dia":
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Date == hoje);
                    break;
                case "Semana":
                    var ultimaSemana = hoje.AddDays(-7);
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Date >= ultimaSemana && a.DataAgendamento.Date <= hoje);
                    break;
                case "Mes":
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Year == hoje.Year && a.DataAgendamento.Month == hoje.Month);
                    break;
                case "Ano":
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Year == hoje.Year);
                    break;
                default:
                    break;
            }

            decimal lucroTotal = agendamentosFiltrados.Sum(a => a.Preco);
            ViewBag.LucroTotal = lucroTotal;

            return View(agendamentosFiltrados.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
