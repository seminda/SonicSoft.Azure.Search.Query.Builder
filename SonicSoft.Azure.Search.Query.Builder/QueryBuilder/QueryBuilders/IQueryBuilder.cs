using SonicSoft.Azure.Search.Query.Builder.Contracts;

namespace SonicSoft.Azure.Search.Query.Builder.QueryBuilder.QueryBuilders
{
    public interface IQueryBuilder
    {
        bool IsMatch(SearchQueryParameter searchQueryParameter);
        string BuildQuery(SearchQueryParameter searchQueryParameter);
    }
}
