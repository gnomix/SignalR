using System;
using SignalR.Hubs;

namespace SignalR
{
    public interface IConnectionManager
    {
        dynamic GetClients<T>() where T : IHub;
        IBroadcastingConnection GetConnection<T>() where T : PersistentConnection;
    }
}
