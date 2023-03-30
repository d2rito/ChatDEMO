using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace BlazorApp1.Server.Hubs

{
    public class ChatHub : Hub
    {
        private static Regex pattern = new Regex(@"^\/");
        private static Regex stockCommandPattern = new Regex(@"InvalidStock");
        private static Dictionary<string, string> Users = new Dictionary<string, string>();
        //private readonly RabbitMqMessageBus _messageBus;

        //public ChatHub(RabbitMqMessageBus messageBus)
        //{
        //    _messageBus = messageBus;
        //}

        //Este metodo es el que se ejecuta cuando se conecta el usuario al chat
        public override async Task OnConnectedAsync()
        {
            string username = Context.GetHttpContext().Request.Query["userName"];
            Users.Add(Context.ConnectionId, username);
            await AddMessageToChat(string.Empty, $"{username} connected.");
            await Clients.Client(Context.ConnectionId).SendAsync("CommandInput", string.Empty,  "StockBot: Welcome to chatDemo! You can use our /stock= command to get a specific stock value.\n Keep in mind the only values thar are valid are the symbol of the Stocks in the stooq API\nHave fun!");
            await base.OnConnectedAsync();
        }

        //Cuando se desconecta el usuario
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            await AddMessageToChat(string.Empty, $"{username} left.");
            //return base.OnDisconnectedAsync(exception);
        }

        public async Task AddMessageToChat(string username, string message)
        {
            //aca se puede enviar el mensaje a un client especificamente con Client. ....
            if (pattern.IsMatch(message))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CommandInput", string.Empty, "StockBot: Invalid command, please verify you're typing it right.");
            }
            else if (stockCommandPattern.IsMatch(message))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CommandInput", string.Empty, "StockBot: This stock does not exist or maybe the value isn't updated in the Stooq API");
            }
            else
            {
                await Clients.All.SendAsync("RecievedMessage", username, message);
            }
        }
    }
}
