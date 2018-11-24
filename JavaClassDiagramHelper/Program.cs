using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper
{
    class Program
    {
        static readonly List<Package> packages = new List<Package>();
        static string sourceDirectory = "C:\\Users\\DanielSharp\\source\\repos\\Pockets\\src\\";

        static Package currentPackage;

        static void Main(string[] args)
        {
            ProcessDirectory(sourceDirectory);
            currentPackage = packages.First();

            string cmd;
            while ((cmd = Console.ReadLine()) != "quit")
            {
                if (cmd.Length > 7 && cmd.Substring(0, 7).ToLower() == "package")
                {
                    Package(cmd.Substring(8));
                }
                else if (cmd.Length > 5 && cmd.Substring(0, 5).ToLower() == "class")
                {
                    Class(cmd.Substring(6));
                }
                else if (cmd.Length >= 4 && cmd.Substring(0, 4).ToLower() == "list")
                {
                    List();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No such command!");
                    Console.ResetColor();
                }
            }

            Console.ReadLine();
        }

        static void Package(string package)
        {
            currentPackage = packages.Where(p => p.Name == package).FirstOrDefault();
            if (currentPackage == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No such package!");
                Console.ResetColor();
            }
        }

        static void Class(string className)
        {
            if (currentPackage == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No package selected!");
                Console.ResetColor();
                return;
            }

            Class classObj = currentPackage.Classes.Where(c => c.Name.Name == className).FirstOrDefault();

            if (classObj == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No such class!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine(classObj.ToString());
            Console.WriteLine("\nFields:");
            foreach (Field field in classObj.Fields)
            {
                Console.WriteLine(field.ToString());
            }
            Console.WriteLine("\nMethods:");
            foreach (Method method in classObj.Methods)
            {
                Console.WriteLine(method.ToString());
            }
        }

        static void List()
        {
            Console.WriteLine("Interfaces:");
            foreach (Interface interfaceObj in currentPackage.Interfaces)
            {
                Console.WriteLine(interfaceObj.ToString());
            }
            Console.WriteLine("\nClasses:");
            foreach (Class classObj in currentPackage.Classes)
            {
                Console.WriteLine(classObj.ToString());
            }
        }

        static void ProcessDirectory(string directory)
        {
            if (directory.Contains("test")) return;

            string packageName = directory.Replace(sourceDirectory, "").Replace("\\", ".").Replace("/", ".");
            if (packageName == "") packageName = null;
            Package package = new Package() { Name = packageName ?? "<default>" };
            packages.Add(package);

            foreach (string file in Directory.GetFiles(directory))
            {
                if (Path.GetExtension(file) != ".java") continue;
                
                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    object parsed = new Parser(new Tokenizer(reader.ReadToEnd()).Tokenize()).Parse();
                    if (parsed is Class classObj)
                    {
                        package.Classes.Add(classObj);
                        foreach (Class internalClass in classObj.InternalClasses)
                        {
                            internalClass.Name.Name = classObj.Name.Name + "." + internalClass.Name.Name;
                            package.Classes.Add(internalClass);
                        }
                    }
                    else if (parsed is Interface interfaceObj)
                    {
                        package.Interfaces.Add(interfaceObj);
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
