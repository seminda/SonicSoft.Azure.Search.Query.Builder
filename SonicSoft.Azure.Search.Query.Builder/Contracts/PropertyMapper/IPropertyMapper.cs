namespace SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper
{
    public interface IPropertyMapper
    {
        SearchProperty GetPropertyMapper(string parentProperty, string propertyName);
    }
}
