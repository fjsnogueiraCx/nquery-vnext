﻿using System;
using System.Collections.Generic;
using System.Linq;
using NQuery.Language.Binding;
using NQuery.Language.BoundNodes;
using NQuery.Language.Symbols;

namespace NQuery.Language
{
    public sealed class SemanticModel
    {
        private readonly Compilation _compilation;
        private readonly BindingResult _bindingResult;

        internal SemanticModel(Compilation compilation, BindingResult bindingResult)
        {
            _compilation = compilation;
            _bindingResult = bindingResult;
        }

        public Compilation Compilation
        {
            get { return _compilation; }
        }

        public Conversion ClassifyConversion(Type sourceType, Type targetType)
        {
            return Conversion.Classify(sourceType, targetType);
        }

        public Symbol GetSymbol(ExpressionSyntax expression)
        {
            var boundExpression = GetBoundExpression(expression);
            return boundExpression == null ? null : GetSymbol(boundExpression);
        }

        private static Symbol GetSymbol(BoundExpression expression)
        {
            switch (expression.Kind)
            {
                case BoundNodeKind.NameExpression:
                    return GetSymbol((BoundNameExpression) expression);
                case BoundNodeKind.VariableExpression:
                    return GetSymbol((BoundVariableExpression) expression);
                case BoundNodeKind.FunctionInvocationExpression:
                    return GetSymbol((BoundFunctionInvocationExpression) expression);
                case BoundNodeKind.AggregateExpression:
                    return GetSymbol((BoundAggregateExpression) expression);
                case BoundNodeKind.PropertyAccessExpression:
                    return GetSymbol((BoundPropertyAccessExpression) expression);
                case BoundNodeKind.MethodInvocationExpression:
                    return GetSymbol(((BoundMethodInvocationExpression) expression));
                default:
                    return null;
            }
        }

        private static Symbol GetSymbol(BoundNameExpression expression)
        {
            return expression.Symbol;
        }

        private static Symbol GetSymbol(BoundVariableExpression expression)
        {
            return expression.Symbol;
        }

        private static Symbol GetSymbol(BoundFunctionInvocationExpression expression)
        {
            return expression.Symbol;
        }

        private static Symbol GetSymbol(BoundAggregateExpression expression)
        {
            return expression.Symbol;
        }

        private static Symbol GetSymbol(BoundPropertyAccessExpression expression)
        {
            return expression.Symbol;
        }

        private static Symbol GetSymbol(BoundMethodInvocationExpression expression)
        {
            return expression.Symbol;
        }

        public Type GetExpressionType(ExpressionSyntax expression)
        {
            var boundExpression = GetBoundExpression(expression);
            return boundExpression == null ? null : boundExpression.Type;
        }

        public Conversion GetConversion(CastExpressionSyntax expression)
        {
            var boundExpression = GetBoundExpression(expression) as BoundCastExpression;
            return boundExpression == null ? null : boundExpression.Conversion;
        }

        private BoundExpression GetBoundExpression(ExpressionSyntax expression)
        {
            return _bindingResult.GetBoundNode(expression) as BoundExpression;
        }

        public CommonTableExpressionSymbol GetDeclaredSymbol(CommonTableExpressionSyntax commonTableExpression)
        {
            var result = _bindingResult.GetBoundNode(commonTableExpression) as BoundCommonTableExpression;
            return result == null ? null : result.TableSymbol;
        }

        public TableInstanceSymbol GetDeclaredSymbol(TableReferenceSyntax tableReference)
        {
            var result = _bindingResult.GetBoundNode(tableReference) as BoundNamedTableReference;
            return result == null ? null : result.TableInstance;
        }

        public Symbol GetDeclaredSymbol(DerivedTableReferenceSyntax tableReference)
        {
            var result = _bindingResult.GetBoundNode(tableReference) as BoundDerivedTableReference;
            return result == null ? null : result.TableInstance;
        }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            return _bindingResult.Diagnostics;
        }

        public IEnumerable<Symbol> LookupSymbols(int position)
        {
            // TODO: I think we should'nt associate the binding context with a node but instead the whole binder.
            // TODO: Once this is done, our Lookup* methods should simply call the Binder ones.
            var node = FindClosestNodeWithBindingContext(_bindingResult.Root, position, 0);
            var bindingContext = node == null ? null : _bindingResult.GetBindingContext(node);
            var symbols = bindingContext != null ? bindingContext.LookupSymbols() : Enumerable.Empty<Symbol>();
            foreach (var symbol in symbols)
            {
                yield return symbol;

                var tableInstance = symbol as TableInstanceSymbol;
                if (tableInstance != null)
                {
                    foreach (var columnInstance in tableInstance.ColumnInstances)
                        yield return columnInstance;
                }
            }
        }

        private SyntaxNode FindClosestNodeWithBindingContext(SyntaxNode root, int position, int lastPosition)
        {
            foreach (var nodeOrToken in root.ChildNodesAndTokens())
            {
                if (lastPosition <= position && position < nodeOrToken.Span.End)
                {
                    if (nodeOrToken.IsToken)
                        return null;

                    var node = nodeOrToken.AsNode();
                    var result = FindClosestNodeWithBindingContext(node, position, lastPosition);
                    if (result != null)
                        return result;

                    if (_bindingResult.GetBindingContext(node) != null)
                        return node;
                }

                lastPosition = nodeOrToken.Span.End;
            }

            return null;
        }

        public IEnumerable<MethodSymbol> LookupMethods(Type type)
        {
            // TODO: Should we cache them to ensure object identity for method symbols?
            var dataContext = _compilation.DataContext;
            var methodProvider = dataContext.MethodProviders.LookupValue(type);
            return methodProvider == null
                       ? Enumerable.Empty<MethodSymbol>()
                       : methodProvider.GetMethods(type);
        }

        public IEnumerable<PropertySymbol> LookupProperties(Type type)
        {
            // TODO: Should we cache them to ensure object identity for property symbols?
            var dataContext = _compilation.DataContext;
            var propertyProvider = dataContext.PropertyProviders.LookupValue(type);
            return propertyProvider == null
                       ? Enumerable.Empty<PropertySymbol>()
                       : propertyProvider.GetProperties(type);
        }
    }
}