// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tracker.cs" company="">
//   
// </copyright>
// <summary>
//   The tracker class will track changes on specified objects as well as tracking if an object is dirty
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/************************************************************************************************************
 * LICENCE: The Code Project Open License (CPOL) 1.02
 * LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
 * AUTHOR(S): SACHA BARBER, IAN P JOHNSON
 * WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook
 ************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using RabbitDB.ChangeTracker;

namespace RabbitDB.Entity.ChangeTracker
{
    /// <summary>
    /// The tracker class will track changes on specified objects as well as tracking if an object is dirty
    /// </summary>
    internal class Tracker : ITracker
    {
        #region Fields

        /// <summary>
        /// The component helper.
        /// </summary>
        private ComponentTrackerHelper _componentHelper = new ComponentTrackerHelper();

        /// <summary>
        /// The component trackers.
        /// </summary>
        private List<IComponentTracker> _componentTrackers = new List<IComponentTracker>();

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Public Events

        /// <summary>
        /// Event is raised when the IsDirty flag is changed
        /// </summary>
        public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// True if any of the objects being tracked is dirty
        /// </summary>
        public bool IsDirty
        {
            get
            {
                // true if any of the component trackers are dirty
                return _componentTrackers.Any(x => x.IsDirty);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// List of component trackers associated with this tracker
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<IComponentTracker> ComponentTrackers()
        {
            return _componentTrackers;
        }

        /// <summary>
        /// Dispose of the tracker and all it's component trackers
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (IComponentTracker componentTracker in _componentTrackers)
            {
                componentTracker.Dispose();
            }

            _componentTrackers.Clear();
            _componentTrackers = null;
            _componentHelper = null;
            _disposed = true;
        }

        /// <summary>
        /// Mark all components in the tracker as clean
        /// </summary>
        public void MarkAsClean()
        {
            foreach (IComponentTracker componentTracker in _componentTrackers)
            {
                componentTracker.MarkAsClean();
            }
        }

        /// <summary>
        /// Track all properties on the specified object
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="objectToTrack">
        /// </param>
        public void TrackObject<T>(T objectToTrack)
        {
            TrackObject<T, object>(objectToTrack, null);
        }

        /// <summary>
        /// Track a specified property on an object or a child object
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TProp">
        /// </typeparam>
        /// <param name="objectToTrack">
        /// </param>
        /// <param name="trackingExpression">
        /// </param>
        public void TrackObject<T, TProp>(T objectToTrack, Expression<Func<T, TProp>> trackingExpression)
        {
            IComponentTracker componentTracker = GetComponentTracker(objectToTrack, trackingExpression);

            _componentTrackers.Add(componentTracker);
            componentTracker.IsDirtyChanged += ComponentTrackerOnIsDirtyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The component tracker on is dirty changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void ComponentTrackerOnIsDirtyChanged(object sender, IsDiryChangedArgs eventArgs)
        {
            IsDirtyChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// The get component tracker.
        /// </summary>
        /// <param name="objectToTrack">
        /// The object to track.
        /// </param>
        /// <param name="trackingExpression">
        /// The tracking expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TProp">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IComponentTracker"/>.
        /// </returns>
        private IComponentTracker GetComponentTracker<T, TProp>(
            T objectToTrack, 
            Expression<Func<T, TProp>> trackingExpression)
        {
            // if there is a property expression get the tracker information and create a new component tracker
            if (trackingExpression != null)
            {
                TrackerInfo trackerInfo = GetTrackerInfo(trackingExpression.Body);

                if (trackerInfo != null)
                {
                    return _componentHelper.CreateTracker(objectToTrack, trackerInfo);
                }
            }
            else if (objectToTrack is IEnumerable)
            {
                // object is an IEnumerable object so create a new list tracker and add it to component trackers
                return new ListComponentTracker(
                    _componentHelper, 
                    objectToTrack as IEnumerable, 
                    new TrackerInfo { TrackerType = ComponentTrackerType.List });
            }

            // normal object create LeafComponentTracker to monitor all properties on the object 
            return new LeafComponentTracker(
                _componentHelper, 
                objectToTrack, 
                new TrackerInfo { TrackerType = ComponentTrackerType.Leaf });
        }

        /// <summary>
        /// GetTrackerInfo recursively goes through the property expression
        /// </summary>
        /// <param name="expression">
        /// </param>
        /// <param name="currentInfo">
        /// </param>
        /// <returns>
        /// The <see cref="TrackerInfo"/>.
        /// </returns>
        private TrackerInfo GetTrackerInfo(Expression expression, TrackerInfo currentInfo = null)
        {
            MemberExpression memberExpression = expression as MemberExpression;

            return memberExpression != null
                       ? GetTrackerInfoForMemberExpression(memberExpression, currentInfo)
                       : GetTrackerInfoForMethodCallExpression(expression as MethodCallExpression, currentInfo);
        }

        /// <summary>
        /// The get tracker info for member expression.
        /// </summary>
        /// <param name="memberExpression">
        /// The member expression.
        /// </param>
        /// <param name="currentInfo">
        /// The current info.
        /// </param>
        /// <returns>
        /// The <see cref="TrackerInfo"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private TrackerInfo GetTrackerInfoForMemberExpression(
            MemberExpression memberExpression, 
            TrackerInfo currentInfo = null)
        {
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;

            // we only want to deal with property expressions.
            if (propertyInfo == null)
            {
                throw new Exception("Can only use properties");
            }

            // create a tracker info object to represent the property
            TrackerInfo trackInfo = new TrackerInfo
                                {
                                    PropertyName = memberExpression.Member.Name, 
                                    PropertyType = propertyInfo.PropertyType, 
                                    ChildTrackerInfo = currentInfo, 
                                };

            // handling string and IEnumerable separately with node trackers
            if (propertyInfo.PropertyType == typeof(string)
                || propertyInfo.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                trackInfo.TrackerType = ComponentTrackerType.Node;
            }
                
                // if this is the last node and it's not a value type use a leaf node tracker
            else if (currentInfo == null && !propertyInfo.PropertyType.IsValueType)
            {
                trackInfo.TrackerType = ComponentTrackerType.Leaf;
            }
            else
            {
                // finally everything else will be tracked with a node tracker
                trackInfo.TrackerType = ComponentTrackerType.Node;
            }

            // keep recursing as long as we have not hit the base parameter (i.e. x value in x.ChildA.Child1.IntTest)
            if (memberExpression.Expression != null &&
                memberExpression.Expression.NodeType != ExpressionType.Parameter)
            {
                return GetTrackerInfo(memberExpression.Expression, trackInfo);
            }

            return trackInfo;
        }

        /// <summary>
        /// The get tracker info for method call expression.
        /// </summary>
        /// <param name="methodCallExpression">
        /// The method call expression.
        /// </param>
        /// <param name="currentInfo">
        /// The current info.
        /// </param>
        /// <returns>
        /// The <see cref="TrackerInfo"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private TrackerInfo GetTrackerInfoForMethodCallExpression(
            MethodCallExpression methodCallExpression, 
            TrackerInfo currentInfo = null)
        {
            if (methodCallExpression == null)
            {
                return null;
            }

            // the only method allowed to be called in the expression is the TrackList extension
            if (methodCallExpression.Method.Name != "TrackList")
            {
                throw new Exception("Only TrackList is allowed to be called");
            }

            // create a tracker info for monitoring the list
            TrackerInfo listTracker = new TrackerInfo
                                  {
                                      ChildTrackerInfo = currentInfo, 
                                      TrackerType = ComponentTrackerType.List
                                  };

            // recurse back on the first arguement of the expression the IEnumerable
            TrackerInfo trackerInfo = GetTrackerInfo(methodCallExpression.Arguments[0], listTracker);

            // if the method is called with a filter method we need to handle it
            if (methodCallExpression.Arguments.Count != 2)
            {
                return trackerInfo;
            }

            // get the arguement then get it's type
            Expression arg1 = methodCallExpression.Arguments[1];
            Type filterExpressionType = arg1.Type;

            // find the CreateListComponentFilter method on the tracker
            MethodInfo openMethod = GetType()
                .GetMethod("CreateListComponentFilter", BindingFlags.Instance | BindingFlags.NonPublic);

            // create a closed method info using the first generic type in the expression (i.e. T in Func(T,bool) )
#if NET45
                    MethodInfo closeMethod = openMethod.MakeGenericMethod(filterExpressionType.GenericTypeArguments[0]);
#endif

#if NET4
            MethodInfo closeMethod = openMethod.MakeGenericMethod(filterExpressionType.GetGenericArguments()[0]);
#endif

            // invoke the closed method creating the list filter
            listTracker.ListFilter = (IListComponentFilter)closeMethod.Invoke(this, new object[] { arg1 });

            return trackerInfo;
        }

        #endregion
    }
}