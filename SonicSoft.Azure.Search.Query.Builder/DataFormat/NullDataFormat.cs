using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;

namespace SonicSoft.Azure.Search.Query.Builder.DataFormat
{
    public class NullDataFormat:IDataFormat
    {
        public bool IsMatch(SearchQueryParameterBase searchQueryParameter)
        {
            return searchQueryParameter.IsNullCheck && searchQueryParameter.Value == null;
        }
        
        public Data GetFormattedValue(SearchQueryParameterBase searchQueryParameter)
        {
            return new Data
            {
                Value = "null",
                IsAdditionalNullCheckRequired = false
            };
        }
    }
}
