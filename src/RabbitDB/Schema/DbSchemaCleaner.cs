// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSchemaCleaner.cs" company="">
//   
// </copyright>
// <summary>
//   The db schema cleaner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The db schema cleaner.
    /// </summary>
    internal class DbSchemaCleaner
    {
        #region Static Fields

        /// <summary>
        /// The clean up.
        /// </summary>
        internal static Func<string, string> CleanUp = (str) =>
        {
            str = RxCleanUp.Replace(str, "_");

            return str;
        };

        /// <summary>
        /// The rx clean up.
        /// </summary>
        static readonly Regex RxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        #endregion
    }
}