using ExtractTypes.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        public void testAttributes1()
        {
            var type = typeof(TestClass);
            var properties = type.GetProperties();
            var attributes = properties[0].CustomAttributes;

            foreach (var attribute in attributes)
            {
                Console.WriteLine($"{attribute.AttributeType.Name}");
                foreach (var arg in attribute.ConstructorArguments)
                {
                    Console.WriteLine($"{arg.ArgumentType.Name}:{arg.Value}");
                }
                foreach (var arg in attribute.NamedArguments)
                {
                    Console.WriteLine($"{arg.MemberName}:{arg.TypedValue.Value}");
                }
                Console.WriteLine("------------------------");
            }
        }

        public void testAttributes2()
        {
            var type = typeof(TestClass);
            var properties = type.GetProperties();
            bool isRequired = properties[0].CustomAttributes.Any(a => a.AttributeType.Name == "RequiredAttribute");

            Console.WriteLine($"Name: {properties[0].Name}, IsRequired: {isRequired}");

            isRequired = properties[1].CustomAttributes.Any(a => a.AttributeType.Name == "RequiredAttribute");

            Console.WriteLine($"Name: {properties[1].Name}, IsRequired: {isRequired}");
        }

        public void testAttributes3()
        {
            var type = typeof(TestClass);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                Console.WriteLine($"Name: {property.Name}, MaxLength: {maxLength(property)}");
            }
        }

        public void testAttributes4()
        {
            var type = typeof(TestClass);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                Console.WriteLine($"Name: {property.Name}, MinLength: {minLength(property)}");
            }
        }

        public void testTypeSize1()
        {
            var type = typeof(int);
            Console.WriteLine($"Size of {type.Name} is {Marshal.SizeOf(type)}");
        }

        public void testTypeSize2()
        {
            var properties = typeof(TestClass).GetProperties();

            foreach (var property in properties)
            {
                Console.WriteLine($"Size of {property.Name} is {getTypeSize(property)}");
            }
            
        }

        protected bool isNullable(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            return false;
        }

        protected bool isSimple(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (isNullable(type))
            {
                // nullable type, check if the nested type is simple.
                return isSimple(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal))
              || type.Equals(typeof(DateTime))
              || type.Equals(typeof(Guid));
        }

        protected int? getTypeSize(PropertyInfo property)
        {
            //handle string
            if (property.PropertyType == typeof(string))
            {
                var length = maxLength(property).Value;

                if (length == -1)
                {
                    return -1; //something like  mssql nvarchar(max)
                }
                else
                {
                    return length * 2;//to repsect unicode characters that take 2 bytes each
                }
            }

            if(property.PropertyType == typeof(DateTime))
            {
                return 8;
            }

            if (property.PropertyType == typeof(bool))
            {
                return 1;
            }

            if (property.PropertyType.GetTypeInfo().IsEnum)
            {
                string[] names = Enum.GetNames(property.PropertyType);
                int length = 0;
                foreach (var name in names)
                {
                    int tempLength = name.Length;

                    if(tempLength > length)
                    {
                        length = tempLength;
                    }
                }

                return length*2; //I treated it as unicode string every character occupies 2 bytes
            }

            //handle simple types
            if (isSimple(property.PropertyType))
            {
                return Marshal.SizeOf(property.PropertyType);
            }

            return null;
        }

        private int? minLength(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string))
            {
                return null;
            }

            var attribute = property.CustomAttributes.SingleOrDefault(a => a.AttributeType.Name == "StringLengthAttribute");

            if (attribute != null && attribute.ConstructorArguments != null && attribute.NamedArguments.Count > 0)
            {

                var arg = attribute.NamedArguments.SingleOrDefault(a => a.MemberName == "MinimumLength");

                if (arg != null && arg.TypedValue != null && arg.TypedValue.ArgumentType == typeof(int))
                {
                    return (int)arg.TypedValue.Value;
                }
            }

            return null;
        }
        private int? maxLength(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string))
            {
                return null;
            }

            var attribute = property.CustomAttributes.SingleOrDefault(a => a.AttributeType.Name == "MaxLengthAttribute");

            if (attribute != null && attribute.ConstructorArguments != null && attribute.ConstructorArguments.Count > 0 && attribute.ConstructorArguments[0].ArgumentType == typeof(int))
            {
                return (int)attribute.ConstructorArguments[0].Value;
            }

            attribute = property.CustomAttributes.SingleOrDefault(a => a.AttributeType.Name == "StringLengthAttribute");

            if (attribute != null && attribute.ConstructorArguments != null && attribute.ConstructorArguments.Count > 0 && attribute.ConstructorArguments[0].ArgumentType == typeof(int))
            {
                return (int)attribute.ConstructorArguments[0].Value;
            }


            return -1;
        }
        public class TestClass
        {
            [Required]
            [StringLength(50, MinimumLength = 30)]
            [MaxLength(50)]
            public string TestProp1 { get; set; }

            public string TestProp2 { get; set; }
            [StringLength(40, MinimumLength = 30)]
            public string TestProp3 { get; set; }

            public DateTime BirthDate { get; set; }
            public Guid ID { get; set; }
            public Decimal Price { get; set; }
            
            public Status ProcessStatus { get; set; }
            
            public enum Status { NotSet = 0, Completed = 1, NotStarted = 2}

        }
    }
}
