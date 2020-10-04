using System;
using SonicSoft.Azure.Search.Query.Builder.Contracts;
using SonicSoft.Azure.Search.Query.Builder.Contracts.Configs;
using SonicSoft.Azure.Search.Query.Builder.Contracts.QueryParameter;
using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.DataFormat
{
    public class DateTimeFormat : IDataFormat
    {
        private readonly string _dateFormat;

        public DateTimeFormat(ISearchConfiguration searchConfiguration)
        {
            _dateFormat = searchConfiguration.DateFormat;
        }

        public bool IsMatch(SearchQueryParameterBase searchQueryParameter)
        {
            return searchQueryParameter.Type == DataType.DateTime;
        }
        
        public Data GetFormattedValue(SearchQueryParameterBase searchQueryParameter)
        {
            return new Data
            {
                Value = ((DateTime) searchQueryParameter.Value).ToString(_dateFormat),
                IsAdditionalNullCheckRequired = searchQueryParameter.IsNullCheck
            };
        }
    }
}
