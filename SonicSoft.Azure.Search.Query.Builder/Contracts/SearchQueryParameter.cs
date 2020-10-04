using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts
{
    public class SearchQueryParameter
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public DataType Type { get; set; }
        public bool IsNullCheck { get; set; }
        public ODataOperators ODataOperator { get; set; }
        public QueryConditions SubQueryParameterQueryCondition { get; set; }
        public List<SearchSubQueryParameter> SubQueryParameters { get; set; }
    }
}
