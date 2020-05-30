namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public interface IPropertyMapper
    {
        SearchPropertyMap GetSearchPropertyMap(string parentProperty, string propertyName);
    }
}
