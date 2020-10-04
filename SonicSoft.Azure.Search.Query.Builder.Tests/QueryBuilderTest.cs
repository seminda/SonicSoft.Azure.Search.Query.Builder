using SonicSoft.Azure.Search.Query.Builder.QueryBuilder;
using System.Collections.Generic;
using FluentAssertions;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.PropertyMapper;
using SonicSoft.Azure.Search.Query.Builder.Enums;
using SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders;
using Xunit;

namespace SonicSoft.Azure.Search.Query.Builder.Tests
{
    public class QueryBuilderTest
    {
        private readonly SearchQueryBuilder _queryBuilder;

        public QueryBuilderTest()
        {
            var propertyMaps = new TestAzureSearchProperties();
            var searchConfig = new SearchConfiguration("yyyy-MM-dd", "|");
            var mapper = new PropertyMapper(propertyMaps);
            var queryBuilders = new List<IQueryBuilder>
            {
                new StandardQueryBuilder(mapper, searchConfig),
                new CollectionQueryBuilder(mapper, searchConfig)
            };
            _queryBuilder = new SearchQueryBuilder(queryBuilders);
        }

        [Theory]
        [InlineData(ODataOperators.Eq, "HotelName eq 'Double Sanctuary Resort'")]
        [InlineData(ODataOperators.Ne, "HotelName ne 'Double Sanctuary Resort'")]
        public void QueryBuilder_BuildQuery_One_filter(ODataOperators oDataOperator, string expectedQuery)
        {
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Name = TestAzureSearchProperties.HotelName,
                    Value = "Double Sanctuary Resort",
                    Parent = TestAzureSearchProperties.HotelName,
                    ODataOperator = oDataOperator
                }
            };

            var searchParameters = new List<SearchQueryParameters>()
            {
                new SearchQueryParameters(searchOptions)
            };

            var actualQuery = _queryBuilder.BuildQuery(null, searchParameters.ToArray());

            actualQuery.Should().Be(expectedQuery);
        }

        [Theory]
        [InlineData(QueryConditions.Or, "(HotelName eq 'Double Sanctuary Resort' or Category eq 'Resort and Spa')")]
        [InlineData(QueryConditions.And, "(HotelName eq 'Double Sanctuary Resort' and Category eq 'Resort and Spa')")]
        public void QueryBuilder_BuildQuery_Combine_filter(QueryConditions queryOperator, string expectedQuery)
        {
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.HotelName,
                    Name = TestAzureSearchProperties.HotelName,
                    Value = "Double Sanctuary Resort",
                    ODataOperator = ODataOperators.Eq
                },
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Category,
                    Name = TestAzureSearchProperties.Category,
                    Value = "Resort and Spa",
                    ODataOperator = ODataOperators.Eq
                }
            };

            var searchParameters = new List<SearchQueryParameters>()
            {
                new SearchQueryParameters(searchOptions, queryOperator)
            };

            var actualQuery = _queryBuilder.BuildQuery(null, searchParameters.ToArray());

            actualQuery.Should().Be(expectedQuery);
        }

        [Theory]
        [InlineData(TestAzureSearchProperties.Rooms, TestAzureSearchProperties.RoomType, "Suite|Standard Room",
            "search.in(Rooms/Type, 'Suite|Standard Room', '|')")]
        [InlineData(TestAzureSearchProperties.Tags, TestAzureSearchProperties.Tags, "view|laundry service",
            "search.in(Tags, 'view|laundry service', '|')")]
        public void QueryBuilder_BuildQuery_SearchIn(string parent, string propertyName, string values,
            string expectedQuery)
        {
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = parent,
                    Name = propertyName,
                    Value = values,
                    ODataOperator = ODataOperators.SearchIn
                }
            };

            var actualQuery = _queryBuilder.BuildQuery(null, new SearchQueryParameters(searchOptions));

            actualQuery.Should().Be(expectedQuery);
        }

        [Theory]
        [InlineData(true, "not Rooms/any()")]
        [InlineData(false, "Rooms/any()")]
        public void QueryBuilder_BuildQuery_Any(bool isNullCheck, string expectedQuery)
        {
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Rooms,
                    Name = TestAzureSearchProperties.Rooms,
                    Value = "",
                    IsNullCheck = isNullCheck,
                    ODataOperator = ODataOperators.Any
                }
            };

            var actualQuery = _queryBuilder.BuildQuery(null, new SearchQueryParameters(searchOptions));

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void QueryBuilder_BuildQuery_Any_SubQueryCheck()
        {
            var expectedFilter =
                "Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and x/BaseRate le 200)";
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Rooms,
                    Name = TestAzureSearchProperties.Rooms,
                    Value = "",
                    Type = DataType.String,
                    ODataOperator = ODataOperators.Any,
                    SubQueryParameterQueryCondition =  QueryConditions.And,
                    SubQueryParameters = new List<SearchSubQueryParameter>
                    {
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomType,
                            Value = "Standard Room",
                            Type = DataType.String,
                            ODataOperator = ODataOperators.Ne
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomBaseRate,
                            Value = 100,
                            Type = DataType.Number,
                            ODataOperator = ODataOperators.Ge
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomBaseRate,
                            Value = 200,
                            Type = DataType.Number,
                            ODataOperator = ODataOperators.Le
                        }
                    }
                }
            };

            var actualQuery =
                _queryBuilder.BuildQuery(null, new SearchQueryParameters(searchOptions, QueryConditions.And));

            actualQuery.Should().Be(expectedFilter);
        }

        [Fact]
        public void QueryBuilder_BuildQuery_Any_SubQuerySearchIn()
        {
            var expectedFilter =
                "Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|'))";
            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Rooms,
                    Name = TestAzureSearchProperties.Rooms,
                    Value = "",
                    Type = DataType.String,
                    ODataOperator = ODataOperators.Any,
                    SubQueryParameterQueryCondition = QueryConditions.And,
                    SubQueryParameters = new List<SearchSubQueryParameter>
                    {
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomType,
                            Value = "Standard Room",
                            Type = DataType.String,
                            ODataOperator = ODataOperators.Ne
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomBaseRate,
                            Value = 100,
                            Type = DataType.Number,
                            ODataOperator = ODataOperators.Ge
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.Tags,
                            Value = "jacuzzi tub|bathroom shower",
                            Type = DataType.String,
                            ODataOperator = ODataOperators.SearchIn
                        }
                    }
                }
            };

            var actualQuery =
                _queryBuilder.BuildQuery(null, new SearchQueryParameters(searchOptions, QueryConditions.And));

            actualQuery.Should().Be(expectedFilter);
        }

        [Fact]
        public void QueryBuilder_BuildQuery_complex_quey()
        {
            var expectedFilter =
                "((Rooms/any(x: x/Type ne 'Standard Room' and x/BaseRate ge 100 and search.in(x/Tags, 'jacuzzi tub|bathroom shower','|')) and search.in(Tags, 'view|laundry service', '|')) or Address/Country eq 'USA')";

            var searchOptions = new List<SearchQueryParameter>
            {
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Rooms,
                    Name = TestAzureSearchProperties.Rooms,
                    Value = "",
                    Type = DataType.String,
                    ODataOperator = ODataOperators.Any,
                    SubQueryParameterQueryCondition = QueryConditions.And,
                    SubQueryParameters = new List<SearchSubQueryParameter>
                    {
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomType,
                            Value = "Standard Room",
                            Type = DataType.String,
                            ODataOperator = ODataOperators.Ne
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.RoomBaseRate,
                            Value = 100,
                            Type = DataType.Number,
                            ODataOperator = ODataOperators.Ge
                        },
                        new SearchSubQueryParameter
                        {
                            AdditionalFilterParent = TestAzureSearchProperties.Rooms,
                            AdditionalFilterName = TestAzureSearchProperties.Tags,
                            Value = "jacuzzi tub|bathroom shower",
                            Type = DataType.String,
                            ODataOperator = ODataOperators.SearchIn
                        }
                    }
                },
                new SearchQueryParameter()
                {
                    Parent = TestAzureSearchProperties.Tags,
                    Name = TestAzureSearchProperties.Tags,
                    Value = "view|laundry service",
                    Type = DataType.String,
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
                _queryBuilder.BuildQuery(QueryConditions.Or, new SearchQueryParameters(searchOptions, QueryConditions.And), countryQuery);

            actualQuery.Should().Be(expectedFilter);
        }
    }
}
