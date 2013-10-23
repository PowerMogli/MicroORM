namespace MicroORM.Entity
{
    public abstract class Entity
    {
        internal EntityState EntityState { get; set; }
        public bool Delete { get; set; }

        public Entity()
        {
        }
    }
}
