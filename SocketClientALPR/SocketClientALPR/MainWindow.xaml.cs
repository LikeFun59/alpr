using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WebSocketSharp;
using System.Windows.Controls;
using SocketClientALPR.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Win32;

namespace SocketClientALPR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<Models> models = new List<Models>();
        }

        WebSocket ws;
        void ConnectServer()
        {
            try
            {
                using (ws = new WebSocket("ws://localhost:8765"))
                {
                    ws.OnMessage += (sender, e) =>
                    {
                        OnMessage(e);
                    };
                    ws.OnClose += (sender, e) =>
                    {
                        OnClose(e);
                    };
                    ws.OnOpen += (sender, e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() => { textBlock.Text = "Подключено"; }));
                    };
                }
                ws.Connect();
                this.listBox.Children.Add(new TextBox { Text = ws.ReadyState.ToString() } );
                if (ws.ReadyState.ToString() == "Open")
                {
                    this.Dispatcher.Invoke(new Action(() => { 
                        textBlock.Text = "Подключено";
                        ConnBtn.IsEnabled = false;
                        DiscBtn.IsEnabled = true;
                    }));
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() => { 
                        textBlock.Text = "Отлючено";
                        ConnBtn.IsEnabled = true;
                        DiscBtn.IsEnabled = false;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            ConnectServer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ws.Close();
        }

        private Task OnClose(CloseEventArgs arg)
        {
            Task tk = new Task(() =>
            {
                try
                {
                    ws.Connect();
                }
                catch
                {
                    return;
                }
            });
            if (arg.Code == 1005)
            {
                this.Dispatcher.Invoke(new Action(() => {
                    textBlock.Text = "Отлючено";
                    ConnBtn.IsEnabled = true;
                    DiscBtn.IsEnabled = false;
                }));
                return tk;
            }
            else
            {
                tk.Start();
                return tk;
            }
        }

        private Task OnMessage(MessageEventArgs args)
        {
            string content = args.Data;
            Task tk = new Task(() =>
            {
                try
                {
                    if (args.IsBinary)
                    {
                        this.Dispatcher.Invoke(new Action(() => {
                            listBox.Children.Add(new Image { Source = LoadImage(args.RawData), Width = 300 } );
                        }));
                        
                    }
                    else
                    {
                        this.Dispatcher.Invoke(new Action(() => {
                            this.listBox.Children.Add(new TextBox { Text = content });
                        }));
                    }
                }
                catch
                {
                    return;
                }
            });
            tk.Start();
            return tk;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void UploadInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridBD.ItemsSource = null;
                var sqlString = "";
                DBConnect DB = new DBConnect();
                var dt_user = new DataTable();
                if (StartDate.SelectedDate is null && EndDate.SelectedDate is null)
                {
                    sqlString = "SELECT * FROM [lpdb].[dbo].[main_gate_alpr_license_plates] ORDER BY [captured_at]";
                    dt_user = DB.Select(sqlString);
                }
                else
                {
                    if (StartDate.SelectedDate is not null && EndDate.SelectedDate is not null)
                    {
                        sqlString = $"SELECT * FROM [lpdb].[dbo].[main_gate_alpr_license_plates] where captured_at between '{StartDate.SelectedDate.Value.ToString("s")}' and '{EndDate.SelectedDate.Value.ToString("s")}' ORDER BY [captured_at]";
                        dt_user = DB.Select(sqlString);
                    }
                    else
                    {
                        MessageBox.Show("Выберите обе даты для отображения результата или очистите оба поля");
                    }
                }
                DataGridBD.ItemsSource = dt_user.DefaultView;
            }
            catch { }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
                {
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("Resources\\Инструкционная карта.docx")
                {
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch { }
        }

        private void UnloadInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stream myStream;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "Excel files (*.xls)|*.xls";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog().Value == true)
                {
                    DataGridBD.SelectAllCells();
                    DataGridBD.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                    ApplicationCommands.Copy.Execute(null, DataGridBD);
                    String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                    String result = (string)Clipboard.GetData(DataFormats.Text);
                    DataGridBD.UnselectAllCells();
                    StreamWriter file = new StreamWriter(saveFileDialog1.FileName);
                    file.WriteLine(result.Replace(',', ' '));
                    file.Close();
                }
            }
            catch { }
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLog.Text) && string.IsNullOrEmpty(txtPass.Text))
            {
                MessageBox.Show("Поля логин и пароль пустые - заполните");
            }
            else
            {
                var sqlString = "";
                DBConnect DB = new DBConnect();
                sqlString = $"INSERT INTO [lpdb].[dbo].[user_pass_alpr] VALUES ('{txtLog.Text}','{txtPass.Text}','')";
                DB.Select(sqlString);
                MessageBox.Show($"Пользователь {txtLog.Text} добавлен в БД");
                txtLog.Text = txtPass.Text =  "";
            }
        }

        private void AddAutoNumber_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNum.Text))
            {
                MessageBox.Show("Введите номер автомобиля, что бы добавить его в базу");
            }
            else
            {
                var sqlString = "";
                DBConnect DB = new DBConnect();
                sqlString = $"INSERT INTO [lpdb].[dbo].[automobile_table] VALUES ('{txtNum.Text}','{txtName.Text}','{txtCompany.Text}')";
                DB.Select(sqlString);
                MessageBox.Show($"Номер {txtNum.Text} добавлен в БД");
                txtNum.Text = txtName.Text = txtCompany.Text = "";
            }
        }
    }
}
