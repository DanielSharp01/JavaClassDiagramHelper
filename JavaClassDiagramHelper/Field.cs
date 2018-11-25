using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Field : Qualified
    {
        public TypeReference Type { get; set; }

        public Field(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += Type.ToString() + " " + Name;

            return ret;
        }

        public override string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Name.ToUMLString());
            stringBuilder.Append(": " + Type.ToUMLString());
            return WrapWithUmlQualifiers(stringBuilder.ToString());
        }
    }
}
