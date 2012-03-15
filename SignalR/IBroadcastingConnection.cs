using System;
using System.Threading.Tasks;

namespace SignalR
{
    public interface IBroadcastingConnection : IConnection
    {
        Task Broadcast(string message, object value);
        Task Broadcast(object value);
    }
}