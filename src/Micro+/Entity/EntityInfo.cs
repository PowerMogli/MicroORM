namespace MicroORM.Entity
{
    internal sealed class EntityInfo
    {
        internal EntityInfo()
        {
            this.EntityState = EntityState.None;
            this.EntityValueCollection = new EntityValueCollection();
        }

        internal EntityState EntityState { get; set; }
        internal EntityValueCollection EntityValueCollection { get; private set; }
    }
}
