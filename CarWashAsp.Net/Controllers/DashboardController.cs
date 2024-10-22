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
                _logger.LogError(ex, "erro ao editar agendamento com plano: " + agendamento.Plano + ", e servico: " + agendamento.Servico + ", id: " + agendamento.Id + ", nomeCliente: " + agendamento.NomeCliente + ", placa: " + agendamento.PlacaCarro + ", data: " + agendamento.DataAgendamento);
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
            var agendamentos = _agendamentoService.ObterAgendamentos();
            Agendamento alvo = agendamentos.FirstOrDefault(a => a.Id == id);
            var pordutosGastos = calcularProdutoGasto(alvo.Plano, alvo.Servico);
            
            deduzirProdutos(pordutosGastos);

            _agendamentoService.MarcarConcluido(id);
            return RedirectToAction("Geral");
        }

        private void deduzirProdutos(List<Produto> produtos)
        {
            foreach (var produto in produtos)
            {
                // Negativar a quantia para deduzir do estoque
                produto.Quantia = -produto.Quantia;

                // Chamar o método para atualizar a quantidade no banco de dados
                _produtoService.AtualizarQuantia(produto);
            }
        }

        private List<Produto> calcularProdutoGasto(Plano plano, Servico servico)
        {
            List<Produto> produtosGastos = new List<Produto>();

            if (plano == Plano.Leve && servico == Servico.Polimento)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 2;
                spray.Quantia = 2;
                detergente.Quantia = 0;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            } else if (plano == Plano.Leve && servico == Servico.LavagemSimples)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 0;
                spray.Quantia = 0;
                detergente.Quantia = 3;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            } else if (plano == Plano.Leve && servico == Servico.LavagemCompleta)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 2;
                spray.Quantia = 2;
                detergente.Quantia = 3;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            } else if (plano == Plano.Pesado && servico == Servico.Polimento)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 5;
                spray.Quantia = 5;
                detergente.Quantia = 0;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            }
            else if (plano == Plano.Pesado && servico == Servico.LavagemSimples)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 0;
                spray.Quantia = 0;
                detergente.Quantia = 6;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            }
            else if (plano == Plano.Pesado && servico == Servico.LavagemCompleta)
            {
                Produto cera = new Produto();
                Produto spray = new Produto();
                Produto detergente = new Produto();

                cera.TipoProduto = TipoProduto.CERA;
                spray.TipoProduto = TipoProduto.SPRAY;
                detergente.TipoProduto = TipoProduto.DETERGENTE;

                cera.Quantia = 5;
                spray.Quantia = 5;
                detergente.Quantia = 6;

                produtosGastos.Add(cera);
                produtosGastos.Add((spray));
                produtosGastos.Add((detergente));

                return produtosGastos;

            }
            return produtosGastos;
        }

        // Métodos para Produtos

        // GET: Dashboard/Produtos
        public IActionResult Produtos()
        {
            var produtos = _produtoService.ObterProdutos();
            return View(produtos);
        }

        public IActionResult AbrirAdicionarProduto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdicionarProduto(Produto produto)
        {
            try
            {
                _produtoService.AtualizarQuantia(produto);
                return RedirectToAction("Produtos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o produto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        public IActionResult AbrirEditarProduto(int id)
        {
            var produto = _produtoService.buscarPorId(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            return View(produto);
        }

        [HttpPost]
        public IActionResult EditarProduto(Produto produto)
        {
            try
            {
                _produtoService.EditarProduto(produto);
                return RedirectToAction("Produtos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o produto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        public IActionResult AbrirRemoverProduto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RemoverProduto(Produto produto)
        {
            try
            {
                int novaQuantia = 0 - produto.Quantia;
                produto.Quantia = novaQuantia;
                _produtoService.AtualizarQuantia(produto);
                return RedirectToAction("Produtos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o produto");
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
