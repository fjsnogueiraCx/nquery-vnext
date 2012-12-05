using System;

namespace NQuery.Symbols
{
    public enum SymbolKind
    {
        BadSymbol,
        BadTable,
        Column,
        SchemaTable,
        DerivedTable,
        TableInstance,
        TableColumnInstance,
        QueryColumnInstance,
        CommonTableExpression,
        Variable,
        Parameter,
        Function,
        Aggregate,
        Method,
        Property
    }
}