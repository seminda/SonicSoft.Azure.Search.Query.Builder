using System.Collections.Generic;
using System.Linq;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder
{
    public class SearchQueryBuilder:ISearchQueryBuilder
    {
        private IPropertyMapper _filterMapper;
        private const string Delimiter = "|";

        public SearchQueryBuilder(IPropertyMapper filterMapper)
        {
            _filterMapper = filterMapper;
        }
        
        public string BuildQuery(QueryOperators? queryOperator, params QueryParameters[] parameters)
        {
            return BuildCustomQuery(queryOperator, parameters.Where(s => s != null).Select(BuildQuery).ToArray());
        }
        
        private string BuildQuery(QueryParameters parameters)
        {
            var queryList = new List<string>();

            foreach (var searchFilterInfo in parameters.Filters)
            {
                var filter = _filterMapper.GetPropertyMapper(searchFilterInfo.Parent, searchFilterInfo.Name);

                var query = GetFilter(filter, searchFilterInfo);
                if (searchFilterInfo.SubQueryParameters != null &&
                    searchFilterInfo.SubQueryParameters.Count > 0)
                {
                    var subQueryValue = BuildSubQuery(searchFilterInfo);
                    query = BuildCustomQuery(searchFilterInfo.SubQueryParameterQueryOperators, query, subQueryValue);
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    queryList.Add(query);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.CustomQuery))
            {
                queryList.Add(parameters.CustomQuery);
            }

            return BuildCustomQuery(parameters.QueryOperator, queryList.ToArray());
        }

        private string BuildCustomQuery(QueryOperators? queryOperator, params string[] queries)
        {
            var value = $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return queries.Length > 1 ? $"({value})" : value;
        }

        private string BuildSubQuery(QueryParameter searchFilterInfo)
        {
            var subQueryList = new List<string>();
            foreach (var additionalSearchFilter in searchFilterInfo.SubQueryParameters)
            {
                var additionalFilter = _filterMapper.GetPropertyMapper(
                    additionalSearchFilter.AdditionalFilterParent, additionalSearchFilter.AdditionalFilterName);
                var subQuery = GetAdditionalFilter(additionalFilter, additionalSearchFilter);
                subQueryList.Add(subQuery);
            }

            return BuildCustomQuery(searchFilterInfo.SubQueryParameterQueryOperators, subQueryList.ToArray());
        }
        
        private string GetFilter(SearchProperty filterMap, QueryParameter searchFilterInfo)
        {
            string searchValue;
            var isAdditionalNullCheckRequired = false;
            var isString = searchFilterInfo.Value is string;

            if (isString && string.IsNullOrWhiteSpace(searchFilterInfo.Value.ToString()))
                return string.Empty;

            if (searchFilterInfo.IsNullCheck && searchFilterInfo.Value == null)
            {
                searchValue = "null";
            }
            else
            {
                searchValue = isString ? $"'{searchFilterInfo.Value}'" : $"{searchFilterInfo.Value.ToString().ToLower()}";
                if (searchFilterInfo.IsNullCheck)
                {
                    isAdditionalNullCheckRequired = true;
                }
            }

            var queryList = new List<string>();

            foreach (var map in filterMap.AzureSearchPropertyMaps)
            {
                queryList.Add($"{map} {searchFilterInfo.LogicalOperator.ToString().ToLower()} {searchValue}");

                if (isAdditionalNullCheckRequired)
                {
                    queryList.Add($"{map} {searchFilterInfo.LogicalOperator.ToString().ToLower()} null");
                }

            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }

        private string GetAdditionalFilter(SearchProperty filterMap, SubQueryParameter additionalSearchFilterInfo)
        {
            string searchValue;
            var isAdditionalNullCheckRequired = false;
            var isString = additionalSearchFilterInfo.Value is string;

            if (isString && string.IsNullOrWhiteSpace(additionalSearchFilterInfo.Value.ToString()))
                return string.Empty;

            if (additionalSearchFilterInfo.IsNullCheck && additionalSearchFilterInfo.Value == null)
            {
                searchValue = "null";
            }
            else
            {
                searchValue = isString ? $"'{additionalSearchFilterInfo.Value}'" : $"{additionalSearchFilterInfo.Value.ToString().ToLower()}";
                if (additionalSearchFilterInfo.IsNullCheck)
                {
                    isAdditionalNullCheckRequired = true;
                }
            }

            var queryList = new List<string>();

            foreach (var map in filterMap.AzureSearchPropertyMaps)
            {
                queryList.Add($"{map} {additionalSearchFilterInfo.LogicalOperator.ToString().ToLower()} {searchValue}");

                if (isAdditionalNullCheckRequired)
                {
                    queryList.Add($"{map} {additionalSearchFilterInfo.LogicalOperator.ToString().ToLower()} null");
                }

            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }
    }
}
