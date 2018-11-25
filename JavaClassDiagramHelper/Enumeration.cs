using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Enumeration : Classifier
    {
        public List<string> Values { get; } = new List<string>();

        public Enumeration(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += "enum " + Name.ToString();

            return ret;
        }

        public override string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<<Enumeration>>");
            stringBuilder.AppendLine(Name.ToUMLString());
            stringBuilder.AppendLine("--");
            foreach (string value in Values)
            {
                stringBuilder.AppendLine(value);
            }
            stringBuilder.Append("bg=yellow");
            return stringBuilder.ToString();
        }
    }
}
