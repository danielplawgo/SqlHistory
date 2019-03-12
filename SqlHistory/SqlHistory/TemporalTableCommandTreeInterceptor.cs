using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    internal class TemporalTableCommandTreeInterceptor : IDbCommandTreeInterceptor
    {
        private static readonly List<string> _namesToIgnore = new List<string> { "ValidFrom", "ValidTo" };

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var insertCommand = interceptionContext.Result as DbInsertCommandTree;
                if (insertCommand != null)
                {
                    var newCommand = HandleInsertCommand(insertCommand);

                    interceptionContext.Result = newCommand;
                }

                var updateCommand = interceptionContext.Result as DbUpdateCommandTree;
                if (updateCommand != null)
                {
                    var newCommand = HandleUpdateCommand(updateCommand);

                    interceptionContext.Result = newCommand;
                }

                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    interceptionContext.Result = HandleQueryCommand(queryCommand);
                }

                var deleteCommand = interceptionContext.Result as DbDeleteCommandTree;
                if (deleteCommand != null)
                {
                    interceptionContext.Result = HandleDeleteCommand(deleteCommand);
                }
            }
        }

        private static DbUpdateCommandTree HandleUpdateCommand(DbUpdateCommandTree updateCommand)
        {
            var newSetClauses = GenerateSetClauses(updateCommand.SetClauses);

            var newCommand = new DbUpdateCommandTree(
                updateCommand.MetadataWorkspace,
                updateCommand.DataSpace,
                updateCommand.Target,
                updateCommand.Predicate,
                newSetClauses,
                updateCommand.Returning);
            return newCommand;
        }

        private static DbInsertCommandTree HandleInsertCommand(DbInsertCommandTree insertCommand)
        {
            var newSetClauses = GenerateSetClauses(insertCommand.SetClauses);

            var newCommand = new DbInsertCommandTree(
                insertCommand.MetadataWorkspace,
                insertCommand.DataSpace,
                insertCommand.Target,
                newSetClauses,
                insertCommand.Returning);

            return newCommand;
        }

        private static ReadOnlyCollection<DbModificationClause> GenerateSetClauses(IList<DbModificationClause> modificationClauses)
        {
            var props = new List<DbModificationClause>(modificationClauses);
            props = props.Where(_ => IgnoreProperty(_) == false).ToList();

            var newSetClauses = new ReadOnlyCollection<DbModificationClause>(props);
            return newSetClauses;
        }

        private static bool IgnoreProperty(DbModificationClause clause)
        {
            string propertyName = (((clause as DbSetClause)?.Property as DbPropertyExpression)?.Property as EdmProperty)
                ?.Name;

            if (propertyName == null)
            {
                return false;
            }

            return _namesToIgnore.Any(n => propertyName.Contains(n));
        }

        private static DbCommandTree HandleQueryCommand(DbQueryCommandTree queryCommand)
        {
            var newQuery = queryCommand.Query.Accept(new QueryVisitor());
            return new DbQueryCommandTree(
                queryCommand.MetadataWorkspace,
                queryCommand.DataSpace,
                newQuery);
        }

        private static DbCommandTree HandleDeleteCommand(DbDeleteCommandTree deleteCommand)
        {
            var predicate = deleteCommand.Predicate.Accept(new DeleteVisitor());

            return new DbDeleteCommandTree(deleteCommand.MetadataWorkspace,
                deleteCommand.DataSpace,
                deleteCommand.Target,
                predicate);
        }

        private class DeleteVisitor : DefaultExpressionVisitor
        {
            public override DbExpression Visit(DbAndExpression expression)
            {
                var visitor = new TemporalTableVisitor();

                visitor.ProcessExpression(expression);

                if (visitor.IsTemporalExpression)
                {
                    return visitor.Expression;
                }

                return base.Visit(expression);
            }
        }

        private class QueryVisitor : DefaultExpressionVisitor
        {
            public override DbExpression Visit(DbJoinExpression expression)
            {
                var visitor = new TemporalTableVisitor();

                visitor.ProcessExpression(expression.JoinCondition);

                if (visitor.IsTemporalExpression)
                {
                    return DbExpressionBuilder.InnerJoin(expression.Left, expression.Right, visitor.Expression);
                }

               return base.Visit(expression);
            }
        }

        private class TemporalTableVisitor
        {
            private List<DbExpression> _expressions = new List<DbExpression>();

            public DbExpression Expression
            {
                get
                {
                    if (_expressions.Count == 0)
                    {
                        return null;
                    }

                    if (_expressions.Count == 1)
                    {
                        return _expressions[0];
                    }

                    return DbExpressionBuilder.And(_expressions[0], _expressions[1]);
                }
            }

            private bool _isTemporalExpression = false;

            public bool IsTemporalExpression
            {
                get { return _isTemporalExpression && Expression != null; }
            }

            public void ProcessExpression(DbExpression expression)
            {
                if (expression.ExpressionKind == DbExpressionKind.And)
                {
                    var endExpression = expression as DbAndExpression;
                    ProcessExpression(endExpression.Left);
                    ProcessExpression(endExpression.Right);
                }

                if (expression.ExpressionKind == DbExpressionKind.Equals)
                {
                    bool temporalComparision = false;

                    var equalExpression = expression as DbComparisonExpression;

                    var left = equalExpression.Left as DbPropertyExpression;

                    if (left != null)
                    {
                        temporalComparision = _namesToIgnore.Any(n => left.Property.Name.Contains(n));
                    }

                    var right = equalExpression.Right as DbPropertyExpression;

                    if (right != null)
                    {
                        temporalComparision = temporalComparision | _namesToIgnore.Any(n => right.Property.Name.Contains(n));
                    }

                    if (temporalComparision == false)
                    {
                        _expressions.Add(expression);
                    }
                    else
                    {
                        _isTemporalExpression = true;
                    }
                }
            }
        }
    }
}
