using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractTypes.Business.Models
{
    public class ExtractedItem
    {
        public ExtractedItem()
        {
            this.Parents = new List<string>();
        }

        public int ID { get; set; }
        public string PathName { get; set; }
        public List<string> Parents { get; private set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine($"ID: {ID}");
            bld.AppendLine($"PathName: {PathName}");
            bld.AppendLine($"Name: {Name}");
            bld.AppendLine($"Type: {Type}");
            return bld.ToString();
        }
    }
}
