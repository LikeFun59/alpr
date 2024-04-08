using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SocketClientALPR.Classes;
using System.Data.SqlClient;
using System.Data;

namespace SocketClientALPR
{
    /// <summary>
    /// Логика взаимодействия для AuthForms.xaml
    /// </summary>
    public partial class AuthForms : Window
    {
        public AuthForms()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AuthMethod();
        }

        private void TextPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AuthMethod();
            }
        }

        private void AuthMethod()
        {
            try
            {
                var sqlString = "SELECT [password] FROM [lpdb].[dbo].[user_pass_alpr] WHERE [user] = '" + TextLogin.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS";
                DBConnect DB = new DBConnect();
                DataTable dt_user = DB.Select(sqlString); // получаем данные из таблицы

                if (dt_user.Rows.Count != 0)
                {
                    if (dt_user.Rows[0][0].ToString() == TextPassword.Password)
                    {
                        MainWindow main = new MainWindow();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Введен неверный логин или пароль");
                    }
                }
                else
                {
                    MessageBox.Show("Введен неверный логин или пароль");
                }
            }
            catch
            {

            }
        }
    }
}
