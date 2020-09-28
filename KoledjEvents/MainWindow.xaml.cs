using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace KoledjEvents
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataTable dt = new DataTable();

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Otou6e5C5EJtcRG3ZwmWZo4vCOO2dsChxbtedcfu",
            BasePath = "https://koledj-8575b.firebaseio.com/"
        };

        IFirebaseClient client;


        private async void export()
        {
            dt.Rows.Clear();
            int i = 0;
            FirebaseResponse response = await client.GetAsync("Counter/node");
            Counter obj1 = response.ResultAs<Counter>();
            int cnt = Convert.ToInt32(obj1.cnt);
            while (true)
            {
                if (i == cnt)
                {
                    break;
                }
                i++;
                try
                {
                    FirebaseResponse resp2 = await client.GetAsync("Events/" + i);
                    Data obj2 = resp2.ResultAs<Data>();
                    DataRow row = dt.NewRow();
                    row["№"] = obj2.Id;
                    row["Name"] = obj2.Name;
                    row["Date"] = obj2.Date;
                    row["Description"] = obj2.Description;
                    dt.Rows.Add(row);
                   
                }
                catch
                {

                }

            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
            dt.Columns.Add("№");
            dt.Columns.Add("Name");
            dt.Columns.Add("Date");
            dt.Columns.Add("Description");
            EventsGrid.ItemsSource = dt.DefaultView;
            export();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            FirebaseResponse resp = await client.GetAsync("Counter/node");
            Counter get = resp.ResultAs<Counter>();


            var data = new Data
            {
                Id = (Convert.ToInt32(get.cnt) + 1).ToString(),
                Name = tbName.Text,
                Date = tbDate.SelectedDate.Value.ToShortDateString(),
                Description = tbDescription.Text,
            };


            SetResponse response = await client.SetAsync("Events/" + data.Id, data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Добавлено мероприятие № " + result.Id);

            var obj = new Counter
            {
                cnt = data.Id,
            };

            SetResponse response1 = await client.SetAsync("Counter/node", obj);
            export();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            export();
        }
    }
}
