// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifiedChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The notified change recorder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Entity.ChangeTracker;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.Entity.ChangeRecorder
{
    /// <summary>
    ///     The notified change recorder.
    /// </summary>
    internal class NotifiedChangeRecorder : BaseChangeRecorder
    {
        #region Fields

        /// <summary>
        ///     The _tracker.
        /// </summary>
        private readonly ITracker _tracker;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifiedChangeRecorder" /> class.
        /// </summary>
        /// <param name="tracker">
        ///     The tracker.
        /// </param>
        /// <param name="validEntityArgumentsReader">
        ///     The valid entity arguments reader.
        /// </param>
        public NotifiedChangeRecorder(ITracker tracker, IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            _tracker = tracker;
            _tracker.IsDirtyChanged += UpdateOrCreateHashSet;
            NotifiedValues = new Dictionary<string, object>();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the notified values.
        /// </summary>
        private Dictionary<string, object> NotifiedValues { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compute values to update.
        /// </summary>
        /// <returns>
        ///     The <see cref="KeyValuePair{string, object}" />.
        /// </returns>
        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            IEnumerable<KeyValuePair<string, object>> validEntityValues = ValidArgumentReader.ReadValidEntityArguments();
            Dictionary<string, object> valuesToUpdate = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> kvp in NotifiedValues)
            {
                if (validEntityValues.Any(arg => arg.Key == kvp.Key))
                {
                    valuesToUpdate.Add(kvp.Key, validEntityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key)
                                                                 .Value);
                }
            }

            return valuesToUpdate.ToArray();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="dispose">
        ///     The dispose.
        /// </param>
        private void Dispose(bool dispose)
        {
            if (!dispose || Disposed)
            {
                return;
            }

            _tracker.Dispose();
            NotifiedValues.Clear();
            NotifiedValues = null;
            _tracker.IsDirtyChanged -= UpdateOrCreateHashSet;

            Disposed = true;
        }

        /// <summary>
        ///     The update or create hash set.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void UpdateOrCreateHashSet(object sender, IsDiryChangedArgs args)
        {
            if (NotifiedValues.ContainsKey(args.PropertyName))
            {
                if (args.IsDirty)
                {
                    NotifiedValues[args.PropertyName] = args.NewValue;
                }
                else
                {
                    NotifiedValues.Remove(args.PropertyName);
                }
            }
            else
            {
                NotifiedValues.Add(args.PropertyName, args.NewValue);
            }
        }

        #endregion
    }
}