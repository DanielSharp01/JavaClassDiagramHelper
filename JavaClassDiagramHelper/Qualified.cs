using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public abstract class Qualified
    {
        public List<Annotation> Annotations { get; }
        public List<Qualifier> Qualifiers { get; }
        public TypeReference Name { get; set; }

        public Qualified(List<Annotation> annotations, List<Qualifier> qualifiers)
        {
            Annotations = annotations;
            Qualifiers = qualifiers;
        }

        public string WrapWithUmlQualifiers(string descriptor)
        {
            if (Qualifiers.Contains(Qualifier.Private))
            {
                descriptor = "-" + descriptor;
            }
            else if (Qualifiers.Contains(Qualifier.Protected))
            {
                descriptor = "#" + descriptor;
            }
            else if (Qualifiers.Contains(Qualifier.Public))
            {
                descriptor = "+" + descriptor;
            }
            else
            {
                descriptor = "~" + descriptor;
            }

            if (Qualifiers.Contains(Qualifier.Abstract)) descriptor = "/" + descriptor + "/";
            if (Qualifiers.Contains(Qualifier.Static)) descriptor = "_" + descriptor + "_";

            return descriptor;
        }

        public abstract string ToUMLString();
    }
}
