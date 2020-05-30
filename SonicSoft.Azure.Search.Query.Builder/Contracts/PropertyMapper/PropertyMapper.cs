using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public class PropertyMapper : IPropertyMapper
    {
        private List<SearchPropertyMap> _propertyMaps;

        public PropertyMapper(IAzureSearchProperties azureSearchProperties)
        {
            _propertyMaps = azureSearchProperties?.SearchPropertyMaps ??
                            throw new InvalidDataContractException("List of azure search property mapping required");
        }

        public SearchPropertyMap GetSearchPropertyMap(string parentProperty, string propertyName)
        {
            return _propertyMaps.SingleOrDefault(s =>
                string.Equals(s.ParentPropertyName, parentProperty, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(s.PropertyName, propertyName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
