using System.Collections.Generic;
using System;

namespace MicroORM.Entity
{
    internal sealed class EntityInfo
    {
        internal EntityInfo()
        {
            this.EntityState = EntityState.None;
            this.EntityHashSet = new Dictionary<string, string>();
            this.LastCallTime = DateTime.Now;
        }

        internal bool CanBeRemoved { get { return DateTime.Now.Subtract(this.LastCallTime) > TimeSpan.FromMinutes(2); } }
        internal EntityState EntityState { get; set; }
        internal Dictionary<string, string> EntityHashSet { get; set; }
        internal DateTime LastCallTime { get; set; }
    }
}
