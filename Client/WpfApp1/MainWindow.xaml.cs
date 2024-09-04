using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
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
        public List<String> ServerList { get; set; } = new List<String>(){ "ws://localhost:8080" };
        private String ServerIP = null;
        private Client client;
        public static ClientWebSocket ws = new ClientWebSocket();
        private String appendmessage;
        
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
                StartConnectAsync(ServerIP);
            }

            public async Task StartConnectAsync(String ServerIP)
            {
                var connectTimeout = new CancellationTokenSource();
                connectTimeout.CancelAfter(2000);
                Uri uri = new Uri(ServerIP);

                await ws.ConnectAsync(uri, connectTimeout.Token);
                if (ws.State != System.Net.WebSockets.WebSocketState.Open)
                {

                    Console.WriteLine($"Failed to connect: {ServerIP}");

                    return;

                }
             

                if (ws.State != WebSocketState.Open)
                {
                    // Connect error
                }

                while(ws.State == WebSocketState.Open)
                {
                    string recvMsg = await Read(ws);
                    
                }
            }

        }

        private static async Task<string> Read(ClientWebSocket ws)
        {
            var buffer = new byte[1024];
            var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var message = MyMessage.Text;
            MessageSendAsync(ws, message);
            MyMessage.Clear();
        }

        private async Task MessageSendAsync(ClientWebSocket ws, String Message)
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(Message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void MyMessage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ChatBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}


