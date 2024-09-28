using System.Numerics;

namespace CarWashAsp.Net.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string PlacaCarro { get; set; }
        public DateTime DataAgendamento { get; set; }
        public string Status { get; set; } // "Pendente", "Concluído"
        public Plano Plano { get; set; }   // Leve ou Pesado
        public Servico Servico { get; set; }  // Tipo de serviço
        public decimal Preco { get; set; } // Preço em Meticais
    }
}
