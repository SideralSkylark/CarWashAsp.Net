using CarWashAsp.Net.Data;
using CarWashAsp.Net.Models;

namespace CarWashAsp.Net.Service
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _produtoRepository;

        public ProdutoService(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        }

        public void AdicionarProduto(Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            _produtoRepository.AdicionarProduto(produto);
        }

        public void AtualizarQuantia(Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            _produtoRepository.AtualizarQuantidadeProduto(produto);
        }

        public Produto buscarPorId(int id)
        {
            return _produtoRepository.ObterProdutoPorId(id);
        }

        public List<Produto> ObterProdutos()
        {
            return _produtoRepository.ListarProdutos();
        }

        public void EditarProduto(Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto));
            }
            Console.WriteLine(produto.ToString());
            _produtoRepository.AtualizarProduto(produto);
        }

        public void RemoverProduto(int idProduto)
        {
            var produto = _produtoRepository.ObterProdutoPorId(idProduto);
            if (produto == null)
            {
                throw new ArgumentException($"Produto com ID {idProduto} não encontrado.");
            }

            _produtoRepository.RemoverProduto(produto);
            Console.WriteLine("Produto removido com sucesso");
        }
    }
}
