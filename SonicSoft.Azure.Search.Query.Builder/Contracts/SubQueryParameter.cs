﻿using SonicSoft.Azure.Search.Query.Builder.Enums;

namespace SonicSoft.Azure.Search.Query.Builder.Contracts
{
    public class SubQueryParameter
    {
        public string AdditionalFilterParent { get; set; }
        public string AdditionalFilterName { get; set; }
        public object Value { get; set; }
        public bool IsNullCheck { get; set; }
        public LogicalOperators LogicalOperator { get; set; }
    }
}
