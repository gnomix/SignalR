using System.Threading;
using System.Threading.Tasks;

namespace SignalR
{
    public interface IConnection
    {
        Task<PersistentResponse> ReceiveAsync(CancellationToken timeoutToken);
        Task<PersistentResponse> ReceiveAsync(string messageId, CancellationToken timeoutToken);

        Task SendCommand(SignalCommand command);
        Task Send(object value);
    }
}