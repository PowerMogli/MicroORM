// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifiedEntity.cs" company="">
//   
// </copyright>
// <summary>
//   Base class implements INotifyPropertyChanged
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.ComponentModel;
using System.Linq.Expressions;

using RabbitDB.Entity.ChangeRecorder;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.Entity.Entity
{
    /// <summary>
    ///     Base class implements INotifyPropertyChanged
    /// </summary>
    public class NotifiedEntity : Entity,
                                  INotifyPropertyChanged
    {
        #region  Properties

        /// <summary>
        ///     The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the change tracer option.
        /// </summary>
        internal override ChangeRecorderOption ChangeTracerOption => ChangeRecorderOption.Notified;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets a value to a particular property while calling PropertyChanged
        /// </summary>
        /// <typeparam name="T">
        ///     type of property
        /// </typeparam>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="instanceField">
        ///     backing field for the property
        /// </param>
        /// <param name="newValue">
        ///     new value to set
        /// </param>
        public virtual void SetProperty<T>(Expression<Func<T>> expression, ref T instanceField, T newValue)
        {
            if (!instanceField.Equals(null) && instanceField.Equals(newValue))
            {
                return;
            }

            T oldValue = instanceField;
            instanceField = newValue;

            string propertyName = GetPropertyName(expression);
            OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The get property name.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;

            return memberExpression.Member.Name;
        }

        /// <summary>
        ///     Default implementation of Propertychanged event invoker
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        #endregion
    }
}