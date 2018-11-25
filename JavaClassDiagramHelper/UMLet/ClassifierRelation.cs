using System; 
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper.UMLet
{
    public class ClassifierRelation
    {
        public ClassifierNode Related { get; set; }
    }

    public class Association : ClassifierRelation
    {
        public string Field { get; set; }
        public string Multiplicity { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Association Towards:" + Related.Classifier.Name.ToString());
            stringBuilder.AppendLine("->");
            stringBuilder.AppendLine("m1=" + Field);
            stringBuilder.AppendLine("r1=" + Multiplicity);
            return stringBuilder.ToString();
        }
    }

    public class Dependency : ClassifierRelation
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Dependency Towards:" + Related.Classifier.Name.ToString());
            stringBuilder.AppendLine(".>");
            return stringBuilder.ToString();
        }
    }

    public class Extends : ClassifierRelation
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Extends Towards:" + Related.Classifier.Name.ToString());
            stringBuilder.AppendLine("->>");
            return stringBuilder.ToString();
        }
    }

    public class Implements : ClassifierRelation
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Implements Towards:" + Related.Classifier.Name.ToString());
            stringBuilder.AppendLine(".>>");
            return stringBuilder.ToString();
        }
    }
}
