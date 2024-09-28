using CarWashAsp.Net.Data;
using CarWashAsp.Net.Models;

namespace CarWashAsp.Net.Service
{
    public class AgendamentoService
    {
        private readonly TabelaPrecos _tabelaPrecos;
        private readonly AgendamentoRepository _agendamentoRepository;

        public AgendamentoService(AgendamentoRepository agendamentoRepository, TabelaPrecos tabelaPrecos)
        {
            _agendamentoRepository = agendamentoRepository ?? throw new ArgumentNullException(nameof(agendamentoRepository));
            _tabelaPrecos = tabelaPrecos ?? throw new ArgumentNullException(nameof(tabelaPrecos));
        }

        public void AdicionarAgendamento(string nomeCliente, string placaCarro, DateTime dataAgendamento, Plano plano, Servico servico)
        {
            var preco = _tabelaPrecos.ObterPreco(plano, servico);

            var agendamento = new Agendamento
            {
                NomeCliente = nomeCliente,
                PlacaCarro = placaCarro,
                DataAgendamento = dataAgendamento,
                Status = "Pendente",
                Plano = plano,
                Servico = servico,
                Preco = preco
            };

            _agendamentoRepository.AdicionarAgendamento(agendamento);
        }

        public List<Agendamento> ObterAgendamentos()
        {
            return _agendamentoRepository.ListarAgendamentos();
        }

        public List<Agendamento> ObterAgendamentosConcluidos()
        {
            return _agendamentoRepository.ListarAgendamentosConcluidos();
        }

        public List<Agendamento> ObterAgendamentosPendentes()
        {
            return _agendamentoRepository.ListarAgendamentosPendentes();
        }

        public void MarcarConcluido(int idAgendamento)
        {
            var agendamento = _agendamentoRepository.ObterAgendamentoPorId(idAgendamento);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {idAgendamento} não encontrado.");
            }

            agendamento.Status = "Concluido";
            _agendamentoRepository.AtualizarAgendamento(agendamento);
        }

        public void EditarAgendamento(int idAgendamento, string novoNome, string novaPlaca, DateTime novaData, Plano novoPlano, Servico novoServico)
        {
            var agendamento = _agendamentoRepository.ObterAgendamentoPorId(idAgendamento);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {idAgendamento} não encontrado.");
            }

            agendamento.NomeCliente = novoNome;
            agendamento.PlacaCarro = novaPlaca;
            agendamento.DataAgendamento = novaData;
            agendamento.Plano = novoPlano;
            agendamento.Servico = novoServico;
            agendamento.Preco = _tabelaPrecos.ObterPreco(novoPlano, novoServico); // Recalcula o preço

            _agendamentoRepository.AtualizarAgendamento(agendamento);
        }

        public void RemoverAgendamento(int idAgendamento)
        {
            var agendamento = _agendamentoRepository.ObterAgendamentoPorId(idAgendamento);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {idAgendamento} não encontrado.");
            }

            _agendamentoRepository.Remover(agendamento);
            Console.WriteLine("Removido com sucesso");
        }
    }
}
