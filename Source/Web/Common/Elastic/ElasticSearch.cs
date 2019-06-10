using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using CommonHelper;
using DocumentFormat.OpenXml.Wordprocessing;
using Web.Models;
namespace Web.Common.Elastic
{
    public class ElasticSearch
    {
        //private static string ElasticIndex = "doji";
        //private static string ElasticModel = "elasticmodel";
        private static string ElasticIndex = "hinet";
        private static string ElasticModel = "detai";
        private static string EnableElasticServer = WebConfigurationManager.AppSettings["EnableElasticServer"];

        #region update List user

        public static bool updateListUser(string ID, List<long> LstUserId, string Type)
        {
            if (EnableElasticServer.ToIntOrZero() == 1)
            {
                try
                {
                    var result = ConnectionToES.EsClient().Get<ElasticModel>(Type + "_" + ID.ToString(),
                        x => x.Index(ElasticIndex).Type(ElasticModel));
                    ElasticModel tmpModel = result.Source;
                    if (LstUserId != null && LstUserId.Count > 0)
                    {
                        tmpModel.ListUser.AddRange(LstUserId);
                        var response = ConnectionToES.EsClient().Update(
                            DocumentPath<ElasticModel>.Id(Type + "_" + ID.ToString()),
                            u => u.Index(ElasticIndex).Type(ElasticModel).Doc(tmpModel)
                        );
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion
        #region test for bo cong an
        //public static bool insertDocumentBCA(ElasticBCAModel obj, string ID, string Type)
        //{
        //    try
        //    {
        //        var response = ConnectionToES.EsClient().Index(obj, i => i
        //            .Index(ElasticIndex)
        //            .Type(ElasticModel)
        //            .Id(Type + "_" + ID)
        //            .Refresh());
        //        return response.IsValid;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //public static ISearchResponse<ElasticBCAModel> SmartSearchFileContent(string search)
        //{
        //    try
        //    {                
        //        var response = ConnectionToES.EsClient().Search<ElasticBCAModel>(s => s.From(0).Size(5).Query(
        //            q => q.MatchPhrase(m => m.Field(a => a.file.Content).Query(search))
        //        ).Highlight(h => h.
        //                PreTags("<tag1>").PostTags("</tag1>").Encoder("html").
        //                Fields(f => f.Field(e => e.file.Content))
        //            )

        //        );               
        //        return response;
        //    }
        //    catch
        //    {
        //        return new SearchResponse<ElasticBCAModel>();
        //    }
        //}
        #endregion
        #region Insert document with on ID
        public static bool insertDocument(ElasticModel obj, string ID, string Type)
        {
            if (EnableElasticServer.ToIntOrZero() == 1)
            {
                try
                {
                    var response = ConnectionToES.EsClient().Index(obj, i => i
                    .Index(ElasticIndex)
                    .Type(ElasticModel)
                    .Id(Type + "_" + ID)
                    .Refresh());
                    return response.IsValid;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion Insert document with on ID
        #region Delete Document based on ID
        public static bool deleteDocument(string searchID, string Type)
        {
            if (EnableElasticServer.ToIntOrZero() == 1)
            {
                try
                {
                    var response = ConnectionToES.EsClient().Delete<ElasticModel>(Type + "_" + searchID, d => d
                     .Index(ElasticIndex)
                     .Type(ElasticModel));
                    return response.IsValid;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion Delete document based on ID
        #region Update document based on ID
        public static bool updateDocument(ElasticModel obj, string ID, string Type)
        {
            if (EnableElasticServer.ToIntOrZero() == 1)
            {
                try
                {
                    var response = ConnectionToES.EsClient().Update(DocumentPath<ElasticModel>.Id(Type + "_" + ID.ToString()),
                        u => u.Index(ElasticIndex)
                            .Type(ElasticModel)
                            .Doc(obj)
                    );
                    return response.IsValid;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion Update document based on ID
        #region Get document info based on the ID
        public static List<ElasticModel> SmartSearch(string search, long UserId)
        {
            try
            {
               // var result = ConnectionToES.EsClient().SearchAsync<ElasticModel>(
               //s => s.From(0).Take(10).Query(
               //    q => (q.QueryString(qs => (qs.DefaultField("SoHieuNoSign").Query(search)))
               //    || q.QueryString(qs => (qs.DefaultField("NoiDungNoSign").Query(search)))
               //    || q.QueryString(qs => (qs.DefaultField("TrichYeuNoSign").Query(search)))
               //    || q.QueryString(qs => (qs.DefaultField("NguoiKyNoSign").Query(search))))
               //    &&
               //        q.Match(x => x.Field(f => f.ListUser).Query(UserId.ToString()))
               //    )
               //).Result.Documents.ToList();
                var result1 = ConnectionToES.EsClient().SearchAsync<ElasticModel>(
               s => s.From(0).Take(10).Query(
                   q => q
        .MultiMatch(mm => mm
            .Fields(f => f
                .Field(ff => ff.SoHieuNoSign)
                .Field(ff => ff.NoiDungNoSign)
                .Field(ff => ff.NguoiKyNoSign)
                .Field(ff => ff.TrichYeuNoSign)
            )
            //.Type(TextQueryType.PhrasePrefix)
            .Query(search)
        ) && q.Match(x => x.Field(f => f.ListUser).Query(UserId.ToString()))
                   )
               ).Result.Documents.ToList();
                return result1;
            }
            catch
            {
                return new List<ElasticModel>();
            }
        }
        #endregion Get document info based on the ID

    }
}