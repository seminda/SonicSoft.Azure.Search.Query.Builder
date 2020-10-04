using System;
using System.Collections.Generic;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.DataFormat;
using SonicSoft.Azure.Search.Query.Builder.Enums;
using SonicSoft.Azure.Search.Query.Builder.Example.CreateQuery.AzureIndexSearchProperties;
using SonicSoft.Azure.Search.Query.Builder.QueryBuilder;
using SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders;


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

            //Step 4: Create DataFormat
            var dataFormats = new List<IDataFormat>()
            {
                new DateTimeFormat(searchConfig),
                new NullDataFormat(),
                new NumberFormat(),
                new StringDataFormat()
            };

            //Step 5: Create QueryBuilder list
            var queryBuilders = new List<IQueryBuilder>
            {
                new StandardQueryBuilder(filterMapper, searchConfig, dataFormats),
                new CollectionQueryBuilder(filterMapper, searchConfig, dataFormats)
            };

            //Step 6: Create SearchQueryBuilder object
            var searchQueryBuilder =new SearchQueryBuilder(queryBuilders);

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
                    SubQueryParameterQueryCondition = QueryConditions.And,
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
            var roomsAndTagQueryParameter = new SearchQueryParameters(roomsAndTagFilters, QueryConditions.And);

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
                searchQueryBuilder.BuildQuery(QueryConditions.Or, roomsAndTagQueryParameter, countryQueryParameter);

            Console.WriteLine("Final Query:");
            Console.WriteLine(azureSearchQuery);
            Console.Read();
        }
    }
}
