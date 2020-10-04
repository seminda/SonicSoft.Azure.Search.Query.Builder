using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter
{
    public class SearchQueryParameterBase
    {
        public object Value { get; set; }
        public DataType Type { get; set; }
        public bool IsNullCheck { get; set; }
    }
}
