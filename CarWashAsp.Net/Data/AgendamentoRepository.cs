using CarWashAsp.Net.Models;
using MySql.Data.MySqlClient;

namespace CarWashAsp.Net.Data
{
    public class AgendamentoRepository
    {
        private readonly DatabaseConnection _databaseConnection;

        public AgendamentoRepository()
        {
            _databaseConnection = new DatabaseConnection();
        }

        public void AdicionarAgendamento(Agendamento agendamento)
        {
            if (agendamento == null)
            {
                throw new ArgumentNullException(nameof(agendamento));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO Agendamento (nome_cliente, placa_carro, data_agendamento, status, plano_id, servico_id, preco) " +
                            "VALUES (@nome_cliente, @placa_carro, @data_agendamento, @status, @plano_id, @servico_id, @preco)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    ConfigurarParametrosAgendamento(cmd, agendamento);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Agendamento> ListarAgendamentos()
        {
            return ListarAgendamentosPorStatus(null);
        }

        public List<Agendamento> ListarAgendamentosConcluidos()
        {
            return ListarAgendamentosPorStatus("Concluido");
        }

        public List<Agendamento> ListarAgendamentosPendentes()
        {
            return ListarAgendamentosPorStatus("Pendente");
        }

        private List<Agendamento> ListarAgendamentosPorStatus(string status)
        {
            var agendamentos = new List<Agendamento>();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Agendamento" + (status != null ? " WHERE status = @status" : "");
                using (var cmd = new MySqlCommand(query, connection))
                {
                    if (status != null)
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var agendamento = LerAgendamento(reader);
                            agendamentos.Add(agendamento);
                        }
                    }
                }
            }
            return agendamentos;
        }

        public Agendamento ObterAgendamentoPorId(int id)
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Agendamento WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return LerAgendamento(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void AtualizarAgendamento(Agendamento agendamento)
        {
            if (agendamento == null || agendamento.Id == 0)
            {
                throw new ArgumentNullException(nameof(agendamento));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "UPDATE Agendamento SET nome_cliente = @nome_cliente, placa_carro = @placa_carro, " +
                            "data_agendamento = @data_agendamento, status = @status, plano_id = @plano_id, servico_id = @servico_id, preco = @preco " +
                            "WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    ConfigurarParametrosAgendamento(cmd, agendamento);
                    cmd.Parameters.AddWithValue("@id", agendamento.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Remover(Agendamento agendamento)
        {
            if (agendamento == null || agendamento.Id == 0)
            {
                throw new ArgumentNullException(nameof(agendamento));
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Agendamento WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", agendamento.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static Agendamento LerAgendamento(MySqlDataReader reader)
        {
            return new Agendamento
            {
                Id = reader.GetInt32("id"),
                NomeCliente = reader.GetString("nome_cliente"),
                PlacaCarro = reader.GetString("placa_carro"),
                DataAgendamento = reader.GetDateTime("data_agendamento"),
                Status = reader.GetString("status"),
                Plano = (Plano)Enum.Parse(typeof(Plano), reader.GetInt32("plano_id").ToString()),
                Servico = (Servico)Enum.Parse(typeof(Servico), reader.GetInt32("servico_id").ToString()),
                Preco = reader.GetDecimal("preco")
            };
        }

        private static void ConfigurarParametrosAgendamento(MySqlCommand cmd, Agendamento agendamento)
        {
            cmd.Parameters.AddWithValue("@nome_cliente", agendamento.NomeCliente);
            cmd.Parameters.AddWithValue("@placa_carro", agendamento.PlacaCarro);
            cmd.Parameters.AddWithValue("@data_agendamento", agendamento.DataAgendamento);
            cmd.Parameters.AddWithValue("@status", agendamento.Status);
            cmd.Parameters.AddWithValue("@plano_id", (int)agendamento.Plano);
            cmd.Parameters.AddWithValue("@servico_id", (int)agendamento.Servico);
            cmd.Parameters.AddWithValue("@preco", agendamento.Preco);
        }
    }
}
