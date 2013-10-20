using System;

namespace MicroORM.Entity
{
    internal sealed class EntityInfo : ICloneable
    {

        public EntityInfo() { }

        public EntityState EntityState { get; set; }

        public byte[] Checksum { get; set; }

        internal void Restore(EntityInfo objectInfo)
        {
            this.Checksum = objectInfo.Checksum;
        }

        #region ICloneable Members

        public EntityInfo Clone()
        {
            // ATTENTION: When changing this method change also the Restore method. They are required for transaction management.
            EntityInfo objectInfo = new EntityInfo();

            if (this.Checksum != null)
                objectInfo.Checksum = (byte[])this.Checksum.Clone();

            return objectInfo;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
