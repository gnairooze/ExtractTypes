using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractTypes.Business.Models
{
    public class ExtractedType
    {
        public ExtractedType()
        {
            this.Items = new List<ExtractedItem>();
        }

        public string TypeFullName { get; set; }
        public string AssemblyFullName { get; set; }
        public List<ExtractedItem> Items { get; private set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine($"AssemblyFullName: {AssemblyFullName}");
            bld.AppendLine($"TypeFullName: {TypeFullName}");

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                bld.AppendLine($"Property {i + 1} of {Items.Count}");
                bld.AppendLine(item.ToString());
            }
            return bld.ToString();
        }
    }
}
