using CarWashAsp.Net.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CarWashAsp.Net.Data
{
    public class ProdutoRepository
    {
        private readonly DatabaseConnection _databaseConnection;

        public ProdutoRepository()
        {
            _databaseConnection = new DatabaseConnection();
        }

        public void AdicionarProduto(Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO produto (tipo_produto, quantia) VALUES (@tipo_produto, @quantia)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    ConfigurarParametrosProduto(cmd, produto);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Produto> ListarProdutos()
        {
            var produtos = new List<Produto>();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM produto";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var produto = LerProduto(reader);
                        produtos.Add(produto);
                    }
                }
            }
            return produtos;
        }

        public Produto ObterProdutoPorId(int id)
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM produto WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return LerProduto(reader);
                        }
                    }
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null; // Retorna nulo se o produto não for encontrado
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void AtualizarProduto(Produto produto)
        {
            if (produto == null || produto.Id == 0)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "UPDATE produto SET tipo_produto = @tipo_produto, quantia = @quantia WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    ConfigurarParametrosProduto(cmd, produto);
                    cmd.Parameters.AddWithValue("@id", produto.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoverProduto(Produto produto)
        {
            if (produto == null || produto.Id == 0)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM produto WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", produto.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static Produto LerProduto(MySqlDataReader reader)
        {
            return new Produto
            {
                Id = reader.GetInt32("id"),
                TipoProduto = (TipoProduto)Enum.Parse(typeof(TipoProduto), reader.GetString("tipo_produto")), // Converte a string para o enum
                Quantia = reader.GetInt32("quantia")
            };
        }

        private static void ConfigurarParametrosProduto(MySqlCommand cmd, Produto produto)
        {
            // Converte o enum para string ao configurar o parâmetro
            cmd.Parameters.AddWithValue("@tipo_produto", produto.TipoProduto.ToString());
            cmd.Parameters.AddWithValue("@quantia", produto.Quantia);
        }
    }
}