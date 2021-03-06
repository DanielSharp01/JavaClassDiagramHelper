﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Interface : Classifier
    {
        public List<Method> Methods { get; } = new List<Method>();
        public TypeReference Extends { get; set; }

        public Interface(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += "interface " + Name.ToString();

            if (Extends != null)
            {
                ret += " extends " + Extends.ToString();
            }

            return ret;
        }

        public override string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<<Interface>>");
            stringBuilder.AppendLine(Name.ToUMLString());
            stringBuilder.AppendLine("--");
            foreach (Method method in Methods)
            {
                if (!method.Qualifiers.Contains(Qualifier.Public)) method.Qualifiers.Add(Qualifier.Public);
                stringBuilder.AppendLine(method.ToUMLString());
            }
            stringBuilder.Append("bg=yellow");
            return stringBuilder.ToString();
        }
    }
}
