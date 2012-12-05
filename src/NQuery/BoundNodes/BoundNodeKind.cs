using System;

namespace NQuery.BoundNodes
{
    internal enum BoundNodeKind
    {
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        FunctionInvocationExpression,
        AggregateExpression,
        PropertyAccessExpression,
        MethodInvocationExpression,
        CastExpression,
        IsNullExpression,
        CaseLabel,
        CaseExpression,
        SingleRowSubselect,
        ExistsSubselect,
        AllAnySubselect,
        ValueSlotExpression,
        NamedTableReference,
        DerivedTableReference,
        JoinedTableReference,
        SelectColumn,
        WildcardSelectColumn,
        SelectQuery,
        CombinedQuery,
        CommonTableExpression,
        CommonTableExpressionQuery,
        OrderedQuery,
        TopQuery
    }
}