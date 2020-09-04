using ExtractTypes.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;

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
                    IsNullable = isNullable(property.PropertyType),
                    IsRequired = isRequired(property),
                    MaxLength = maxLength(property),
                    MinLength = minLength(property),
                    SizeInBytes = getTypeSize(property)
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

        protected int? maxLength(PropertyInfo property)
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

        protected int? minLength(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string))
            {
                return null;
            }

            var attribute = property.CustomAttributes.SingleOrDefault(a => a.AttributeType.Name == "StringLengthAttribute");

            if (attribute != null && attribute.ConstructorArguments != null && attribute.NamedArguments.Count > 0)
            {

                var arg = attribute.NamedArguments.SingleOrDefault(a => a.MemberName == "MinimumLength");

                if(arg != null && arg.TypedValue != null && arg.TypedValue.ArgumentType == typeof(int))
                {
                    return (int)arg.TypedValue.Value;
                }
            }

            return null;
        }
        protected bool isRequired(PropertyInfo property)
        {
            return property.CustomAttributes.Any(a => a.AttributeType.Name == "RequiredAttribute");
        }

        protected string getPropertyName(PropertyInfo property)
        {
            var interfaces = property.PropertyType.GetInterfaces();

            bool isCollection = interfaces.Any(type => type.Name == "IList" || type.Name == "ICollection" || type.Name == "IEnumerable" || type.Name == "IQueryable") && property.PropertyType != typeof(string);
            
            if (isCollection)
            {
                return $"{property.Name}[]";
            }

            return property.Name;
        }

        protected Type GetTypeEnhanced(Type type)
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
    
        protected int? getTypeSize(PropertyInfo property)
        {
            //handle string
            if(property.PropertyType == typeof(string))
            {
                var length = maxLength(property).Value;

                if(length == -1)
                {
                    return -1; //something like  mssql nvarchar(max)
                }
                else
                {
                    return length * 2;//to repsect unicode characters that take 2 bytes each
                }
            }

            if (property.PropertyType == typeof(DateTime))
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

                    if (tempLength > length)
                    {
                        length = tempLength;
                    }
                }

                return length * 2; //I treated it as unicode string every character occupies 2 bytes
            }

            //handle simple types
            if (isSimple(property.PropertyType))
            {
                return Marshal.SizeOf(property.PropertyType);
            }

            return null;
        }
    }
}
