// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifiedChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The notified change recorder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity.ChangeRecorder
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.ChangeTracker;
    using RabbitDB.Utils;

    /// <summary>
    /// The notified change recorder.
    /// </summary>
    internal class NotifiedChangeRecorder : BaseChangeRecorder
    {
        #region Fields

        /// <summary>
        /// The _tracker.
        /// </summary>
        private readonly ITracker _tracker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifiedChangeRecorder"/> class.
        /// </summary>
        /// <param name="tracker">
        /// The tracker.
        /// </param>
        /// <param name="validEntityArgumentsReader">
        /// The valid entity arguments reader.
        /// </param>
        public NotifiedChangeRecorder(ITracker tracker, IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            _tracker = tracker;
            _tracker.IsDirtyChanged += UpdateOrCreateHashSet;
            this.NotifiedValues = new Dictionary<string, object>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the notified values.
        /// </summary>
        private Dictionary<string, object> NotifiedValues { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The compute values to update.
        /// </summary>
        /// <returns>
        /// The <see cref="KeyValuePair"/>.
        /// </returns>
        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var validEntityValues = base.ValidArgumentReader.ReadValidEntityArguments();
            var valuesToUpdate = new Dictionary<string, object>();

            foreach (var kvp in this.NotifiedValues)
            {
                if (validEntityValues.Any(arg => arg.Key == kvp.Key))
                {
                    valuesToUpdate.Add(kvp.Key, validEntityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
                }
            }

            return valuesToUpdate.ToArray();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        private void Dispose(bool dispose)
        {
            if (dispose && base.Disposed == false)
            {
                _tracker.Dispose();
                this.NotifiedValues.Clear();
                this.NotifiedValues = null;
                _tracker.IsDirtyChanged -= UpdateOrCreateHashSet;

                base.Disposed = true;
            }
        }

        /// <summary>
        /// The update or create hash set.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void UpdateOrCreateHashSet(object sender, IsDiryChangedArgs args)
        {
            if (this.NotifiedValues.ContainsKey(args.PropertyName))
            {
                if (args.IsDirty)
                {
                    this.NotifiedValues[args.PropertyName] = args.NewValue;
                }
                else
                {
                    this.NotifiedValues.Remove(args.PropertyName);
                }
            }
            else
            {
                this.NotifiedValues.Add(args.PropertyName, args.NewValue);
            }
        }

        #endregion
    }
}