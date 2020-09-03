using ExtractTypes.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtractTypes.Business
{
    public class Core
    {
        protected List<string> _ReservedTypes = new List<string> { "System.String", "System.Decimal", "System.DateTime", "System.Guid" };
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
            extractedType.TypeName = type.Name;

            int counter = 1;
            fillProperties(extractedType, type, ref counter, string.Empty, new List<string>());

            return extractedType;
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

        protected void fillProperties(Models.ExtractedType extractedType, Type type, ref int counter, string propertyPath, List<string> parents)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                string propertyName = getPropertyName(property);

                var extractItem = new ExtractedItem()
                {
                    ID = counter++,
                    PathName = $"{propertyPath}.{propertyName}",
                    Name = propertyName,
                    Type = GetTypeName(property.PropertyType),
                    IsNullable = isNullable(property.PropertyType)
                };
                extractItem.Parents.AddRange(parents);
                extractedType.Fields.Add(extractItem);

                Console.WriteLine(extractItem.ToString());

                var propertyEnhancedType = GetTypeEnhanced(property.PropertyType);
                
                
                if (!isSimple(propertyEnhancedType) && parents.IndexOf(extractItem.Name) == -1)
                {
                    parents.Add(propertyName);
                    fillProperties(extractedType, propertyEnhancedType, ref counter, extractItem.PathName, parents);
                }
            }
        }

        private string getPropertyName(PropertyInfo property)
        {
            var interfaces = property.PropertyType.GetInterfaces();

            bool isCollection = interfaces.Any(type => type.Name == "IList" || type.Name == "ICollection" || type.Name == "IEnumerable" || type.Name == "IQueryable") && property.PropertyType != typeof(string);
            
            if (isCollection)
            {
                return $"{property.Name}[]";
            }

            return property.Name;
        }

        private Type GetTypeEnhanced(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return typeInfo.GetGenericArguments()[0];
            }

            return type;
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

        public string GetTypeName(Type type)
        {
            if (isNullable(type))
            {
                return $"Nullable<{type.GetGenericArguments()[0].Name}>";
            }

            if (type.Name.Contains("`1"))
            {
                StringBuilder bld = new StringBuilder();
                bld.Append(type.Name.Replace("`1", ""));
                bld.Append("<");
                bld.Append(type.GetGenericArguments()[0].Name);
                bld.Append(">");
                return bld.ToString();
            }

            return type.Name;
        }
    }
}
