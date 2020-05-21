using System;
using System.Collections.Generic;
using System.Text;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts
{
    public class QueryParameters
    {
        public QueryParameters(List<QueryParameter> filters, QueryOperators? queryOperator = null, string customQuery = "")
        {
            Filters = filters;
            QueryOperator = queryOperator;
            CustomQuery = customQuery;
        }

        public List<QueryParameter> Filters { get; set; }
        public QueryOperators? QueryOperator { get; set; }

        public string CustomQuery { get; set; }
    }
}
