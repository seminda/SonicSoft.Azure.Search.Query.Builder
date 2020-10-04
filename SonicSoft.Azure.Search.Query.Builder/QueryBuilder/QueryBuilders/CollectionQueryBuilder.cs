using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.DataFormat;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    public class CollectionQueryBuilder : IQueryBuilder
    {
        private readonly IPropertyMapper _filterMapper;
        private readonly ISearchConfiguration _searchConfiguration;
        private readonly IEnumerable<IDataFormat> _dataFormats;

        private readonly string _itemName = "x";

        public CollectionQueryBuilder(IPropertyMapper filterMapper, ISearchConfiguration searchConfiguration, IEnumerable<IDataFormat> dataFormats)
        {
            _filterMapper = filterMapper;
            _searchConfiguration = searchConfiguration;
            _dataFormats = dataFormats;
        }

        public bool IsMatch(SearchQueryParameter searchQueryParameter)
        {
            var searchProperty =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            return searchProperty.IsCollection;
        }

        public string BuildQuery(SearchQueryParameter searchQueryParameter)
        {
            var searchProperty =
                _filterMapper.GetSearchPropertyMap(searchQueryParameter.Parent, searchQueryParameter.Name);

            if ((searchQueryParameter.IsNullCheck || searchQueryParameter.SubQueryParameters == null ||
                 searchQueryParameter.SubQueryParameters.Count == 0) &&
                searchQueryParameter.ODataOperator == ODataOperators.Any)
            {
                return EmptyFieldQuery(searchQueryParameter, searchProperty, searchQueryParameter.IsNullCheck);
            }

            return BuildCollectionQuery(searchQueryParameter, searchProperty);
        }

        private string EmptyFieldQuery(SearchQueryParameter searchFilterInfo, SearchPropertyMap searchProperty,
            bool isNullCheck)
        {
            var anyQuery =
                $"{searchProperty.AzureSearchPropertyMap}/{searchFilterInfo.ODataOperator.ToString().ToLower()}()";

            return isNullCheck ? $"not {anyQuery}" : anyQuery;
        }

        private string BuildCollectionQuery(SearchQueryParameter searchQueryParameter, SearchPropertyMap searchProperty)
        {
            var queryList = new List<string>();
            if (searchQueryParameter.ODataOperator == ODataOperators.SearchIn)
            {
              return  GetSearchInFilter(searchProperty, searchQueryParameter.Value);
            }

            StringBuilder builder =
                new StringBuilder(
                    $"{searchProperty.AzureSearchPropertyMap}/{searchQueryParameter.ODataOperator.ToString().ToLower()}({_itemName}: ");
            builder.Append(BuildSubQuery(searchQueryParameter));
            builder.Append(")");

            queryList.Add(builder.ToString());

            return BuildCustomQuery(QueryConditions.Or, queryList.ToArray());
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

            return BuildCustomQuery(searchQueryParameter.SubQueryParameterQueryCondition, subQueryList.ToArray());
        }

        private string GetAdditionalFilter(SearchPropertyMap propertyMap,
            SearchSubQueryParameter additionalSearchFilterInfo)
        {
            var queryList = new List<string>();
            if (additionalSearchFilterInfo.ODataOperator == ODataOperators.SearchIn)
            {
                if (string.IsNullOrWhiteSpace(additionalSearchFilterInfo.Value.ToString()))
                    return string.Empty;

                queryList.Add(
                    $"search.in({_itemName}/{propertyMap.AzureSearchPropertyMap}, '{additionalSearchFilterInfo.Value}','{_searchConfiguration.Delimiter}')");
            }
            else
            {
                var data = _dataFormats.First(s => s.IsMatch(additionalSearchFilterInfo)).GetFormattedValue(additionalSearchFilterInfo);

                if (string.IsNullOrWhiteSpace(data.Value))
                    return string.Empty;
                
                queryList.Add(
                    $"{_itemName}/{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} {data.Value}");

                if (data.IsAdditionalNullCheckRequired)
                {
                    queryList.Add(
                        $"{_itemName}/{propertyMap.AzureSearchPropertyMap} {additionalSearchFilterInfo.ODataOperator.ToString().ToLower()} null");
                }
            }

            return BuildCustomQuery(QueryConditions.Or, queryList.ToArray());
        }

        protected string BuildCustomQuery(QueryConditions? queryOperator, params string[] queries)
        {
            var value =
                $"{string.Join($" {queryOperator?.ToString().ToLower()} ", queries.Where(s => !string.IsNullOrWhiteSpace(s)))}";

            return value;
        }

        private string GetSearchInFilter(SearchPropertyMap propertyMap, object values)
        {
            if (string.IsNullOrWhiteSpace(values.ToString()))
                return string.Empty;

            var azureSearchPropertyMap = propertyMap.AzureSearchPropertyMap;
            if (propertyMap.PropertyName != propertyMap.ParentPropertyName)
            {
                var parent = _filterMapper.GetSearchPropertyMap(propertyMap.ParentPropertyName, propertyMap.ParentPropertyName);
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
    }
}
