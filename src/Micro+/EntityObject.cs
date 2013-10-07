namespace MicroORM.Storage
{
    public abstract class EntityObject
    {
        public EntityObject()
        {
            this.EntityState = EntityState.None;
        }

        protected EntityState EntityState { get; set; }
    }
}
