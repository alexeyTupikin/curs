using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace curs
{
    public static class SqlServer
    {
        private static SqlConnection SqlConnection { get; set; }
        public static void CreateConnecion(string computer, string database) //создает подключение
        {
            SqlConnection = new SqlConnection(@"Data Source=" + computer + ";;Initial Catalog=" + database + ";"
                    + "Integrated Security=True;Connect Timeout=15;Encrypt=False;"
                    +
                    "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlConnection.Open();
        }
        public static SqlCommand CreateSqlCommand(string command) //функцция создает новую переменную класса SqlCommand с текстом, который передается в нее
        {
            return new SqlCommand
            {
                Connection = SqlConnection,
                CommandText = command
            };
        }
    }
}
