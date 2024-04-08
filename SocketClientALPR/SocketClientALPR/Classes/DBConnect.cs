
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace SocketClientALPR.Classes
{
    internal class DBConnect
    {
        public DataTable Select(string selectSQL) // функция подключения к базе данных и обработка запросов
        {
            DataTable dataTable = new DataTable("dataBase");                // создаём таблицу в приложении

            SqlConnection sqlConnection = new SqlConnection("server=localhost;DataBase=lpdb;User Id=SA;Password=MyV€ryStr0ngP4ssW0rĐ");
            sqlConnection.Open();                                           // открываем базу данных
            SqlCommand sqlCommand = sqlConnection.CreateCommand();          // создаём команду
            sqlCommand.CommandText = selectSQL;                             // присваиваем команде текст
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // создаём обработчик
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
    }
}
