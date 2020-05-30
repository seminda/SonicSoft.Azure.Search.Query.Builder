# Introduction 

Azure Cognitive Search (formerly known as "Azure Search") is a search-as-a-service cloud solution that gives developers APIs and tools for adding a rich search experience over private, heterogeneous content in web, mobile, and enterprise applications.

SonicSoft azure search query builder design and develop to support any c# project that required to create  filter queries for retrieve data from  Azure Cognitive Search index. This query builder supports most of the query types. More details about supported query types and details of how to use this package included in the below sections.

For more details about Azure Cognitive Search please refer to this link: [Azure Cognitive Search documentation](https://docs.microsoft.com/en-us/azure/search/)  

## Supported Query types
- Collection filter expressions using **any**.
- Logical expressions that combine other Boolean expressions using the operators **and**, **or**, and **not**.
- Comparison expressions, which compare fields or range variables to constant values using the operators **eq**, **ne**, **gt**, **lt**, **ge**, and **le**. 
-The Boolean literals **true** and **false**.
- **search.in**, which compares a field or range variable with each value in a list of values.

Learn more about filters: [OData $filter syntax in Azure Cognitive Search](https://docs.microsoft.com/en-us/azure/search/search-query-odata-filter)

## Sample filter queries you can build using SonicSoft azure search query builder
```bash
HotelName eq 'Double Sanctuary Resort'
(HotelName eq 'Double Sanctuary Resort' or Category eq 'Resort and Spa')
search.in(Rooms/Type, 'Suite|Standard Room', '|')
Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and x/BaseRate le 200)
Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|'))
((Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|')) and search.in(Tags, 'view|laundry service', '|')) or Address/Country eq 'USA')
```
## Installation

install the package

## Usage

#### Step 1
- Create your azure search index property list by inheriting **IAzureSearchProperties** interface
- Example
```c#
public class AzureSearchProperties : IAzureSearchProperties
{
	public const string HotelName = "HotelName";
	public const string StreetAddress = "StreetAddress";
	public const string Rooms = "Rooms";
	public const string RoomType = "Type";
	public const string RoomBaseRate = "BaseRate";

	public AzureSearchProperties()
	{
		SearchPropertyMaps = new List<SearchPropertyMap>()
		{
			CreateSearchPropertyMap(HotelName, HotelName, HotelName),
			CreateSearchPropertyMap(StreetAddress, StreetAddress, $"Address/{StreetAddress}"),
			CreateSearchPropertyMap(Rooms, Rooms, Rooms, true),
			CreateSearchPropertyMap(Rooms, RoomType, $"{RoomType}"),
			CreateSearchPropertyMap(Rooms, RoomBaseRate, $"{RoomBaseRate}"),
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
			IsArray = isArray
		};
	}
}
```
#### Step 2
- Create or dependency inject **SearchQueryBuilder** object
- Example
```c#
 var searchConfig = new SearchConfiguration("yyyy-MM-dd", "|");
```
#### Step 3
- Create or dependency inject **PropertyMapper** 
- Example
```c#
 var propertyMaps = new AzureSearchProperties();
 var mapper = new PropertyMapper(propertyMaps);
```


#### Step 4
- Finally Create or dependency inject you search builder
- Example
```c#
var _queryBuilder = new SearchQueryBuilder(mapper, searchConfig);
```
#### Step 5
- Create your **SearchQueryParameters** and  Build your query using query builder
- Example 1
```c#
var searchOptions = new List<SearchQueryParameter>
	{
		new SearchQueryParameter()
		{
			Name = TestAzureSearchProperties.HotelName,
			Value = "Double Sanctuary Resort",
			Parent = TestAzureSearchProperties.HotelName,
			ODataOperator = ODataOperators.Eq
		}
	};
            
var actualQuery = _queryBuilder.BuildQuery(null, new SearchQueryParameters(searchOptions));
```
- Example 1 result
```bash
"HotelName eq 'Double Sanctuary Resort'"
```
- Example 2
```c#
 var searchOptions = new List<SearchQueryParameter>
 {
    new SearchQueryParameter()
    {
		Parent = TestAzureSearchProperties.Rooms,
		Name = TestAzureSearchProperties.Rooms,
		Value = "",
		ODataOperator = ODataOperators.Any,
		SubQueryParameterQueryOperators = QueryOperators.And,
		SubQueryParameters = new List<SearchSubQueryParameter>
		{
			new SearchSubQueryParameter
			{
				AdditionalFilterParent = TestAzureSearchProperties.Rooms,
				AdditionalFilterName = TestAzureSearchProperties.RoomType,
				Value = "Standard Room",
				ODataOperator = ODataOperators.Ne
			},
			new SearchSubQueryParameter
			{
				AdditionalFilterParent = TestAzureSearchProperties.Rooms,
				AdditionalFilterName = TestAzureSearchProperties.RoomBaseRate,
				Value = 100,
				ODataOperator = ODataOperators.Ge
			},
			new SearchSubQueryParameter
			{
				AdditionalFilterParent = TestAzureSearchProperties.Rooms,
				AdditionalFilterName = TestAzureSearchProperties.Tags,
				Value = "jacuzzi tub|bathroom shower",
				ODataOperator = ODataOperators.SearchIn
			}
		}
	},
	new SearchQueryParameter()
	{
		Parent = TestAzureSearchProperties.Tags,
		Name = TestAzureSearchProperties.Tags,
		Value = "view|laundry service",
		ODataOperator = ODataOperators.SearchIn
	}
};

var countryQuery = new SearchQueryParameters(new List<SearchQueryParameter>
{
	new SearchQueryParameter
	{
		Parent = TestAzureSearchProperties.Country,
		Name = TestAzureSearchProperties.Country,
		Value = "USA",
		ODataOperator = ODataOperators.Eq
	}
});

var actualQuery =
	_queryBuilder.BuildQuery(QueryOperators.Or, new SearchQueryParameters(searchOptions, QueryOperators.And), countryQuery);
```
- Example 2 result
```bash
"((Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|')) and search.in(Tags, 'view|laundry service', '|')) or Address/Country eq 'USA')"
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[SonicSoft License](https://github.com/seminda/SonicSoft.Azure.Search.Query.Builder/blob/master/LICENSE)