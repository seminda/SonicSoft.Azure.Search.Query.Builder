using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;

namespace SonicSoft.Azure.Search.Query.Builder.DataFormat
{
    public interface IDataFormat
    {
        bool IsMatch(SearchQueryParameterBase searchQueryParameter);

        Data GetFormattedValue(SearchQueryParameterBase searchQueryParameter);
    }
}
