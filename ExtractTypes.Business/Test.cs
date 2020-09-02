﻿using ExtractTypes.Business.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExtractTypes.Business
{
    public class Test
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
            var extractedType = core.ExtractType(@"c:\temp\8\Projects.DTO.dll", "LinkDev.GAFIFreeZones.Integration.Projects.DTO.Profile");

            Console.WriteLine(extractedType.ToString());
        }
    }
}
