using JavaClassDiagramHelper.UMLet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper
{
    class Program
    {
        static readonly List<Classifier> classifiers = new List<Classifier>();
        static string sourceDirectory = "C:\\Users\\DanielSharp\\source\\repos\\Pockets\\src\\";

        static void Main(string[] args)
        {
            ProcessDirectory(sourceDirectory);
            ClassifierGraph graph = new ClassifierGraph(classifiers);

            string cmd;
            while ((cmd = Console.ReadLine()) != "quit")
            {
                Console.Clear();
                ClassifierNode classifierNode = graph.Nodes.Where(n => n.Classifier.Name.Name == cmd).FirstOrDefault();
                if (classifierNode != null)
                {
                    classifierNode.Relations.Clear();
                    graph.SetRelationsFor(classifierNode);
                    classifierNode.Text = classifierNode.Classifier.ToUMLString();
                    Console.WriteLine(classifierNode.Text);

                    foreach (ClassifierRelation relation in classifierNode.Relations)
                    {
                        Console.WriteLine();
                        Console.WriteLine(relation);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No such classifier!");
                    Console.ResetColor();
                }
            }

            Console.ReadLine();
        }

        static void ProcessDirectory(string directory)
        {
            if (directory.Contains("test")) return;

            foreach (string file in Directory.GetFiles(directory))
            {
                if (Path.GetExtension(file) != ".java") continue;
                
                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    Classifier parsed = new Parser(new Tokenizer(reader.ReadToEnd()).Tokenize()).Parse();
                    classifiers.Add(parsed);
                    if (parsed is Class classObj)
                    {
                        foreach (Classifier internalClassifier in classObj.InternalClassifiers)
                        {
                            internalClassifier.Name.Name = classObj.Name.Name + "." + internalClassifier.Name.Name;
                            classifiers.Add(internalClassifier);
                        }
                    }
                }
            }

            foreach (string dir in Directory.GetDirectories(directory))
            {
                ProcessDirectory(dir);
            }
        }
    }
}
