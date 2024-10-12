using CarWashAsp.Net.Models;
using MySql.Data.MySqlClient;

namespace CarWashAsp.Net.Data
{
    public class UsuarioRepository
    {
        private readonly DatabaseConnection _connection;

        public UsuarioRepository()
        {
            _connection = new DatabaseConnection();
        }

        public Usuario Login(string email, string senha)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Usuario WHERE Email = @Email AND Senha = @Senha";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return LerUsuario(reader);
                        }
                    }
                }
            }
            return null; // Retorna null se o login falhar
        }

        private static Usuario LerUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Email = reader.GetString("Email"),
                Senha = reader.GetString("Senha")
            };
        }
    }
}
