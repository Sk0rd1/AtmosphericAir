using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfAA
{
    public static class DB
    {
        private static string connectionString;
        private static NpgsqlConnection connection;

        public static void CreateConnection(string host = "localhost", int port = 1029, string username = "admin", string password = "1234", string database = "AtmosphericAir")
        {
            connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database};";
            //connectionString = $"Host=localhost;Port=1029;Username=admin;Password=1234;Database=AtmosphericAir;";
            connection = new NpgsqlConnection(connectionString);
        }
        public static NpgsqlConnection GetConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }
        public static bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch 
            { 
                return false; 
            }
        }
        public static void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

    }
}
