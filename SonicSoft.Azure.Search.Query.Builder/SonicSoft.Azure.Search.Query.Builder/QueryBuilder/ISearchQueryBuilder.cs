using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder
{
    public interface ISearchQueryBuilder
    {
        string BuildQuery(QueryOperators? queryOperator, params SearchQueryParameters[] parameters);
        string BuildCustomQuery(QueryOperators? queryOperator, params string[] queries);
    }
}
