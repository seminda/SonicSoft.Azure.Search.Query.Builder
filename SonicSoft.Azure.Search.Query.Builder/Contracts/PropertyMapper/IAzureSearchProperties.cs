using System.Collections.Generic;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public interface IAzureSearchProperties
    {
        List<SearchPropertyMap> SearchPropertyMaps { get; set; }
    }
}
