
namespace Joqds.Zookeeper.Configuration
{
    internal class JoqdsWatcher : Watcher, IDisposable
    {
        private readonly ZookeeperConfigurationProvider _configurationProvider;
        private bool _fetching;
        private bool _disposed;

        public JoqdsWatcher(ZookeeperConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override Task process(WatchedEvent @event)
        {
            if (!_fetching && !_disposed)
            {
                _fetching = true;
                Console.WriteLine($"{@event.get_Type()}, {@event.getPath()}");
                switch (@event.get_Type())
                {
                    case Event.EventType.NodeChildrenChanged:
                    case Event.EventType.NodeDeleted:
                    case Event.EventType.NodeCreated:
                    case Event.EventType.NodeDataChanged:
                        _configurationProvider.Load();
                        break;
                }

                _fetching = false;
            }
            else
            {
                Console.WriteLine("Fetch Canceled");
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}