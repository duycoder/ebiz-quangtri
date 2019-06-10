using Elasticsearch.Net;
using Nest;
using System;
using System.Web.Configuration;
using Web.Common.Elastic;

namespace Web.Common
{
    public class ElasticSearch
    {
        public static string ELASTICSEARCHSERVER = WebConfigurationManager.AppSettings["ELASTICSEARCHSERVER"];
        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;
            var nodes = new Uri[]
                {
                    new Uri(ELASTICSEARCHSERVER),
                };
            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }

        #region Insert
        public static bool insertDocument(string searchID, string tbxname, string tbxOriginalVoiceActor, string tbxAnimatedDebut)
        {
            bool status;

            var myJson = new
            {
                name = tbxname,
                original_voice_actor = tbxOriginalVoiceActor,
                animated_debut = tbxAnimatedDebut
            };

            var response = ElasticSearch.EsClient().Index(myJson, i => i
                .Index("disney")
                .Type("character")
                .Id(searchID)
                .Refresh());

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }
        #endregion
        #region Update document based on ID

        ///Updating a Document can be done in trhee way
        ///1. Update by Partial Document
        ///2. Update by Index Query
        ///3. Update by Script
        ///Here we demonstrated Update by Partial Document and  Update by Index Query. User can choose any of these from below.
        ///Just comment one part and uncomment another.

        public static bool updateDocument(string searchID, string tbxname, string tbxOriginalVoiceActor, string tbxAnimatedDebut)
        {
            bool status;

            //Update by Partial Document
            var response = ElasticSearch.EsClient().Update<DocumentAttributes, UpdateDocumentAttributes>(searchID, d => d
                .Index("disney")
                .Type("character")
                .Doc(new UpdateDocumentAttributes
                {
                    name = tbxname,
                    original_voice_actor = tbxOriginalVoiceActor,
                    animated_debut = tbxAnimatedDebut
                }));
            //End of Update by Partial Document

            //Update by Index Query
            /*var myJson = new 
            {
                name = tbxname,
                original_voice_actor = tbxOriginalVoiceActor,
                animated_debut = tbxAnimatedDebut
            };

            var response = ConnectionToES.EsClient().Index(myJson, i => i
                .Index("disney")
                .Type("character")
                .Id(searchID)
                .Refresh());*/
            //End of Update by Index Query

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        #endregion Update document based on ID

        #region Delete Document based on ID

        public static bool deleteDocument(string searchID)
        {
            bool status;

            var response = ElasticSearch.EsClient().Delete<DocumentAttributes>(searchID, d => d
                .Index("disney")
                .Type("character"));

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        #endregion Delete document based on ID
    }
}