using System;
using System.Collections.Generic;
using System.Linq;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    internal class StandardQueryBuilder : QueryBuilder
    {
        private readonly IPropertyMapper _filterMapper;
        private readonly ISearchConfiguration _searchConfiguration;

        internal StandardQueryBuilder(IPropertyMapper filterMapper, ISearchConfiguration searchConfiguration)
        {
            _filterMapper = filterMapper;
            _searchConfiguration = searchConfiguration;
        }

        public override bool IsMatch(SearchQueryParameter searchQueryParameter)
        {
            var searchPropertyMap =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);
            return !searchPropertyMap.IsArray;
        }

        public override string BuildQuery(SearchQueryParameter searchQueryParameter)
        {
            var searchPropertyMap =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            var query = searchQueryParameter.ODataOperator == ODataOperators.SearchIn
                ? GetSearchInFilter(searchPropertyMap, searchQueryParameter.Value)
                : GetFilter(searchPropertyMap, searchQueryParameter);

            if (searchQueryParameter.SubQueryParameters == null || searchQueryParameter.SubQueryParameters.Count <= 0)
                return query;

            var subQueryValue = BuildSubQuery(searchQueryParameter);
            query = BuildCustomQuery(searchQueryParameter.SubQueryParameterQueryOperators, query, subQueryValue);

            return query;
        }

        private string GetFilter(SearchPropertyMap propertyMap, SearchQueryParameter searchQueryParameter)
        {
            string searchValue;
            var isAdditionalNullCheckRequired = false;
            var isString = searchQueryParameter.Value is string;
            var isDateTime = searchQueryParameter.Value is DateTime;

            if (searchQueryParameter.IsNullCheck && searchQueryParameter.Value == null)
            {
                searchValue = "null";
            }
            else if (isDateTime)
            {
                searchValue = ((DateTime) searchQueryParameter.Value).ToString(_searchConfiguration.DateFormat);
            }
            else
            {
                searchValue = isString
                    ? $"'{searchQueryParameter.Value}'"
                    : $"{searchQueryParameter.Value.ToString().ToLower()}";
                if (searchQueryParameter.IsNullCheck)
                {
                    isAdditionalNullCheckRequired = true;
                }
            }


            var queryList = new List<string>();

            if (isString && !string.IsNullOrWhiteSpace(searchQueryParameter.Value.ToString()) || !isString)
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {searchQueryParameter.ODataOperator.ToString().ToLower()} {searchValue}");
            }

            if (isAdditionalNullCheckRequired)
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {searchQueryParameter.ODataOperator.ToString().ToLower()} null");
            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }


        private string GetSearchInFilter(SearchPropertyMap propertyMap, object values)
        {
            if (string.IsNullOrWhiteSpace(values.ToString()))
                return string.Empty;

            var azureSearchPropertyMap = propertyMap.AzureSearchPropertyMap;
            if (propertyMap.PropertyName != propertyMap.ParentPropertyName)
            {
                var parent= _filterMapper.GetSearchPropertyMap(propertyMap.ParentPropertyName, propertyMap.ParentPropertyName);
                if (parent != null)
                {
                    azureSearchPropertyMap = $"{parent.AzureSearchPropertyMap}/{propertyMap.AzureSearchPropertyMap}";
                }
            }

            var queryList = new List<string>()
            {
                $"search.in({azureSearchPropertyMap}, '{values}', '{_searchConfiguration.Delimiter}')"
            };

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }

        private string BuildSubQuery(SearchQueryParameter searchFilterInfo)
        {
            var subQueryList = new List<string>();
            foreach (var additionalSearchFilter in searchFilterInfo.SubQueryParameters)
            {
                var additionalFilter = _filterMapper.GetSearchPropertyMap(
                    additionalSearchFilter.AdditionalFilterParent, additionalSearchFilter.AdditionalFilterName);
                var subQuery = GetAdditionalFilter(additionalFilter, additionalSearchFilter);
                subQueryList.Add(subQuery);
            }

            return BuildCustomQuery(searchFilterInfo.SubQueryParameterQueryOperators, subQueryList.ToArray());
        }

        private string GetAdditionalFilter(SearchPropertyMap propertyMap,
            SearchSubQueryParameter additionalSearchFilterInfo)
        {
            string searchValue;
            var isAdditionalNullCheckRequired = false;
            var isString = additionalSearchFilterInfo.Value is string;
            var isDateTime = additionalSearchFilterInfo.Value is DateTime;

            if (isString && string.IsNullOrWhiteSpace(additionalSearchFilterInfo.Value.ToString()))
                return string.Empty;

            if (additionalSearchFilterInfo.IsNullCheck && additionalSearchFilterInfo.Value == null)
            {
                searchValue = "null";
            }
            else if (isDateTime)
            {
                searchValue = ((DateTime) additionalSearchFilterInfo.Value).ToString(_searchConfiguration.DateFormat);
            }
            else
            {
                searchValue = isString
                    ? $"'{additionalSearchFilterInfo.Value}'"
                    : $"{additionalSearchFilterInfo.Value.ToString().ToLower()}";
                if (additionalSearchFilterInfo.IsNullCheck)
                {
                    isAdditionalNullCheckRequired = true;
                }
            }

            var queryList = new List<string>();

            queryList.Add(
                $"{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} {searchValue}");

            if (isAdditionalNullCheckRequired)
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} null");
            }

            return BuildCustomQuery(QueryOperators.Or, queryList.ToArray());
        }

        protected string BuildCustomQuery(QueryOperators? queryOperator, params string[] queries)
        {
            var value =
                $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return queries.Length > 1 ? $"({value})" : value;
        }
    }
}
