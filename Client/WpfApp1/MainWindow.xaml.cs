using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> ServerList { get; set; } = new List<String>(){ "ws://192.168.5.103:8080" };
        private String ServerIP = null;
        private Client client;
        public static ClientWebSocket ws;
        private String appendmessage;
        private bool selectServer = false;
        private String UserIDText = null;
        
        public MainWindow()
        {
            InitializeComponent();
            
            ComboBoxList.ItemsSource = ServerList;
            
        }
        private void OnMessageReceived(string message)
        {
            // Dispatcher를 사용하여 UI 업데이트
            Dispatcher.Invoke(() =>
            {
                ChatBox.AppendText(message + Environment.NewLine);
            });
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
            if (!selectServer)
            {
                if (ServerIP != null)
                {
                    client = new Client(ServerIP);
                    client.MessageReceived += OnMessageReceived;
                    selectServer = true;
                    MessageBox.Show("서버 선택 완료");
                }
            }
            else
            {
                MessageBox.Show("서버 선택 불가");
            }
            
        }
        struct DataPacket {
            public string Name { get; set; }
            public string Message { get; set; }
        }
            
        public class Client
        {
            private ClientWebSocket _webSocket;
            // 메시지를 전달하기 위한 이벤트 정의
            public event Action<string> MessageReceived;

            public Client(string serverIP)
            {
                ws = new ClientWebSocket();
                StartConnectAsync(serverIP);
            }

            public async Task StartConnectAsync(string serverIP)
            {
                
                try
                {
                    var connectTimeout = new CancellationTokenSource();
                    connectTimeout.CancelAfter(2000);
                    Uri uri = new Uri(serverIP);

                    await ws.ConnectAsync(uri, connectTimeout.Token);
                    if (ws.State != WebSocketState.Open)
                    {
                        Console.WriteLine($"Failed to connect: {serverIP}");
                        return;
                    }

                    Console.WriteLine($"Connected to {serverIP}");

                    // 메시지 리시브
                    await ReceiveMessagesAsync(ws);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            private async Task ReceiveMessagesAsync(ClientWebSocket webSocket)
            {
                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string recvMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // 이벤트를 통해 메시지를 전달
                    MessageReceived?.Invoke(recvMsg);

                    Console.WriteLine($"Received: {recvMsg}");
                }
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var message = MyMessage.Text;
            var name = UserID.Text;
            await MessageSendAsync(ws, message, name);
            MyMessage.Clear();
        }

        private async Task MessageSendAsync(ClientWebSocket ws, String Message, String Name)
        {
            var packet = new DataPacket { Name = Name, Message = Message };

            var jsonString = JsonSerializer.Serialize(packet);

            var buffer = Encoding.UTF8.GetBytes(jsonString);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            //await ws.SendAsync(Encoding.UTF8.GetBytes(Message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void MyMessage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void ChatBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MyMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

    }

}


