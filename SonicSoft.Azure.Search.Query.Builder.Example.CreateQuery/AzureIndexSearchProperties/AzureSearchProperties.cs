using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;

namespace SonicSoft.Azure.Search.Query.Builder.Example.CreateQuery.AzureIndexSearchProperties
{
    public class AzureSearchProperties:IAzureSearchProperties
    {
        public const string HotelName = "HotelName";
        public const string Category = "Category";
        public const string Tags = "Tags";
        public const string StreetAddress = "StreetAddress";
        public const string City = "City";
        public const string PostalCode = "PostalCode";
        public const string Country = "Country";
        public const string Rooms = "Rooms";
        public const string RoomType = "Type";
        public const string RoomBaseRate = "BaseRate";

        public AzureSearchProperties()
        {
            SearchPropertyMaps = new List<SearchPropertyMap>()
            {
                CreateSearchPropertyMap(HotelName, HotelName, HotelName),
                CreateSearchPropertyMap(Category, Category, Category),
                CreateSearchPropertyMap(Tags, Tags, Tags, true),
                CreateSearchPropertyMap(StreetAddress, StreetAddress, $"Address/{StreetAddress}"),
                CreateSearchPropertyMap(City, City, $"Address/{City}"),
                CreateSearchPropertyMap(PostalCode, PostalCode, $"Address/{PostalCode}"),
                CreateSearchPropertyMap(Country, Country, $"Address/{Country}"),
                CreateSearchPropertyMap(Rooms, Rooms, Rooms, true),
                CreateSearchPropertyMap(Rooms, RoomType, $"{RoomType}"),
                CreateSearchPropertyMap(Rooms, RoomBaseRate, $"{RoomBaseRate}"),
                CreateSearchPropertyMap(Rooms, Tags, $"{Tags}", true)
            };
        }

        public List<SearchPropertyMap> SearchPropertyMaps { get; set; }

        private SearchPropertyMap CreateSearchPropertyMap(string parent, string property, string azureSearchPropertyMap,
            bool isArray = false)
        {
            return new SearchPropertyMap
            {
                ParentPropertyName = parent,
                PropertyName = property,
                AzureSearchPropertyMap = azureSearchPropertyMap,
                IsCollection = isArray
            };
        }
    }
}
