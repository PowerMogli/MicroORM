using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// The tracker class will track changes on specified objects as well as tracking if an object is dirty
    /// </summary>
    internal class Tracker : ITracker
    {
        private bool disposed;
        private List<IComponentTracker> componentTrackers = new List<IComponentTracker>();
        private ComponentTrackerHelper componentHelper = new ComponentTrackerHelper();

        /// <summary>
        /// Track all properties on the specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToTrack"></param>
        public void TrackObject<T>(T objectToTrack)
        {
            TrackObject<T, object>(objectToTrack, null);
        }

        /// <summary>
        /// Track a specified property on an object or a child object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="objectToTrack"></param>
        /// <param name="trackingExpression"></param>
        public void TrackObject<T, TProp>(T objectToTrack, Expression<Func<T, TProp>> trackingExpression)
        {
            IComponentTracker componentTracker = GetComponentTracker<T, TProp>(objectToTrack, trackingExpression);

            componentTrackers.Add(componentTracker);
            componentTracker.IsDirtyChanged += ComponentTrackerOnIsDirtyChanged;
        }

        private IComponentTracker GetComponentTracker<T, TProp>(T objectToTrack, Expression<Func<T, TProp>> trackingExpression)
        {
            // if there is a property expression get the tracker information and create a new component tracker
            if (trackingExpression != null)
            {
                TrackerInfo trackerInfo = GetTrackerInfo(trackingExpression.Body);

                if (trackerInfo != null)
                {
                    return componentHelper.CreateTracker(objectToTrack, trackerInfo);
                }
            }
            else if (objectToTrack is IEnumerable)
            {
                // object is an IEnumerable object so create a new list tracker and add it to component trackers
                return new ListComponentTracker(componentHelper, objectToTrack as IEnumerable, new TrackerInfo { TrackerType = ComponentTrackerType.List });
            }

            // normal object create LeafComponentTracker to monitor all properties on the object 
            return new LeafComponentTracker(componentHelper, objectToTrack, new TrackerInfo { TrackerType = ComponentTrackerType.Leaf });
        }

        /// <summary>
        /// True if any of the objects being tracked is dirty
        /// </summary>
        public bool IsDirty
        {
            get
            {
                // true if any of the component trackers are dirty
                return componentTrackers.Any(x => x.IsDirty);
            }
        }

        /// <summary>
        /// Mark all components in the tracker as clean
        /// </summary>
        public void MarkAsClean()
        {
            foreach (IComponentTracker componentTracker in componentTrackers)
            {
                componentTracker.MarkAsClean();
            }
        }

        /// <summary>
        /// List of component trackers associated with this tracker
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComponentTracker> ComponentTrackers()
        {
            return componentTrackers;
        }

        /// <summary>
        /// Event is raised when the IsDirty flag is changed
        /// </summary>
        public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

        /// <summary>
        /// Dispose of the tracker and all it's component trackers
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                foreach (IComponentTracker componentTracker in componentTrackers)
                {
                    componentTracker.Dispose();
                }

                componentTrackers.Clear();
                componentTrackers = null;
                componentHelper = null;
                disposed = true;
            }
        }

        private void ComponentTrackerOnIsDirtyChanged(object sender, IsDiryChangedArgs eventArgs)
        {
            if (IsDirtyChanged != null)
            {
                IsDirtyChanged(this, eventArgs);
            }
        }

        /// <summary>
        /// GetTrackerInfo recursively goes through the property expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="currentInfo"></param>
        /// <returns></returns>
        private TrackerInfo GetTrackerInfo(Expression expression, TrackerInfo currentInfo = null)
        {
            MemberExpression memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                return GetTrackerInfoForMemberExpression(memberExpression, currentInfo);
            }

            return GetTrackerInfoForMethodCallExpression(expression as MethodCallExpression, currentInfo);
        }

        private TrackerInfo GetTrackerInfoForMemberExpression(MemberExpression memberExpression, TrackerInfo currentInfo = null)
        {
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;

            // we only want to deal with property expressions.
            if (propertyInfo == null)
            {
                throw new Exception("Can only use properties");
            }

            // create a tracker info object to represent the property
            var trackInfo = new TrackerInfo
            {
                PropertyName = memberExpression.Member.Name,
                PropertyType = propertyInfo.PropertyType,
                ChildTrackerInfo = currentInfo,
            };

            // handling string and IEnumerable separately with node trackers
            if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                trackInfo.TrackerType = ComponentTrackerType.Node;
            }
            // if this is the last node and it's not a value type use a leaf node tracker
            else if (currentInfo == null && !propertyInfo.PropertyType.IsValueType)
            {
                trackInfo.TrackerType = ComponentTrackerType.Leaf;
            }
            else // finally everything else will be tracked with a node tracker
            {
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

        private TrackerInfo GetTrackerInfoForMethodCallExpression(MethodCallExpression methodCallExpression, TrackerInfo currentInfo = null)
        {
            if (methodCallExpression != null)
            {
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
                if (methodCallExpression.Arguments.Count == 2)
                {
                    // get the arguement then get it's type
                    Expression arg1 = methodCallExpression.Arguments[1];
                    Type filterExpressionType = arg1.Type;

                    // find the CreateListComponentFilter method on the tracker
                    MethodInfo openMethod = GetType().GetMethod("CreateListComponentFilter", BindingFlags.Instance | BindingFlags.NonPublic);


                    // create a closed method info using the first generic type in the expression (i.e. T in Func(T,bool) )
#if NET45
                    MethodInfo closeMethod = openMethod.MakeGenericMethod(filterExpressionType.GenericTypeArguments[0]);
#endif

#if NET4
                    MethodInfo closeMethod = openMethod.MakeGenericMethod(filterExpressionType.GetGenericArguments()[0]);
#endif
                    // invoke the closed method creating the list filter
                    listTracker.ListFilter = (IListComponentFilter)closeMethod.Invoke(this, new object[] { arg1 });
                }

                return trackerInfo;
            }
            return null;
        }

        /// <summary>
        /// This method takes a linq expression that represents a filter for type T
        /// It's called using reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterMethod"></param>
        /// <returns></returns>
        private IListComponentFilter CreateListComponentFilter<T>(Expression<Func<T, bool>> filterMethod)
        {
            // compile the filter method
            Func<T, bool> compiledDelegate = filterMethod.Compile();

            return new ListComponentFilter<T>(compiledDelegate);
        }
    }
}