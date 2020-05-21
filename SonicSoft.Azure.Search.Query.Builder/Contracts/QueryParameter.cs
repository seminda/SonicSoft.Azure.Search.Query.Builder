using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts
{
    public class QueryParameter
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsNullCheck { get; set; }
        public LogicalOperators LogicalOperator { get; set; }

        public QueryOperators SubQueryParameterQueryOperators { get; set; }
        public List<SubQueryParameter> SubQueryParameters { get; set; }
    }
}
