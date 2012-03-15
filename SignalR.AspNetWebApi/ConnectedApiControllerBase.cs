using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using SignalR.Hosting;
using SignalR.Hubs;
using SignalR.Infrastructure;

namespace SignalR.AspNetWebApi
{
    public abstract class ConnectedApiControllerBase : ApiController, IHub
    {
        private readonly IDependencyResolver _dependencyResolver;

        public ConnectedApiControllerBase(IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
            {
                throw new ArgumentNullException("dependencyResolver");
            }

            _dependencyResolver = dependencyResolver;
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var hub = ((IHub)this);
            
            var user = controllerContext.Request.GetUserPrincipal();
            // Response parameter is null here because outgoing broadcast messages will always go
            // via the SignalR intrinsics, and method return values via the Web API intrinsics.
            var hostContext = new HostContext(new WebApiRequest(controllerContext.Request), null, user);
            var connectionId = hostContext.Request.QueryString["connectionId"];
            hub.Context = new HubContext(hostContext, connectionId);
            var hubName = this.GetType().FullName;
            var connection = _dependencyResolver.Resolve<IConnectionManager>().GetConnection<HubDispatcher>();
            var state = new TrackingDictionary();
            var agent = new ClientAgent(connection, hubName);
            hub.Caller = new SignalAgent(connection, connectionId, hubName, state);
            hub.Agent = agent;
            hub.GroupManager = agent;
            
            base.Initialize(controllerContext);
        }

        public dynamic Caller
        {
            get { return ((IHub)this).Caller; }
        }

        public dynamic Clients
        {
            get { return ((IHub)this).Agent; }
        }

        public HubContext HubContext
        {
            get { return ((IHub)this).Context; }
        }

        public bool IsConnectedRequest
        {
            get { return !String.IsNullOrEmpty(HubContext.ConnectionId); }
        }

        public Task AddToGroup(string connectionId, string groupName)
        {
            return ((IHub)this).GroupManager.AddToGroup(connectionId, groupName);
        }

        public Task RemoveFromGroup(string connectionId, string groupName)
        {
            return ((IHub)this).GroupManager.RemoveFromGroup(connectionId, groupName);
        }

        IClientAgent IHub.Agent
        {
            get;
            set;
        }

        dynamic IHub.Caller
        {
            get;
            set;
        }

        HubContext IHub.Context
        {
            get;
            set;
        }

        IGroupManager IHub.GroupManager
        {
            get;
            set;
        }
    }
}
