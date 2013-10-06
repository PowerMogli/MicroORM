using System;
using System.Reflection;

namespace MicroORM.Base.Mapping
{
    internal static class TypeMappingBuilder
    {
        internal static TypeMapping CreateTypeMapping(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsInterface)
            {
                throw new TypeMappingException(
                    string.Format("Cannot create mapping for interface '{0}'! Please use TypeMapping.RegisterPersistentInterface to register the interface with persistent type.", type.FullName));
            }
            TableAttribute attribute = GetPersistentAttribute(type);
            if (attribute == null)
                throw new TypeMappingException(string.Format("Cannot create mapping for type '{0}' without persistent attribute.", type.FullName));

            MemberInfoCollection members = new MemberInfoCollection();
            CreateMemberMappings(type, members);

            return new TypeMapping(type, attribute, members);
        }

        private static TableAttribute GetPersistentAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            TableAttribute attribute = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute), true);
            if (attribute == null) return null;

            if (attribute.EntityName == null)
                attribute.EntityName = type.Name;

            return attribute;
        }

        private static void CreateMemberMappings(Type type, MemberInfoCollection list)
        {
            if (list == null) throw new ArgumentNullException("list");

            // Return if we reached the object.
            if (type == null || type == typeof(object)) return;

            foreach (MemberInfo member in type.GetMembers(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly))
            {
                FieldAttribute fieldAttribute = GetMemberFieldAttribute(type, member);
                if (fieldAttribute == null) continue;

                if (string.IsNullOrEmpty(fieldAttribute.FieldName))
                    fieldAttribute.FieldName = member.Name;

                Type memberType = GetMemberType(type, member);

                IMemberInfo info = null;
                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        info = new PropertyMetaInfo((PropertyInfo)member, memberType, fieldAttribute);
                        break;
                    default:
                        throw new TypeMappingException("Member type is not supported for mapping.");
                }

                list.Add(info);
            }

            // Create the mapping for the base class.
            CreateMemberMappings(type.BaseType, list);
        }

        private static Type GetMemberType(Type type, MemberInfo member)
        {
            Type memberType = null;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    memberType = ((FieldInfo)member).FieldType;
                    break;
                case MemberTypes.Property:
                    memberType = ((PropertyInfo)member).PropertyType;
                    break;
                default:
                    throw new TypeMappingException("Member type is not supported for mapping.");
            }

            if (memberType.IsInterface)
            {
                // Get an instance of the given type.
                object instance = Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

                // Get the value of the member.
                object value = null;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        value = ((FieldInfo)member).GetValue(instance);
                        break;
                    case MemberTypes.Property:
                        value = ((PropertyInfo)member).GetValue(instance, null);
                        break;
                    default:
                        throw new TypeMappingException("Member type is not supported for mapping.");
                }

                if (value != null)
                    return value.GetType();
            }

            return memberType;
        }

        private static FieldAttribute GetMemberFieldAttribute(Type type, MemberInfo member)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (member == null)
                throw new ArgumentNullException("member");

            return (FieldAttribute)Attribute.GetCustomAttribute(member, typeof(FieldAttribute), false);
        }
    }
}
