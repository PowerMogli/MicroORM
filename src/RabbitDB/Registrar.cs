// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Registrar.cs" company="">
//   
// </copyright>
// <summary>
//   The registrar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// The registrar.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public static class Registrar<T>
    {
        #region Static Fields

        /// <summary>
        /// The _container.
        /// </summary>
        private static readonly ConcurrentDictionary<string, T> Container = new ConcurrentDictionary<string, T>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The flush.
        /// </summary>
        public static void Flush()
        {
            Container.Clear();
        }

        /// <summary>
        /// Registers a value for the given namespace.
        /// </summary>
        /// <param name="nameSpace">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Register(string nameSpace, T value)
        {
            return !Container.ContainsKey(nameSpace) && Container.TryAdd(nameSpace, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get for.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        internal static T GetFor(Type entityType)
        {
            T value;

            var nameSpace = entityType.ToString();

            while (true)
            {
                nameSpace = string.Concat(nameSpace, ".*");

                if (Container.TryGetValue(nameSpace, out value) || nameSpace == ".*")
                {
                    break;
                }

                var lastIndexOf = nameSpace.LastIndexOf('.', nameSpace.Length - 3);
                nameSpace = lastIndexOf < 0 ? ".*" : nameSpace.Substring(0, lastIndexOf);
            }

            return value.Equals(null) == false ? value : default(T);
        }

        #endregion
    }
}