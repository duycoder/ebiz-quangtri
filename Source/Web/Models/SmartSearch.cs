using System.Collections.Generic;

namespace Web.Models
{
    public class SmartSearch
    {
        public string search { get; set; }
        public List<ElasticModel> ListSearchResult { get; set; }
    }
}