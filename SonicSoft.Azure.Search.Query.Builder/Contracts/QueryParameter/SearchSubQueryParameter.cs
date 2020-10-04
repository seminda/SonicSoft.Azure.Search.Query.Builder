using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter
{
    public class SearchSubQueryParameter:SearchQueryParameterBase
    {
        public string AdditionalFilterParent { get; set; }
        public string AdditionalFilterName { get; set; }
        public ODataOperators ODataOperator { get; set; }
    }
}
