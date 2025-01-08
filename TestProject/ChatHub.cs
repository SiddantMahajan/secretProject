using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject.Hubs
{
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public override Task OnConnected()
        {
            var user = Context.User.Identity.Name;
            _connections.Add(user, Context.ConnectionId);

            return Clients.AllExcept(Context.ConnectionId).SendAsync("UserConnected", user);
        }

        public Task OnDisconnected(bool stopCalled)
        {
            var user = Context.User.Identity.Name;
            _connections.Remove(user, Context.ConnectionId);

            return Clients.AllExcept(Context.ConnectionId).SendAsync("UserDisconnected", user);
        }


        public Task SendMessageToUser(string user, string message)
        {
            return Clients.Client(_connections.GetConnection(user)).SendAsync("ReceiveMessage", message);
        }

        public Task SendSignal(string signal, string user)
        {
            return Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
        }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        public void Add(T key, string connectionId)
        {
            if (!_connections.ContainsKey(key))
            {
                _connections[key] = new HashSet<string>();
            }

            _connections[key].Add(connectionId);
        }

        public void Remove(T key, string connectionId)
        {
            if (_connections.ContainsKey(key))
            {
                _connections[key].Remove(connectionId);

                if (_connections[key].Count == 0)
                {
                    _connections.Remove(key);
                }
            }
        }

        public string GetConnection(T key)
        {
            if (_connections.ContainsKey(key))
            {
                return _connections[key].FirstOrDefault();
            }

            return null;
        }
    }
}