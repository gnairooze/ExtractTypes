using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractTypes.Business.Models
{
    public class ExtractedItem
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine($"ID: {ID}");
            bld.AppendLine($"FullName: {FullName}");
            bld.AppendLine($"Name: {Name}");
            bld.AppendLine($"Type: {Type}");
            return bld.ToString();
        }
    }
}
