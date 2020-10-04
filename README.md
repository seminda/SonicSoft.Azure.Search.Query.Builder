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
```bash
dotnet add package SonicSoft.Azure.Search.Query.Builder --version 1.0.0
```
## Full Documentation
[Full Documentation](https://github.com/seminda/SonicSoft.Azure.Search.Query.Builder/wiki)



## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT License](https://github.com/seminda/SonicSoft.Azure.Search.Query.Builder/blob/master/LICENSE)
