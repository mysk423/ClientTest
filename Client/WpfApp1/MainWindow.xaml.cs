using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> ServerList { get; set; } = new List<String>(){ "ip" };
        private String ServerIP = null;
        private Client client;

        public MainWindow()
        {
            InitializeComponent();

            ComboBoxList.ItemsSource = ServerList;
        }

        private void ComboBoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxList.SelectedItem != null)
            {
                ServerIP = ComboBoxList.SelectedItem.ToString();
            }
        }

        private void ServerConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(ServerIP != null)
            {
                client = new Client(ServerIP);
            }
            else
            {
                MessageBox.Show("Do not Select Server!!!!");
            }
        }

        public class Client
        {
            private WebSocket testWebSocket;
            public Client(String ServerIP)
            {
                StartConnect(ServerIP);
            }

            public async void StartConnect(String ServerIP)
            {
                ClientWebSocket ws = new ClientWebSocket();
                Uri uri = new Uri(ServerIP);

                await ws.ConnectAsync(uri, System.Threading.CancellationToken.None);
            }
        }
    }

}


