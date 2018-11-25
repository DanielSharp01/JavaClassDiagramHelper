using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper.UMLet
{
    public class ClassifierGraph
    {
        public List<ClassifierNode> Nodes { get; } = new List<ClassifierNode>();

        public ClassifierGraph(List<Classifier> classifiers)
        {
            foreach (Classifier classifier in classifiers)
            {
                Nodes.Add(new ClassifierNode { Classifier = classifier });
            }
        }

        public void SetRelationsFor(ClassifierNode classifierNode)
        {
            List<string> dependencyCandidates = new List<string>();

            if (classifierNode.Classifier is Class classObj)
            {
                // Extends
                if (classObj.Extends != null)
                {
                    ClassifierNode node = Nodes.FirstOrDefault(n => n.Classifier.Name.Name == classObj.Extends.Name);
                    if (node != null)
                    {
                        classifierNode.Relations.Add(new Extends { Related = node });
                    }
                    dependencyCandidates.AddRange(GetInternalTypeReferences(classObj.Extends));
                }

                // Implements
                foreach (TypeReference implemented in classObj.Implements)
                {
                    ClassifierNode node = Nodes.FirstOrDefault(n => n.Classifier.Name.Name == implemented.Name);
                    if (node != null)
                    {
                        classifierNode.Relations.Add(new Implements { Related = node });
                    }
                    dependencyCandidates.AddRange(GetInternalTypeReferences(implemented));
                }

                classObj.ExcludedUMLFields.Clear();
                // Fields
                foreach (Field field in classObj.Fields)
                {
                    ClassifierNode node = Nodes.FirstOrDefault(n => n.Classifier.Name.Name == field.Type.Name);
                    if (node != null)
                    {
                        classifierNode.Relations.Add(new Association { Related = node, Field = field.Name.Name, Multiplicity = field.Type.IsArray ? "0..*" : "1" });
                        classObj.ExcludedUMLFields.Add(field);
                    }
                    dependencyCandidates.AddRange(GetInternalTypeReferences(field.Type));
                }

                // Methods
                foreach (Method method in classObj.Methods)
                {
                    if (method.Name != null)
                    {
                        if (method.ReturnType != null)
                        {
                            dependencyCandidates.Add(method.ReturnType.Name);
                            dependencyCandidates.AddRange(GetInternalTypeReferences(method.ReturnType));
                        }
                        foreach (Parameter parameter in method.Parameters)
                        {
                            dependencyCandidates.Add(parameter.Type.Name);
                            dependencyCandidates.AddRange(GetInternalTypeReferences(parameter.Type));
                        }
                    }
                    dependencyCandidates.AddRange(method.BodyIdentifiers);
                }
            }
            else if (classifierNode.Classifier is Interface interfaceObj)
            {
                // Extends
                if (interfaceObj.Extends != null)
                {
                    ClassifierNode node = Nodes.FirstOrDefault(n => n.Classifier.Name.Name == interfaceObj.Extends.Name);
                    if (node != null)
                    {
                        classifierNode.Relations.Add(new Extends { Related = node });
                    }
                    dependencyCandidates.AddRange(GetInternalTypeReferences(interfaceObj.Extends));
                }
  
                // Methods
                foreach (Method method in interfaceObj.Methods)
                {
                    dependencyCandidates.Add(method.ReturnType.Name);
                    dependencyCandidates.AddRange(GetInternalTypeReferences(method.ReturnType));
                    foreach (Parameter parameter in method.Parameters)
                    {
                        dependencyCandidates.Add(parameter.Type.Name);
                        dependencyCandidates.AddRange(GetInternalTypeReferences(parameter.Type));
                    }
                }
            }

            foreach (string dependencyCandidate in dependencyCandidates.Distinct())
            {
                ClassifierNode node = Nodes.FirstOrDefault(n => n.Classifier.Name.Name == dependencyCandidate);
                if (node != null && node != classifierNode)
                {
                    classifierNode.Relations.Add(new Dependency { Related = node });
                }
            }
        }

        private List<string> GetInternalTypeReferences(TypeReference type)
        {
            List<string> ret = new List<string>();
            foreach (TypeReference genType in type.GenericTypes)
            {
                if (genType.Name != "?")
                {
                    ret.Add(genType.Name);
                }

                if (genType.Extends != null)
                {
                    ret.Add(genType.Extends);
                }

                ret.AddRange(GetInternalTypeReferences(genType));
            }

            return ret;
        }
    }
}
