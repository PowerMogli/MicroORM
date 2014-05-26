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
    using System.Runtime.CompilerServices;

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
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return instance ?? (instance = new Configuration());
            }
        }

        #endregion
    }
}