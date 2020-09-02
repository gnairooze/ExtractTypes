using ExtractTypes.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtractTypes.Business
{
    public class Core
    {
        protected Assembly LoadAssembly(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        protected Type[] getTypes(string assemblyPath, string typeFullName = null)
        {
            Assembly loadedAssembly = Assembly.LoadFrom(assemblyPath);

            Type[] types = loadedAssembly.GetTypes();
            if (typeFullName == null)
            {
                return types;
            }
            else
            {
                types = (from type in types.ToList()
                         where type.FullName == typeFullName
                         select type).ToArray();
            }

            return types;
        }
        protected PropertyInfo[] getTypeProperties(string assemblyPath, string typeFullName)
        {
            Type[] types = getTypes(assemblyPath, typeFullName);

            if (types.Length == 0)
            {
                throw new InvalidOperationException("type not found");
            }

            if (types.Length > 1)
            {
                throw new InvalidOperationException("type is repeated. please specify the type full name");
            }

            return types[0].GetProperties();
        }

        public List<string> ExtractTypesNames(string assemblyPath)
        {
            Type[] types = getTypes(assemblyPath);

            var query = from type in types
                        select type.FullName;
            return query.ToList();
        }

        public List<string> ExtractTypePropertiesNames(string assemblyPath, string typeFullName)
        {
            var properties = getTypeProperties(assemblyPath, typeFullName);

            var names = (from property in properties
                        select property.Name).ToList();

            return names;
        }

        public Models.ExtractedType ExtractType(string assemblyPath, string typeFullName)
        {
            Models.ExtractedType extractedType = new Models.ExtractedType();

            var type = getTypes(assemblyPath, typeFullName)[0];
            extractedType.AssemblyFullName = type.AssemblyQualifiedName;
            extractedType.TypeFullName = type.FullName;

            int counter = 1;
            FillProperties(extractedType, type, ref counter, string.Empty);

            return extractedType;
        }

        protected void FillProperties(Models.ExtractedType extractedType, Type type, ref int counter, string propertyPath)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var extractItem = new ExtractedItem()
                {
                    ID = counter++,
                    FullName = $"{propertyPath}.{property.Name}",
                    Name = property.Name,
                    Type = property.PropertyType.FullName
                };
                extractedType.Items.Add(extractItem);

                if (!IsSimple(property.PropertyType))
                {
                    FillProperties(extractedType, property.PropertyType, ref counter, extractItem.FullName);
                }
            }
        }

        protected bool IsSimple(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal))
              || type.Equals(typeof(DateTime))
              || type.Equals(typeof(Guid));
        }
    }
}
