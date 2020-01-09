using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Messanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Message> Messages = new List<Message>();
        public MainWindow()
        {
            InitializeComponent();
            GetMessage();
        }

        private void ClearName(object sender, RoutedEventArgs e)
        {
            nameUser.Text = String.Empty;
        }

        private void ClearMessage(object sender, RoutedEventArgs e)
        {
            message.Text = String.Empty;
        }

        private void ClearIpAddress(object sender, RoutedEventArgs e)
        {
            ipAddress.Text = String.Empty;
        }

        private void ClearPortUser(object sender, RoutedEventArgs e)
        {
            portUser.Text = String.Empty;
        }

        private  void SendMessageClick(object sender, RoutedEventArgs e)
        {
            string text = DateTime.Now.ToShortTimeString() + ":" + nameUser.Text + ":" + message.Text;
            SendMessage(text);
            //MessageBox.Show(text);
        }

        private async Task SendMessage(string text)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var localIp = IPAddress.Parse("10.1.4.87");
                var port = 3231;
                var endPoint = new IPEndPoint(localIp, port);

                await socket.ConnectAsync(endPoint);
                var buffer = Encoding.UTF8.GetBytes(text);
                socket.Send(buffer);
                socket.Shutdown(SocketShutdown.Both);
            }
        }

        private async Task GetMessage()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var context = new Context())
            {
                var set = context.Set<Message>();
                var localIp = IPAddress.Parse("10.1.4.87");
                var port = 3231;
                var endPoint = new IPEndPoint(localIp, port);

                socket.Bind(endPoint);
                socket.Listen(5);

                while (true)
                {
                    var incomnigSocket = await socket.AcceptAsync(); // блокирует пока не получит соединение
                    while (incomnigSocket.Available > 0)
                    {
                        var buffer = new byte[incomnigSocket.Available];
                        incomnigSocket.Receive(buffer);

                        string sendMessage = System.Text.Encoding.UTF8.GetString(buffer);
                        set.Add(new Message { Text = sendMessage });
                        await context.SaveChangesAsync();
                    }

                    bool isCheckMessage = false;
                    foreach (var message in context.Messages)
                    {
                        foreach (var existingMessage in Messages)
                        {
                            if (Guid.Equals(message.Id, existingMessage.Id))
                            {
                                isCheckMessage = true;
                                break;
                            }
                        }
                        if(!isCheckMessage)
                        {
                            string[] tokens = message.Text.Split(':');
                            
                            if (String.Equals(tokens[1], nameUser))
                            {
                                messang.Text += message.Text + '\n';
                            }
                            else
                            {
                                messang.Text += message.Text + '\n';
                            }
                            Messages.Add(message);
                        }
                    }

                    incomnigSocket.Close();
                }
            }
        }
    }
}
