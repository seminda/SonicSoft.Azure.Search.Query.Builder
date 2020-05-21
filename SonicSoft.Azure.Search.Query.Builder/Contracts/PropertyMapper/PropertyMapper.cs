using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public class PropertyMapper:IPropertyMapper
    {
        private List<SearchProperty> _propertyMaps;

        public PropertyMapper(List<SearchProperty> properties)
        {
            _propertyMaps = properties ??
                            throw new InvalidDataContractException("List of azure search property mapping required");
        }
        public SearchProperty GetPropertyMapper(string parentProperty, string propertyName)
        {
            return _propertyMaps.SingleOrDefault(s =>
                string.Equals(s.ParentPropertyName, parentProperty, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(s.PropertyName, propertyName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
