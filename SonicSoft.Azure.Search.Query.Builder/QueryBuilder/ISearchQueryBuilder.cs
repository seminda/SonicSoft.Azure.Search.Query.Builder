using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder
{
    interface ISearchQueryBuilder
    {
     string BuildQuery(QueryOperators? queryOperator, params QueryParameters[] parameters);
    }
}
