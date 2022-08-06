using Microsoft.Extensions.Configuration;

namespace Joqds.Zookeeper.Configuration
{
    public class ZookeeperConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connectionString;
        private readonly string _path;
        private JoqdsWatcher? _watcher;
        private ZooKeeper? _zookeeper;

        public ZookeeperConfigurationProvider(string connectionString, string path)
        {
            _connectionString = connectionString;
            _path = path;
        }

        public override void Load()
        {
            Data.Clear();
            StartLoadData(_path).Wait();

            foreach (var item in Data)
            {
                Console.WriteLine($"{item.Key}, {item.Value}");
            }
        }

        private async Task StartLoadData(string path)
        {
            _watcher?.Dispose();
            if (_zookeeper != null) await _zookeeper.closeAsync();

            _watcher = new JoqdsWatcher(this);
            _zookeeper = new ZooKeeper(_connectionString, 10000, _watcher, true);
            await LoadData(path);
        }
        private async Task LoadData(string path)
        {
            if(_zookeeper==null) return;
            var childrenResult = await _zookeeper.getChildrenAsync(path, true);
            if (childrenResult.Children.Count == 0)
            {
                var result = await _zookeeper.getDataAsync(path, true);
                var str = System.Text.Encoding.UTF8.GetString(result.Data);
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                var key = GetKey(path);
                Data[key] = str;
            }
            else
            {
                foreach (var currentPath in childrenResult.Children.Select(child => $"{path}/{child}"))
                {
                    await LoadData(currentPath);
                }
            }

        }
        private string GetKey(string path)
        {
            path = path.Replace(_path, "");
            path = path.Trim('/');
            path = path.Replace('/', ':');
            return path;
        }
    }
}