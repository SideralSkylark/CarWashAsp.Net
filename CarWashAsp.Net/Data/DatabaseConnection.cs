using MySql.Data.MySqlClient;

namespace CarWashAsp.Net.Data
{
    public class DatabaseConnection
    {
        private string connectionString = "Server=localhost;Port=3306;Database=carwash;User Id=root;Password=;";


        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
