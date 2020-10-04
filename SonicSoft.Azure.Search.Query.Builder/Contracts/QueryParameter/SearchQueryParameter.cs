using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter
{
    public class SearchQueryParameter: SearchQueryParameterBase
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public ODataOperators ODataOperator { get; set; }
        public QueryConditions SubQueryParameterQueryCondition { get; set; }
        public List<SearchSubQueryParameter> SubQueryParameters { get; set; }
    }
}
