using System.Configuration;
using MySql.Data.MySqlClient;

namespace Projekat_B_isTovar
{
    public static class Database
    {
        public static MySqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            return new MySqlConnection(connStr);
        }
    }
}
