using Microsoft.AspNetCore.SignalR;
using Server.Models;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        static List<ClientUser> clientUsers = new();
        static List<Message> messages = new();
        static List<PersonelMessages> personalMessages = new();
        public async Task GetName(string name)
        {
            ClientUser clientUser = new ClientUser
            {
                ConnectionId = Context.ConnectionId,
                Name = name,
                LoginDate = DateTime.Now,
            };
            clientUsers.Add(clientUser);
            await Clients.Others.SendAsync("clientJoined", name);
            await Clients.All.SendAsync("getClients", clientUsers);
            await Clients.Caller.SendAsync("getClientName", name);
        }

        public async Task SendMessage(string messageContent)
        {
            ClientUser user = clientUsers.FirstOrDefault(predicate: x => x.ConnectionId == Context.ConnectionId);
            Message message = new Message
            {
                MessageContent = messageContent,
                Name = user.Name,
                Date = DateTime.Now.ToShortTimeString()
            };
            messages.Add(message);
            await Clients.All.SendAsync("getMessages", messages.OrderByDescending(x => x.Date));
        }

        public async Task GetMessgaes()
        {
            await Clients.Caller.SendAsync("getMessages", messages);
        }

        public async Task SendPersonalMessage(string message, string name)
        {
            ClientUser senderUser = clientUsers.FirstOrDefault(predicate: x => x.ConnectionId == Context.ConnectionId);
            ClientUser recieverUser = clientUsers.FirstOrDefault(predicate: x => x.Name == name);
            personalMessages.Add(new PersonelMessages
            {
                SenderId = senderUser.ConnectionId,
                SenderName = senderUser.Name,
                RecieverId = recieverUser.ConnectionId,
                RecieverName = recieverUser.Name,
                Message = message,
                Date = DateTime.Now.ToShortTimeString()

            });
            List<PersonelMessages> personelMessagesToSend = personalMessages.Where(x => x.RecieverId == recieverUser.ConnectionId).ToList();
            await Clients.Client(recieverUser.ConnectionId).SendAsync("getPersonalMessages", personelMessagesToSend);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientUser user = clientUsers.FirstOrDefault(predicate: x => x.ConnectionId == Context.ConnectionId);
            if (user is not null)
            {
                await Clients.Others.SendAsync("userLeaved", user.Name);
                clientUsers.Remove(user);
            }
            await Clients.All.SendAsync("getClients", clientUsers);
        }
    }
}
