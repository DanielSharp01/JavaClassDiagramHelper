using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Method : Qualified
    {
        public TypeReference ReturnType { get; set; }
        public List<Parameter> Parameters { get; } = new List<Parameter>();
        public List<TypeReference> Throws { get; } = new List<TypeReference>();
        public List<string> BodyIdentifiers { get; } = new List<string>();

        public Method(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += (ReturnType != null ? ReturnType.ToString() + " " : "") + Name + "(";

            if (Parameters.Count > 0)
            {
                foreach (Parameter parameter in Parameters)
                {
                    ret += parameter.ToString() + ", ";
                }
                ret = ret.Substring(0, ret.Length - 2);
            }

            return ret + ")";
        }

        public override string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Name.ToUMLString());
            stringBuilder.Append("(");

            foreach (Parameter parameter in Parameters)
            {
                stringBuilder.Append(parameter.Name + ": " + parameter.Type.ToUMLString() + ", ");
            }

            if (Parameters.Count > 0) stringBuilder.Length -= 2;
            stringBuilder.Append(")");
            if (ReturnType != null && ReturnType.Name != "void")
            {
                stringBuilder.Append(": " + ReturnType.ToUMLString());
            }

            return WrapWithUmlQualifiers(stringBuilder.ToString());
        }
    }
}
