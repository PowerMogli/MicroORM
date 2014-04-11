// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB
{
    /// <summary>
    /// The configuration.
    /// </summary>
    public sealed class Configuration
    {
        #region Static Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static volatile Configuration instance;

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="Configuration"/> class from being created.
        /// </summary>
        private Configuration()
        {
            this.AutoDetectChangesEnabled = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes enabled.
        /// </summary>
        public bool AutoDetectChangesEnabled { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        internal static Configuration Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (SyncRoot)
                {
                    if (instance == null)
                    {
                        instance = new Configuration();
                    }
                }

                return instance;
            }
        }

        #endregion
    }
}