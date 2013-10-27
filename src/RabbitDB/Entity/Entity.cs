namespace RabbitDB.Entity
{
    public abstract class Entity
    {
        internal bool MarkedForDeletion { get; set; }

        public Entity() { }
    }
}
