using System;

namespace MicroORM.Entity
{
    internal sealed class EntityInfo : ICloneable
    {
        private bool _markForDeletion;
        private byte[] _checksum;

        /// <summary>
        /// Creates an instance of the <see cref="EntityInfo">ObjectInfo Class</see>.
        /// </summary>
        public EntityInfo() { }

        public EntityState EntityState { get; set; }

        public bool MarkForDeletion
        {
            get { return _markForDeletion; }
            set { _markForDeletion = value; }
        }

        /// <summary>
        /// Specifies the checksum of the associated persistent object. This sum is used
        /// to check if the object has been changed since being in memory.
        /// </summary>
        /// <returns>A byte array that contains the checksum for the persistent object.</returns>
        public byte[] Checksum
        {
            get { return _checksum; }
            set { _checksum = value; }
        }

        /// <summary>
        /// Restores the data of the <paramref name="ObjectInfo">ObjectInfo</paramref> object
        /// by using the data of the <paramref name="ObjectInfo">ObjectInfo</paramref> object
        /// passed.
        /// </summary>
        /// <param name="objectInfo">
        /// 	<see cref="EntityInfo">ObjectInfo</see> object used to restored this
        /// instance.
        /// </param>
        internal void Restore(EntityInfo objectInfo)
        {
            this.MarkForDeletion = objectInfo.MarkForDeletion;
            this.Checksum = objectInfo.Checksum;
        }

        #region ICloneable Members

        public EntityInfo Clone()
        {
            // ATTENTION: When changing this method change also the Restore method. They are required for transaction management.
            EntityInfo objectInfo = new EntityInfo();

            objectInfo.MarkForDeletion = _markForDeletion;
            if (_checksum != null)
                objectInfo.Checksum = (byte[])_checksum.Clone();

            return objectInfo;
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
