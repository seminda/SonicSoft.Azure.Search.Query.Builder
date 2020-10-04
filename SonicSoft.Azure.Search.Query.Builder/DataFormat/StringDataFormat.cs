using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.DataFormat
{
    public class StringDataFormat:IDataFormat
    {
        public bool IsMatch(SearchQueryParameterBase searchQueryParameter)
        {
            return searchQueryParameter.Type == DataType.String;
        }
        
        public Data GetFormattedValue(SearchQueryParameterBase searchQueryParameter)
        {
            return new Data
            {
                Value = $"'{searchQueryParameter.Value}'",
                IsAdditionalNullCheckRequired = searchQueryParameter.IsNullCheck
            };
        }
    }
}
