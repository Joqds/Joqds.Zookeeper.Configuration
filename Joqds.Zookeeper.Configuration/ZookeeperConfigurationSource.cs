using Microsoft.Extensions.Configuration;

namespace Joqds.Zookeeper.Configuration
{
    public class ZookeeperConfigurationSource : IConfigurationSource
    {
        private readonly string _connectionString;
        private readonly string _path;
        public ZookeeperConfigurationSource(string connectionString, string applicationName, string environment)
        {
            _connectionString = connectionString;
            _path = $"/{applicationName}/{environment}";
        }

        public ZookeeperConfigurationSource(string connectionString, string rootPath)
        {
            //todo: trailing slash safe
            _connectionString = connectionString;
            _path = $"/{rootPath}";
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(_connectionString, _path);
        }
    }
}