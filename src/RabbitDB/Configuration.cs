namespace RabbitDB.Base
{
    public sealed class Configuration
    {
        private static volatile Configuration instance;
        private static object syncRoot = new object();

        private Configuration()
        {
            this.AutoDetectChangesEnabled = true;
        }

        internal static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Configuration();
                    }
                }

                return instance;
            }
        }
        
        public bool AutoDetectChangesEnabled { get; set; }
    }
}
