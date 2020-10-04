using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.DataFormat
{
    public class NumberFormat:IDataFormat
    {
        public bool IsMatch(SearchQueryParameterBase searchQueryParameter)
        {
            return searchQueryParameter.Type == DataType.Number;
        }
        
        public Data GetFormattedValue(SearchQueryParameterBase searchQueryParameter)
        {
            return new Data
            {
                Value = $"{searchQueryParameter.Value.ToString().ToLower()}",
                IsAdditionalNullCheckRequired = searchQueryParameter.IsNullCheck
            };
        }
    }
}
