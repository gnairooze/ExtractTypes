using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractTypes.Business.Models
{
    public class ExtractedType
    {
        public ExtractedType()
        {
            this.Fields = new List<ExtractedItem>();
        }

        public string TypeFullName { get; set; }
        public string TypeName { get; set; }
        public string AssemblyFullName { get; set; }
        public List<ExtractedItem> Fields { get; private set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine($"AssemblyFullName: {AssemblyFullName}");
            bld.AppendLine($"TypeFullName: {TypeFullName}");

            for (int i = 0; i < Fields.Count; i++)
            {
                var item = Fields[i];
                bld.AppendLine($"Property {i + 1} of {Fields.Count}");
                bld.AppendLine(item.ToString());
            }
            return bld.ToString();
        }
    }
}
