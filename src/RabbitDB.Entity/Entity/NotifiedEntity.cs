using RabbitDB.Caching;
using RabbitDB.ChangeTracker;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace RabbitDB.Entity
{
    /// <summary>
    /// Base class implements INotifyPropertyChanged
    /// </summary>
    public class NotifiedEntity : Entity, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ITracker _tracker;
        private NotifiedEntityInfo _notifiedEntityInfo;

        public NotifiedEntity()
        {
            _tracker = new Tracker();
            _tracker.TrackObject(this);
            _tracker.IsDirtyChanged += UpdateHashSet;
        }

        ~NotifiedEntity()
        {
            if (_tracker != null)
            {
                _tracker.IsDirtyChanged -= UpdateHashSet;
                _tracker = null;
            }
        }

        public void Dispose()
        {
            if (_tracker != null)
            {
                _tracker.IsDirtyChanged -= UpdateHashSet;
                _tracker = null;
            }
        }

        private void UpdateHashSet(object sender, IsDiryChangedArgs args)
        {
            _notifiedEntityInfo = _notifiedEntityInfo ?? EntityInfoCacheManager.GetNotifiedEntityInfo(this);
            if (_notifiedEntityInfo != null)
            {
                _notifiedEntityInfo.UpdateOrCreateHashSet(args);
            }
        }

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
        /// Calling PropertyChanged event for given property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">Property in expression form (i.e. () => PropertyName).</param>
        public void NotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var propertyName = GetPropertyName(expression);
            OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

        internal override bool HasChanges(IEnumerable<KeyValuePair<string, object>> entityValues)
        {
            var valuesToUpdate = _notifiedEntityInfo.ComputeValuesToUpdate(this, entityValues);
            return valuesToUpdate.Count() > 0
                || MarkedForDeletion
                || _notifiedEntityInfo.EntityState == EntityState.Deleted
                || _notifiedEntityInfo.EntityState == EntityState.None;
        }
    }
}