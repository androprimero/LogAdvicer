using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace LogAdvicer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LogAdvicerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LogAdvicer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString TitleMethod = new LocalizableResourceString(nameof(Resources.AnalyzerMethodTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString DescriptionMethod = new LocalizableResourceString(nameof(Resources.AnalyzerMethodTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MethodFormat = new LocalizableResourceString(nameof(Resources.MethodFormat), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";
        private const string CategoryMethod = "Logging";
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static DiagnosticDescriptor MethodLoged = new DiagnosticDescriptor(DiagnosticId, TitleMethod, MethodFormat, CategoryMethod, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionMethod);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule,MethodLoged); } }
        private static Decision decision;
        private static Analizer analizer;
        public override void Initialize(AnalysisContext context)
        {
            decision = new Decision();
            analizer = new Analizer();
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;
            MethodMetrics metric = analizer.AnalyzeMethod(method);
            if (decision.ApplyRule(metric, method))
            {
                var diagnostic = Diagnostic.Create(MethodLoged,method.GetLocation(), method.Identifier.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
