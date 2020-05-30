using System.Collections.Generic;
using System.Linq;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;
using SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder
{
    public class SearchQueryBuilder : ISearchQueryBuilder
    {
        private readonly List<IQueryBuilder> _queryBuilders;

        public SearchQueryBuilder(IPropertyMapper filterMapper, ISearchConfiguration searchConfiguration)
        {
            _queryBuilders = new List<IQueryBuilder>
            {
                new StandardQueryBuilder(filterMapper, searchConfiguration),
                new CollectionQueryBuilder(filterMapper, searchConfiguration)
            };
        }

        public string BuildQuery(QueryOperators? queryOperator, params SearchQueryParameters[] parameters)
        {
            var list = new List<string>();

            foreach (var parameter in parameters.Where(s => s?.Filters != null))
            {
                var query = BuildQuery(parameter);
                if (!string.IsNullOrWhiteSpace(query))
                {
                    list.Add(query);
                }
            }

            return BuildCustomQuery(queryOperator, list.ToArray());
        }

        public string BuildCustomQuery(QueryOperators? queryOperator, params string[] queries)
        {
            var value =
                $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return (queries.Length > 1 && !string.IsNullOrWhiteSpace(value)) ? $"({value})" : value;
        }

        private string BuildQuery(SearchQueryParameters parameters)
        {
            var queryList = new List<string>();

            foreach (var searchFilterInfo in parameters.Filters.Where(s => s != null))
            {
                var query = _queryBuilders.First(s => s.IsMatch(searchFilterInfo))
                    .BuildQuery(searchFilterInfo);

                if (string.IsNullOrWhiteSpace(query)) continue;

                queryList.Add(query);
            }

            if (!string.IsNullOrWhiteSpace(parameters.CustomQuery))
            {
                queryList.Add(parameters.CustomQuery);
            }

            return BuildCustomQuery(parameters.QueryOperator, queryList.ToArray());
        }
    }
}
