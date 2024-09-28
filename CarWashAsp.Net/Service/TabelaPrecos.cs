using CarWashAsp.Net.Models;

namespace CarWashAsp.Net.Service
{
    public class TabelaPrecos
    {
        // Dicionário que armazena preços baseados no plano e serviço
        private Dictionary<(Plano, Servico), decimal> precos = new Dictionary<(Plano, Servico), decimal>
        {
            { (Plano.Leve, Servico.Polimento), 1000M },
            { (Plano.Pesado, Servico.Polimento), 1500M },
            { (Plano.Leve, Servico.LavagemSimples), 500M },
            { (Plano.Pesado, Servico.LavagemSimples), 800M },
            { (Plano.Leve, Servico.LavagemCompleta), 1200M },
            { (Plano.Pesado, Servico.LavagemCompleta), 1800M }
        };

        // Método para obter o preço de acordo com o plano e o serviço
        public decimal ObterPreco(Plano plano, Servico servico)
        {
            return precos[(plano, servico)];
        }
    }
}

