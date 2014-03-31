using RabbitDB.Entity.ChangeTracker;
using RabbitDB.Utils;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RabbitDB.Entity
{
    /// <summary>
    /// Base class implements INotifyPropertyChanged
    /// </summary>
    public class NotifiedEntity : Entity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal override ChangeTracerOption ChangeTracerOption { get { return ChangeTracerOption.Notified; } }

        /// <summary>
        /// Sets a value to a particular property while calling PropertyChanged
        /// </summary>
        /// <typeparam name="T">type of property</typeparam>
        /// <param name="instanceField">backing field for the property</param>
        /// <param name="newValue">new value to set</param>
        public virtual void SetProperty<T>(Expression<Func<T>> expression, ref T instanceField, T newValue)
        {
            if (instanceField == null || !instanceField.Equals(newValue))
            {
                T oldValue = instanceField;
                instanceField = newValue;

                var propertyName = GetPropertyName(expression);
                OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
            }
        }

        /// <summary>
        /// Default implementation of Propertychanged event invoker
        /// </summary>
        /// <param name="propertyName">property that changed</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
    }
}