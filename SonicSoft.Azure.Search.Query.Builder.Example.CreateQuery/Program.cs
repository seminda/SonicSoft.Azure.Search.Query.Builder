using System;
using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;
using SonicSoft.Azure.Search.Query.Builder.Example.CreateQuery.AzureIndexSearchProperties;
using SonicSoft.Azure.Search.Query.Builder.QueryBuilder;


namespace SonicSoft.Azure.Search.Query.Builder.Example.CreateQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            //Step 1: Create your search properties
            var propertyMaps = new AzureSearchProperties();

            //Step 2: Provide configurations required - Date format and Delimiter 
            var searchConfig = new SearchConfiguration("yyyy-MM-dd", "|");

            //Step 3: Create property mapper object
            var filterMapper = new PropertyMapper(propertyMaps);

            //Step 4: Create SearchQueryBuilder object
            var searchQueryBuilder =new SearchQueryBuilder(filterMapper,searchConfig);

            //Example Query we required to build
            //  "((Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|')) and search.in(Tags, 'view|laundry service', '|')) or Address/Country eq 'USA')"

            //To create above query you have to build your searchQueryParameters in c#

            //Build Rooms and tags query block
            var roomsAndTagFilters = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = AzureSearchProperties.Rooms,
                    Name = AzureSearchProperties.Rooms,
                    Value = "",
                    ODataOperator = ODataOperators.Any,
                    SubQueryParameterQueryOperators = QueryOperators.And,
                    SubQueryParameters = new List<SearchSubQueryParameter>
                    {
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = AzureSearchProperties.Rooms,
                            AdditionalFilterName = AzureSearchProperties.RoomType,
                            Value = "Standard Room",
                            ODataOperator = ODataOperators.Ne
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = AzureSearchProperties.Rooms,
                            AdditionalFilterName = AzureSearchProperties.RoomBaseRate,
                            Value = 100,
                            ODataOperator = ODataOperators.Ge
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = AzureSearchProperties.Rooms,
                            AdditionalFilterName = AzureSearchProperties.Tags,
                            Value = "jacuzzi tub|bathroom shower",
                            ODataOperator = ODataOperators.SearchIn
                        }
                    }
                },
                new SearchQueryParameter()
                {
                    Parent = AzureSearchProperties.Tags,
                    Name = AzureSearchProperties.Tags,
                    Value = "view|laundry service",
                    ODataOperator = ODataOperators.SearchIn
                }
            };

            //This parameter will build the "((Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|')) and search.in(Tags, 'view|laundry service', '|'))"
            var roomsAndTagQueryParameter = new SearchQueryParameters(roomsAndTagFilters,QueryOperators.And);

            //This parameter will build the "Address/Country eq 'USA'"
            var countryQueryParameter = new SearchQueryParameters(new List<SearchQueryParameter>
            {
                new SearchQueryParameter
                {
                    Parent = AzureSearchProperties.Country,
                    Name = AzureSearchProperties.Country,
                    Value = "USA",
                    ODataOperator = ODataOperators.Eq
                }
            });

            //Finally combined the two parameters with or gate

            var azureSearchQuery =
                searchQueryBuilder.BuildQuery(QueryOperators.Or, roomsAndTagQueryParameter, countryQueryParameter);

            Console.WriteLine("Final Query:");
            Console.WriteLine(azureSearchQuery);
            Console.Read();
        }
    }
}
