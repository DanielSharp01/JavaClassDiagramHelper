using JavaClassDiagramHelper.UMLet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Class : Classifier
    {
        public List<Classifier> InternalClassifiers { get; } = new List<Classifier>();
        public List<Method> Methods { get; } = new List<Method>();
        public List<Field> Fields { get; } = new List<Field>();
        public TypeReference Extends { get; set; }
        public List<TypeReference> Implements { get; } = new List<TypeReference>();

        public Class(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += "class " + Name.ToString();

            if (Extends != null)
            {
                ret += " extends " + Extends.ToString();
            }

            if (Implements.Count > 0)
            {
                ret += " implements ";
                foreach (TypeReference type in Implements)
                {
                    ret += type.ToString() + ", ";
                }
                ret = ret.Substring(0, ret.Length - 2);
            }

            return ret;
        }

        public List<Field> ExcludedUMLFields { get; } = new List<Field>();

        public override string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Name.ToUMLString());
            stringBuilder.AppendLine("--");

            List<Method> excludedMethods = new List<Method>();
            foreach (Field field in Fields.Except(ExcludedUMLFields))
            {
                bool getter = false, setter = false;
                Method method;
                if ((method = Methods.Where(m => m.Name != null).FirstOrDefault(m => m.Name.Name == "get" + Parser.UpperFirstChar(field.Name.Name))) != null)
                {
                    excludedMethods.Add(method);
                    getter = true;
                }
                if ((method = Methods.Where(m => m.Name != null).FirstOrDefault(m => m.Name.Name == "set" + Parser.UpperFirstChar(field.Name.Name))) != null)
                {
                    excludedMethods.Add(method);
                    setter = true;
                }

                if (getter && setter)
                {
                    stringBuilder.Append("<<get/set>> ");
                }
                else if (getter)
                {
                    stringBuilder.Append("<<get>> ");
                }
                else if (setter)
                {
                    stringBuilder.Append("<<set>> ");
                }

                stringBuilder.AppendLine(field.ToUMLString());
            }
            stringBuilder.AppendLine("--");
            foreach (Method method in Methods.Except(excludedMethods))
            {
                if (method.Name != null && method.Annotations.All(a => a.Name != "Override"))
                {
                    stringBuilder.AppendLine(method.ToUMLString());
                }
            }
            stringBuilder.Append("bg=yellow");
            return stringBuilder.ToString();
        }
    }
}
