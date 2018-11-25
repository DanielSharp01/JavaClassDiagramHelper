using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper.UMLet
{
    public class ClassifierNode
    {
        public Classifier Classifier { get; set; }
        public string Text { get; set; }
        public List<ClassifierRelation> Relations { get; } = new List<ClassifierRelation>();
    }
}
