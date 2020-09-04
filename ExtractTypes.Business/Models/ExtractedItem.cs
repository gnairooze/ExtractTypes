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
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? SizeInBytes { get; set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.Append($"ID: {ID}");
            bld.Append($" | PathName: {PathName}");
            bld.Append($" | Name: {Name}");
            bld.Append($" | Type: {Type}");
            bld.Append($" | IsNullable: {IsNullable}");
            bld.Append($" | IsRequired: {IsRequired}");
            bld.Append($" | MinLength: {MinLength}");
            bld.Append($" | MaxLength: {MaxLength}");
            bld.Append($" | SizeInBytes: {SizeInBytes}");
            bld.AppendLine();
            return bld.ToString();
        }
    }
}
