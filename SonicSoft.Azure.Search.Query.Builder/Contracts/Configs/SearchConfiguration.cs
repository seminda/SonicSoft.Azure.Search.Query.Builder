namespace SonicSoft.Azure.Search.Query.Builder.Contracts.Configs
{
   public class SearchConfiguration: ISearchConfiguration
    {
        public SearchConfiguration(string dateFormat,string delimiter)
        {
            DateFormat = dateFormat;
            Delimiter = delimiter;
        }
        public string DateFormat { get;  }
        public string Delimiter { get;  }
    }
}
