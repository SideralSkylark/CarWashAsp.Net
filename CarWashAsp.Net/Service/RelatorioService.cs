using CarWashAsp.Net.Models;

namespace CarWashAsp.Net.Service
{
    public class RelatorioService
    {
        public List<Agendamento> GerarRelatorioDia(List<Agendamento> agendamentos, DateTime dia, out decimal total)
        {
            var relatorio = FiltrarPorDia(agendamentos, dia);
            total = CalcularTotal(relatorio);
            return relatorio;
        }

        public List<Agendamento> GerarRelatorioSemana(List<Agendamento> agendamentos, DateTime semana, out decimal total)
        {
            var (inicioDaSemana, fimDaSemana) = ObterIntervaloSemana(semana);
            var relatorio = FiltrarPorIntervalo(agendamentos, inicioDaSemana, fimDaSemana);
            total = CalcularTotal(relatorio);
            return relatorio;
        }

        public List<Agendamento> GerarRelatorioMes(List<Agendamento> agendamentos, DateTime mes, out decimal total)
        {
            var relatorio = FiltrarPorMes(agendamentos, mes);
            total = CalcularTotal(relatorio);
            return relatorio;
        }

        private List<Agendamento> FiltrarPorDia(List<Agendamento> agendamentos, DateTime dia)
        {
            return agendamentos
                .Where(a => a.DataAgendamento.Date == dia.Date)
                .ToList();
        }

        private (DateTime inicioDaSemana, DateTime fimDaSemana) ObterIntervaloSemana(DateTime semana)
        {
            var inicioDaSemana = semana.Date.AddDays(-(int)semana.DayOfWeek);
            var fimDaSemana = inicioDaSemana.AddDays(7);
            return (inicioDaSemana, fimDaSemana);
        }

        private List<Agendamento> FiltrarPorIntervalo(List<Agendamento> agendamentos, DateTime inicio, DateTime fim)
        {
            return agendamentos
                .Where(a => a.DataAgendamento.Date >= inicio && a.DataAgendamento.Date < fim)
                .ToList();
        }

        private List<Agendamento> FiltrarPorMes(List<Agendamento> agendamentos, DateTime mes)
        {
            return agendamentos
                .Where(a => a.DataAgendamento.Month == mes.Month && a.DataAgendamento.Year == mes.Year)
                .ToList();
        }

        private decimal CalcularTotal(List<Agendamento> agendamentos)
        {
            return agendamentos.Sum(a => a.Preco);
        }
    }
}
