﻿using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts
{
    public class SearchQueryParameters
    {
        public SearchQueryParameters(List<SearchQueryParameter> filters, QueryOperators? queryOperator = null, string customQuery = "")
        {
            Filters = filters;
            QueryOperator = queryOperator;
            CustomQuery = customQuery;
        }

        public List<SearchQueryParameter> Filters { get; set; }
        public QueryOperators? QueryOperator { get; set; }

        public string CustomQuery { get; set; }
    }
}
