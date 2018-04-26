using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace LogAdvicer
{
    public class Analizer
    {
        MethodMetrics metrics;
        bool catchClauselogged;
        int trynumber;
        public Analizer()
        {
            metrics = new MethodMetrics();
            trynumber = 0;
        }
        public MethodMetrics AnalyzeMethod(MethodDeclarationSyntax method)
        {
            var body = method.Body;
            foreach (var child in body.ChildNodes())
            {
                switch (child.Kind()) {
                    case SyntaxKind.IfStatement:
                        IfStatement((IfStatementSyntax)child);
                        break;
                    case SyntaxKind.TryStatement:
                        TryStatement((TryStatementSyntax)child);
                        break;
                }
            }
            return metrics;
        }
        public void IfStatement(IfStatementSyntax node)
        {
            metrics.SetHasIf(true);
            var block = node.ChildNodes();
            var condition = node.Condition;
            if (condition.ToString().Contains("null"))
            {
                metrics.AddIfValues("NullValueCondition");
            }
            foreach (var statement in block)
            {
                if (statement.IsKind(SyntaxKind.TryStatement))
                {
                    TryStatement((TryStatementSyntax)statement);
                }
                else
                {
                    AnalyzeKindIf(statement.Kind());
                }
            }
            if (node.Else != null)
            {
                metrics.AddIfValues("HasElse");
                ElseClause(node.Else);
            }
        }
        public void ElseClause(ElseClauseSyntax node)
        {
            var block = node.ChildNodes();
            foreach (var statement in block)
            {
                AnalyzeKindElse(statement.Kind());
            }
        }
        public void TryStatement(TryStatementSyntax node)
        {
            var block = node.Block.ChildNodes();
            trynumber++;
            metrics.SetHasTry(true);
            foreach (var statement in block)
            {
                AnalyzeKindTry(statement.Kind(), statement);
            }
            foreach (var catchClause in node.Catches)
            {
                metrics.AddTryValues("NumberCatchClauses");
                CatchClause(catchClause);
            }
        }
        public void CatchClause(CatchClauseSyntax node)
        {
            var block = node.Block.ChildNodes();
            var declarationchild = node.Declaration.ChildNodes();
            catchClauselogged = false;
            foreach (var statement in block)
            {
                AnalyzeKindCatch(statement.Kind());
            }
            foreach (var child in declarationchild) // captures the exception declarations
            {
                metrics.AddCatchValues(child.ToString(), catchClauselogged);
            }
        }

        private void AnalyzeKindCatch(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement: // return statement must be the last statement 
                    metrics.AddCatchValues("ReturnCatch", catchClauselogged);
                    break;
                case SyntaxKind.ThrowStatement: // throw statement must be the last statement
                    metrics.AddCatchValues("ThrowCacth", catchClauselogged);
                    break;
            }
        }

        private void AnalyzeKindTry(SyntaxKind Kind, SyntaxNode statement)
        {

            switch (Kind)
            {
                case SyntaxKind.VariableDeclaration:
                    metrics.AddTryValues("VariableDeclaration" + trynumber.ToString());
                    break;
                case SyntaxKind.ReturnStatement:
                    metrics.AddTryValues("ReturnStatement" + trynumber.ToString(), 1);
                    break;
                case SyntaxKind.ThrowStatement:
                    metrics.AddTryValues("ThrowStatement" + trynumber.ToString(), 1);
                    break;
                case SyntaxKind.IfStatement:
                    IfStatement((IfStatementSyntax)statement);
                    metrics.AddTryValues("IfStatements" + trynumber.ToString());
                    break;
                case SyntaxKind.InvocationExpression:
                    metrics.AddTryValues("ExpresionCount" + trynumber.ToString());
                    if (statement.ToString().Contains("Sleep"))
                    {
                        metrics.AddTryValues("SleepInTry" + trynumber.ToString(), 1);
                    }
                    break;
            }
        }
        private void AnalyzeKindIf(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    metrics.AddIfValues("ReturnStatement");
                    break;
                case SyntaxKind.ThrowExpression:
                    metrics.AddIfValues("ThrowsStatement");
                    break;
            }
        }
        private void AnalyzeKindElse(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    metrics.AddElseValues("ReturnElse");
                    break;
                case SyntaxKind.ThrowStatement:
                    metrics.AddElseValues("ThrowElse");
                    break;
            }
        }
    }
}
