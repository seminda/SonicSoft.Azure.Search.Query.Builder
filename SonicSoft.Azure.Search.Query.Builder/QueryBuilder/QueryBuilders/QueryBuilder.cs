﻿using SonicSoft.Azure.Search.Query.Builder.Contracts;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    internal abstract class QueryBuilder : IQueryBuilder
    {
        public abstract bool IsMatch(SearchQueryParameter searchQueryParameter);

        public abstract string BuildQuery(SearchQueryParameter searchQueryParameter);


    }
}
