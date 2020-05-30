using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    internal class CollectionQueryBuilder : QueryBuilder
    {
        private readonly IPropertyMapper _filterMapper;
        private readonly ISearchConfiguration _searchConfiguration;
        private readonly string _itemName = "x";

        internal CollectionQueryBuilder(IPropertyMapper filterMapper, ISearchConfiguration searchConfiguration)
        {
            _filterMapper = filterMapper;
            _searchConfiguration = searchConfiguration;
        }

        public override bool IsMatch(SearchQueryParameter searchQueryParameter)
        {
            var searchProperty = _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            return searchProperty.IsArray;
        }

        public override string BuildQuery(SearchQueryParameter searchQueryParameter)
        {
            var searchProperty = _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            if ((searchQueryParameter.IsNullCheck || searchQueryParameter.SubQueryParameters == null ||
                 searchQueryParameter.SubQueryParameters.Count == 0) &&
                searchQueryParameter.ODataOperator == ODataOperators.Any)
            {
                return EmptyFieldQuery(searchQueryParameter, searchProperty, searchQueryParameter.IsNullCheck);
            }

            return BuildCollectionQuery(searchQueryParameter, searchProperty);
        }

        private string EmptyFieldQuery(SearchQueryParameter searchFilterInfo, SearchPropertyMap searchProperty, bool isNullCheck)
        {
            var anyQuery = $"{searchProperty.AzureSearchPropertyMaps[0]}/{searchFilterInfo.ODataOperator.ToString().ToLower()}()";

            return isNullCheck ? $"not {anyQuery}" : anyQuery;
        }

        private string BuildCollectionQuery(SearchQueryParameter searchQueryParameter, SearchPropertyMap searchProperty)
        {
            var itemName = "x";
            var queryList = new List<string>();
            if (searchQueryParameter.ODataOperator == ODataOperators.SearchIn)
            {
                throw new NotImplementedException();
            }

            foreach (var azureSearchPropertyMap in searchProperty.AzureSearchPropertyMaps)
            {
                StringBuilder builder = new StringBuilder($"{azureSearchPropertyMap}/{searchQueryParameter.ODataOperator.ToString().ToLower()}({itemName}: ");
                builder.Append(BuildSubQuery(searchQueryParameter));
                builder.Append(")");
                queryList.Add(builder.ToString());
            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }

        private string BuildSubQuery(SearchQueryParameter searchQueryParameter)
        {
            var subQueryList = new List<string>();
            foreach (var additionalSearchFilter in searchQueryParameter.SubQueryParameters)
            {
                var additionalFilter = _filterMapper.GetSearchPropertyMap(
                    additionalSearchFilter.AdditionalFilterParent, additionalSearchFilter.AdditionalFilterName);
                var subQuery = GetAdditionalFilter(additionalFilter, additionalSearchFilter);
                subQueryList.Add(subQuery);


            }

            return BuildCustomQuery(searchQueryParameter.SubQueryParameterQueryOperators, subQueryList.ToArray());
        }

        private string GetAdditionalFilter(SearchPropertyMap propertyMap, SearchSubQueryParameter additionalSearchFilterInfo)
        {
            var queryList = new List<string>();
            if (additionalSearchFilterInfo.ODataOperator == ODataOperators.SearchIn)
            {
                if (string.IsNullOrWhiteSpace(additionalSearchFilterInfo.Value.ToString()))
                    return string.Empty;

                foreach (var map in propertyMap.AzureSearchPropertyMaps)
                {
                    queryList.Add($"search.in({_itemName}/{map}, '{additionalSearchFilterInfo.Value}',{_searchConfiguration.Delimiter})");
                }
            }
            else
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

                foreach (var map in propertyMap.AzureSearchPropertyMaps)
                {
                    queryList.Add($"{_itemName}/{map} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} {searchValue}");

                    if (isAdditionalNullCheckRequired)
                    {
                        queryList.Add($"{_itemName}/{map} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} null");
                    }

                }
            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }

        protected string BuildCustomQuery(QueryOperators? queryOperator, params string[] queries)
        {
            var value = $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return value;
        }
    }
}
