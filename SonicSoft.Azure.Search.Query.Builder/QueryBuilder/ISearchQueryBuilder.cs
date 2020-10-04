using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder
{
    public interface ISearchQueryBuilder
    {
        string BuildQuery(QueryConditions? queryOperator, params SearchQueryParameters[] parameters);
        string BuildCustomQuery(QueryConditions? queryOperator, params string[] queries);
    }
}
