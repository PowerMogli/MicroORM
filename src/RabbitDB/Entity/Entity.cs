namespace RabbitDB.Entity
{
    public abstract class Entity
    {
        internal bool MarkedForDeletion { get; set; }
        internal bool ChangeTrackingEnabled { get; set; }

        public Entity()
        {
            this.ChangeTrackingEnabled = true;
        }
    }
}
