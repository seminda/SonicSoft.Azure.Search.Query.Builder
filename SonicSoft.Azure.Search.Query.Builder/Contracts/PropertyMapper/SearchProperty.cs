using System;
using System.Collections.Generic;
using System.Text;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public class SearchProperty
    {
        public string ParentPropertyName { get; set; }
        public string PropertyName { get; set; }
        public List<string> AzureSearchPropertyMaps { get; set; }
    }
}
