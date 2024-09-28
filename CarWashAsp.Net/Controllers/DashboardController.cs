using CarWashAsp.Net.Data;
using CarWashAsp.Net.Models;
using CarWashAsp.Net.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace CarWashAsp.Net.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AgendamentoService _agendamentoService;

        public DashboardController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _agendamentoService = new AgendamentoService(new AgendamentoRepository(), new TabelaPrecos());
        }

        // GET: Dashboard/Geral
        public IActionResult Geral(string status = "")
        {
            var agendamentos = _agendamentoService.ObterAgendamentos();

            if (status == "Pendente")
            {
                agendamentos = _agendamentoService.ObterAgendamentosPendentes();
            } else if (status == "Concludo")
            {
                agendamentos = _agendamentoService.ObterAgendamentosConcluidos();
            }

            return View(agendamentos);
        }

        [HttpPost]
        public IActionResult AdicionarAgendamento(string nomeCliente, string placaCarro, DateTime dataAgendamento, Plano plano, Servico servico)
        {
            try
            {
                Console.WriteLine(dataAgendamento);
                _agendamentoService.AdicionarAgendamento(nomeCliente, placaCarro, dataAgendamento, plano, servico);
                return RedirectToAction("Geral");
            } catch (Exception ex)
            {
                _logger.LogError(ex, "erro ao adicionar o agendamento");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        public IActionResult EditarAgendamento(int id, string nomeCliente, string placaCarro, DateTime dataAgendamento, Plano plano, Servico servico)
        {
            try
            {
                _agendamentoService.EditarAgendamento(id, nomeCliente, placaCarro, dataAgendamento, plano, servico);
                return RedirectToAction("Geral");

            } catch(Exception ex)
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

        // Adicione este método no DashboardController
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

        // GET: DashBoard/Relatorio
        public IActionResult Relatorio(string periodo = "")
        {
            // Obtem todos os agendamentos já concluídos
            var agendamentosConfirmados = _agendamentoService.ObterAgendamentosConcluidos();

            // Data de hoje
            DateTime hoje = DateTime.Today;

            // Variável para armazenar os agendamentos filtrados
            IEnumerable<Agendamento> agendamentosFiltrados = agendamentosConfirmados;

            // Filtrar agendamentos de acordo com o período especificado
            switch (periodo)
            {
                case "Dia":
                    // Agendamentos do dia atual
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Date == hoje);
                    break;

                case "Semana":
                    // Agendamentos dos últimos 7 dias
                    var ultimaSemana = hoje.AddDays(-7);
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Date >= ultimaSemana && a.DataAgendamento.Date <= hoje);
                    break;

                case "Mes":
                    // Agendamentos do mês atual
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Year == hoje.Year && a.DataAgendamento.Month == hoje.Month);
                    break;

                case "Ano":
                    // Agendamentos do ano atual
                    agendamentosFiltrados = agendamentosConfirmados
                        .Where(a => a.DataAgendamento.Year == hoje.Year);
                    break;

                default:
                    // Se nenhum período for selecionado, retornará todos os agendamentos
                    break;
            }

            // Calcular o lucro total dos agendamentos filtrados
            decimal lucroTotal = agendamentosFiltrados.Sum(a => a.Preco);

            // Passar os dados filtrados e o lucro total para a view
            ViewBag.LucroTotal = lucroTotal;

            // Retornar a lista de agendamentos filtrados para a view
            return View(agendamentosFiltrados.ToList());
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
