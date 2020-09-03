using ExtractTypes.Business;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Extractor
{
    class Test
    {
        public void sample()
        {
            Assembly SampleAssembly;
            SampleAssembly = Assembly.LoadFrom(@"c:\temp\8\GFZ.NotifyByMail.Infra.dll");
            // Obtain a reference to a method known to exist in assembly.
            //MethodInfo Method = SampleAssembly.GetTypes()[0].GetMethod("Method1");
            // Obtain a reference to the parameters collection of the MethodInfo instance.
            //ParameterInfo[] Params = Method.GetParameters();
            // Display information about method parameters.
            // Param = sParam1
            //   Type = System.String
            //   Position = 0
            //   Optional=False
            //foreach (ParameterInfo Param in Params)
            //{
            //    Console.WriteLine("Param=" + Param.Name.ToString());
            //    Console.WriteLine("  Type=" + Param.ParameterType.ToString());
            //    Console.WriteLine("  Position=" + Param.Position.ToString());
            //    Console.WriteLine("  Optional=" + Param.IsOptional.ToString());
            //}

            Console.WriteLine($"FullName: {SampleAssembly.FullName}");
            Console.WriteLine($"Location: {SampleAssembly.Location}");
            Console.WriteLine($"ImageRuntimeVersion: {SampleAssembly.ImageRuntimeVersion}");

            Type[] types = SampleAssembly.GetTypes();
            int counter = 1;
            foreach (var type in types)
            {
                Console.WriteLine($"Type {counter++} of {types.Length}");
                Console.WriteLine($"Type-FullName: {type.FullName}");
            }
        }

        public void sample2()
        {
            Core core = new Core();
            List<string> types = core.ExtractTypesNames(@"c:\temp\8\GFZ.NotifyByMail.Infra.dll");

            int counter = 1;
            foreach (var type in types)
            {
                Console.WriteLine($"Type {counter++} of {types.Count}");
                Console.WriteLine($"Type-FullName: {type}");
            }
        }

        public void sample3()
        {
            Core core = new Core();
            List<string> results = core.ExtractTypePropertiesNames(@"c:\temp\8\GFZ.NotifyByMail.Infra.dll", "GFZ.NotifyByMail.Infra.DBModels.MailLog");

            int counter = 1;
            foreach (var result in results)
            {
                Console.WriteLine($"Property {counter++} of {results.Count}");
                Console.WriteLine($"Property-Name: {result}");
            }
        }

        public void sample4()
        {
            Core core = new Core();
            var extractedType = core.ExtractType(@"c:\temp\8\LinkDev.FreeZones.ExecutionDepartment.DTO.dll", "LinkDev.FreeZones.ExecutionDepartment.DTO.Endorsement.ExportedToAbroadEndoresmentDTO");

            Console.WriteLine(extractedType.ToString());
        }

        public void sample5()
        {
            Core core = new Core();
            var extractedType = core.ExtractType(@"c:\temp\8\LinkDev.FreeZones.ExecutionDepartment.DTO.dll", "LinkDev.FreeZones.ExecutionDepartment.DTO.Endorsement.ExportedToAbroadEndoresmentDTO");

            FileManagerCore.Controller fileController = new FileManagerCore.Controller();
            fileController.SaveFile(false, $"{extractedType.TypeName}.csv", "ID,PathName,Name,Type,IsNullable");

            for (int i = 0; i < extractedType.Fields.Count; i++)
            {
                var item = extractedType.Fields[i];
                fileController.SaveFile(true, $"{extractedType.TypeName}.csv", $"{item.ID},{item.PathName},{item.Name},{item.Type},{item.IsNullable}");
            }
        }

        public void testType1()
        {
            Core core = new Core();
            Console.WriteLine($"type is {core.GetTypeName(typeof(int))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(long))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(short))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(byte))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(double))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(decimal))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(Guid))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(DateTime))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(string))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(bool))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(char))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(short))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(sbyte))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(float))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(uint))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(ulong))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(ushort))}");
            Console.WriteLine($"type is {core.GetTypeName(typeof(List<int>))}");
        }

        public void testType2()
        {
            var type = typeof(List<TestClass>);
            var typeInfo = type.GetTypeInfo();

            Console.WriteLine("List<TestClass>");
            Console.WriteLine($"Name is {type.Name}");
            Console.WriteLine($"FullName is {type.FullName}");
            Console.WriteLine($"Element Type is {type.GetGenericArguments()[0].Name}");
        }

        public void testType3()
        {
            var type = typeof(ICollection<TestClass>);
            var typeInfo = type.GetTypeInfo();

            Console.WriteLine("ICollection<TestClass>");
            Console.WriteLine($"Name is {type.Name}");
            Console.WriteLine($"FullName is {type.FullName}");
            Console.WriteLine($"Element Type is {type.GetGenericArguments()[0].Name}");
        }

        public class TestClass
        {
            public string TestProp1 { get; set; }
        }
    }
}
