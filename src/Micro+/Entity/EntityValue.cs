namespace MicroORM.Entity
{
    public struct EntityValue<TValue>
    {
        public readonly bool IsDirty;
        public readonly TValue Value;

        public EntityValue(TValue value)
        {
            this.IsDirty = true;
            this.Value = value;
        }

        public static implicit operator EntityValue<TValue>(TValue value)
        {
            return new EntityValue<TValue>(value);
        }
    }
}
