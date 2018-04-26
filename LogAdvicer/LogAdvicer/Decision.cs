using System;
using System.Collections.Generic;
using System.Text;
using Accord.MachineLearning.DecisionTrees;
using Accord.Statistics.Filters;
using Accord.IO;
using Accord.Math;
using System.IO;
using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace LogAdvicer
{
    public class Decision
    {
        DecisionTree tree;
        string rulePath = "Rules.xml";
        string configurationPath = "Config.txt";
        string columnsPath = "columns.txt";
        Configuration configuration;
        FileReader reader;
        string[] columnNames;
        DataTable data;
        Codification codebook;
        List<string> inputs;
        public Decision()
        {
            if (File.Exists(rulePath))
            {
                Serializer.Load(rulePath, out tree);
            }
            if (File.Exists(configurationPath))
            {
                configuration = new Configuration();
                configuration.LoadConfigurations(configurationPath);
            }
            if (File.Exists(columnsPath))
            {
                reader = new FileReader(columnsPath);
                columnNames = reader.readlAll();
                reader.Close();
            }
            data = new DataTable();
            foreach (var columnname in columnNames)
            {
                data.Columns.Add(columnname);
            }
            codebook = new Codification(data);
            inputs = new List<string>();
        }
        public bool ApplyRule(MethodMetrics metrics, MethodDeclarationSyntax method)
        {
            bool methodLogged = false;
            if(configuration != null)
            {
                foreach(var statement in method.Body.ChildNodes())
                {
                    if (configuration.IsLogStatement(statement))
                    {
                        methodLogged = true;
                    }
                }
                if (!methodLogged)
                {
                    int[] query =codebook.Transform(TranslateInputs(metrics), columnNames);
                    int output = tree.Decide(query);
                    if(output == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public DataRow TranslateInputs(MethodMetrics metrics)
        {
            DataRow row = data.NewRow();
            row["HasTry"] = metrics.MethodHasTry();
            row["HasIf"] = metrics.MethodHasIf();
            bool found;
            foreach (var column in columnNames) // search in dictionaries
            {
                found = false;
                foreach (var key in metrics.GetIfKeys())
                {
                    if (column.Equals(key))
                    {
                        row[key] = metrics.GetIfValue(key);
                        found = true;
                    }
                }
                if (!found)
                {
                    foreach (var key in metrics.GetTryKeys())
                    {
                        if (column.Equals(key))
                        {
                            row[key] = metrics.GetTryValue(key);
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    foreach (var key in metrics.GetElseKeys())
                    {
                        if (column.Equals(key))
                        {
                            row[key] = metrics.GetElseValue(key);
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    foreach (var key in metrics.GetCatchKeys())
                    {
                        if (column.Equals(key))
                        {
                            row[key] = metrics.GetCatchValue(key);
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    if (row[column] == null)
                    {
                        row[column] = 0;// complete the table
                    }
                }
            }
            return row;
        }
    }
}
