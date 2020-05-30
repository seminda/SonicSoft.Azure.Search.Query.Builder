using System.Collections.Generic;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public class SearchPropertyMap
    {
        public string ParentPropertyName { get; set; }
        public string PropertyName { get; set; }
        public bool IsArray { get; set; }
        public string AzureSearchPropertyMap { get; set; }
    }
}
