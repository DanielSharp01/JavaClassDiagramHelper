using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Method
    {
        public Annotation Annotation { get; set; }
        public string Name { get; set; }
        public TypeReference ReturnType { get; set; }
        public List<Parameter> Parameters { get; } = new List<Parameter>();
        public List<Qualifier> Qualifiers { get; } = new List<Qualifier>();

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
    }
}
