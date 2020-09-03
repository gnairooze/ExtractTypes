using ExtractTypes.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Extractor
{
    class Program
    {
        static List<string> EnhancedArgs { get; set; }
        static string AssemblyPath { get; set; }
        static string TypeFullName { get; set; }
        static bool HelpMode { get; set; }
        static void Main(string[] args)
        {
            EnhancedArgs = args.ToList();

            Start();
        }

        private static void Start()
        {
            SetArguments();
            RunReal();
        }

        private static void SetArguments()
        {
            string arg = "--help";
            if (EnhancedArgs.Count == 0 || EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                HelpMode = true;
                ShowHelp();
                return;
            }

            arg = "--assembly-Path";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                AssemblyPath = value.Substring(arg.Length);
            }

            arg = "--type-fullname";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                TypeFullName = value.Substring(arg.Length);
            }

            if(String.IsNullOrWhiteSpace(AssemblyPath) || String.IsNullOrWhiteSpace(TypeFullName))
            {
                HelpMode = true;
                ShowHelp();
                return;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("==================================");
            Console.WriteLine("Export Type Properties to CSV file");
            Console.WriteLine("==================================");
            Console.WriteLine("--help\tshow this help. This help also shows when no arguments added");
            Console.WriteLine();
            Console.WriteLine("--assembly-Path\tset the assembly path that contains the type");
            Console.WriteLine();
            Console.WriteLine("--type-fullname\tset the type full name to be exported");
            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("Extractor.exe --assembly-Path\"c:\\temp\\assembly.dll\" --type-fullname\"Namespace.ClassName\"");
            Console.WriteLine();
            Console.WriteLine("==================================");
        }

        private static void RunReal()
        {
            if (HelpMode)
            {
                return;
            }

            string resultFileParentPath = @"result";
            if (!Directory.Exists(resultFileParentPath))
            {
                Directory.CreateDirectory(resultFileParentPath);
            }

            Core core = new Core();
            var extractedType = core.ExtractType(AssemblyPath, TypeFullName);

            FileManagerCore.Controller fileController = new FileManagerCore.Controller();
            string columns = "ID,PathName,Name,Type,IsNullable";
            string resultFilePath = Path.Combine(resultFileParentPath, $"{extractedType.TypeName}.csv");
            fileController.SaveFile(false, resultFilePath, columns);

            for (int i = 0; i < extractedType.Fields.Count; i++)
            {
                var item = extractedType.Fields[i];
                fileController.SaveFile(true, resultFilePath, $"{item.ID},{item.PathName},{item.Name},{item.Type},{item.IsNullable}");
            }
        }
    }
}
