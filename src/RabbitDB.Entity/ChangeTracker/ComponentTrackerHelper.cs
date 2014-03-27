using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// ComponentTrackerHelper is used create property accessor and new component trackers. 
    /// </summary>
    internal class ComponentTrackerHelper
    {
        private readonly Dictionary<string, Func<object, object>> accessMethods = new Dictionary<string, Func<object, object>>();

        /// <summary>
        /// Access a named property on an object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object AccessProperty(object target, string propertyName)
        {
            Func<object, object> accessDelegate = GetPropertyAccessor(target, propertyName);

            if (accessDelegate != null)
            {
                return accessDelegate(target);
            }

            throw new Exception(string.Format("Could not access property {0} on {1}", propertyName, target.GetType().FullName));
        }

        /// <summary>
        /// Creates a new property accessor function
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public Func<object, object> GetPropertyAccessor(object target, string propertyName)
        {
            string fullPropertyName = target.GetType().FullName + "|" + propertyName;
            Func<object, object> accessDelegate;

            if (!accessMethods.TryGetValue(fullPropertyName, out accessDelegate))
            {
                PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

                // create the parameter for the instance parameter of type object
                ParameterExpression inParameter = Expression.Parameter(typeof(object), "objectParam");

                // cast the instance to it's real type
                Expression castExpression = Expression.Convert(inParameter, target.GetType());

                // create the property access expression
                Expression propertyAccessExpression =
                    Expression.Property(castExpression, propertyInfo);

                // cast the property access expression to an object
                Expression returnCastExpression = Expression.Convert(propertyAccessExpression, typeof(object));

                accessDelegate = Expression.Lambda<Func<object, object>>(returnCastExpression, inParameter).Compile();

                accessMethods.Add(fullPropertyName, accessDelegate);
            }

            return accessDelegate;
        }

        /// <summary>
        /// Creates a new component tracker based on the provided TrackerInfo
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public IComponentTracker CreateTracker(object target, TrackerInfo path)
        {
            switch (path.TrackerType)
            {
                case ComponentTrackerType.Node:
                    return new NodeComponentTracker(this, target, path);

                case ComponentTrackerType.Leaf:
                    return new LeafComponentTracker(this, target, path);

                case ComponentTrackerType.List:
                    return new ListComponentTracker(this, target as IEnumerable, path);
            }

            throw new Exception("Unknown tracker type: " + path.TrackerType);
        }
    }
}