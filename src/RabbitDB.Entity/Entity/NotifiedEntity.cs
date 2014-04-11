// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifiedEntity.cs" company="">
//   
// </copyright>
// <summary>
//   Base class implements INotifyPropertyChanged
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using RabbitDB.Entity.ChangeRecorder;
    using RabbitDB.Utils;

    /// <summary>
    /// Base class implements INotifyPropertyChanged
    /// </summary>
    public class NotifiedEntity : Entity, INotifyPropertyChanged
    {
        #region Public Events

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the change tracer option.
        /// </summary>
        internal override ChangeRecorderOption ChangeTracerOption
        {
            get
            {
                return ChangeRecorderOption.Notified;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets a value to a particular property while calling PropertyChanged
        /// </summary>
        /// <typeparam name="T">
        /// type of property
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="instanceField">
        /// backing field for the property
        /// </param>
        /// <param name="newValue">
        /// new value to set
        /// </param>
        public virtual void SetProperty<T>(Expression<Func<T>> expression, ref T instanceField, T newValue)
        {
            if (!instanceField.Equals(null) && instanceField.Equals(newValue))
            {
                return;
            }

            var oldValue = instanceField;
            instanceField = newValue;

            var propertyName = GetPropertyName(expression);
            OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get property name.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Default implementation of Propertychanged event invoker
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion
    }
}