namespace CarWashAsp.Net.Models
{
    public enum TipoProduto
    {
        DETERGENTE,
        CERA,
        SPRAY
    }

    public class Produto
    {
        public int Id { get; set; }
        public TipoProduto TipoProduto { get; set; } // Mudado para o tipo enum
        public int Quantia { get; set; }
    }
}
