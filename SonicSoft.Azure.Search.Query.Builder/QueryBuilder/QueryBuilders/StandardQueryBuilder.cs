using System.Collections.Generic;
using System.Linq;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.DataFormat;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    public class StandardQueryBuilder : IQueryBuilder
    {
        private readonly IPropertyMapper _filterMapper;
        private readonly ISearchConfiguration _searchConfiguration;
        private readonly IEnumerable<IDataFormat> _dataFormats;
        public StandardQueryBuilder(IPropertyMapper filterMapper, ISearchConfiguration searchConfiguration, IEnumerable<IDataFormat> dataFormats)
        {
            _filterMapper = filterMapper;
            _searchConfiguration = searchConfiguration;
            _dataFormats = dataFormats;
        }

        public bool IsMatch(SearchQueryParameter searchQueryParameter)
        {
            var searchPropertyMap =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);
            return !searchPropertyMap.IsCollection;
        }

        public string BuildQuery(SearchQueryParameter searchQueryParameter)
        {
            var searchPropertyMap =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            var query = searchQueryParameter.ODataOperator == ODataOperators.SearchIn
                ? GetSearchInFilter(searchPropertyMap, searchQueryParameter.Value)
                : GetFilter(searchPropertyMap, searchQueryParameter);

            if (searchQueryParameter.SubQueryParameters == null || searchQueryParameter.SubQueryParameters.Count <= 0)
                return query;

            var subQueryValue = BuildSubQuery(searchQueryParameter);
            query = BuildCustomQuery(searchQueryParameter.SubQueryParameterQueryCondition, query, subQueryValue);

            return query;
        }

        private string GetFilter(SearchPropertyMap propertyMap, SearchQueryParameter searchQueryParameter)
        {
            var data= _dataFormats.First(s => s.IsMatch(searchQueryParameter)).GetFormattedValue(searchQueryParameter);

            var queryList = new List<string>();

            if (!string.IsNullOrWhiteSpace(data.Value))
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {searchQueryParameter.ODataOperator.ToString().ToLower()} {data.Value}");
            }

            if (data.IsAdditionalNullCheckRequired)
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {searchQueryParameter.ODataOperator.ToString().ToLower()} null");
            }

            return BuildCustomQuery(QueryConditions.Or, queryList.ToArray());
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

            return BuildCustomQuery(QueryConditions.Or, queryList.ToArray());
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

            return BuildCustomQuery(searchFilterInfo.SubQueryParameterQueryCondition, subQueryList.ToArray());
        }

        private string GetAdditionalFilter(SearchPropertyMap propertyMap,
            SearchSubQueryParameter additionalSearchFilterInfo)
        {
           var data = _dataFormats.First(s => s.IsMatch(additionalSearchFilterInfo)).GetFormattedValue(additionalSearchFilterInfo);

            var queryList = new List<string>();

            queryList.Add(
                $"{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} {data.Value}");

            if (data.IsAdditionalNullCheckRequired)
            {
                queryList.Add(
                    $"{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} null");
            }

            return BuildCustomQuery(QueryConditions.Or, queryList.ToArray());
        }

        protected string BuildCustomQuery(QueryConditions? queryOperator, params string[] queries)
        {
            var value =
                $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return queries.Length > 1 ? $"({value})" : value;
        }
    }
}
