using Elasticsearch.Net;
using Nest;
using System;
using System.Web.Configuration;

namespace Web.Common.Elastic
{
    public class ConnectionToES
    {
        private static string ElasticServer = WebConfigurationManager.AppSettings["ElasticServer"];
        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;
            var nodes = new Uri[]
                {
                    new Uri(ElasticServer),
                };
            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }
    }
}