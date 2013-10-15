﻿using System;

namespace MicroORM.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string EntityName { get; set; }
        public bool AutoGenerated { get; set; }

        public TableAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            this.EntityName = name;
            this.AutoGenerated = true;
        }
    }
}
