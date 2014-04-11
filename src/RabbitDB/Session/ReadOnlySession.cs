// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlySession.cs" company="">
//   
// </copyright>
// <summary>
//   The read only session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.Query.Generic;
    using RabbitDB.Reader;
    using RabbitDB.SqlBuilder;
    using RabbitDB.Storage;

    /// <summary>
    /// The read only session.
    /// </summary>
    public class ReadOnlySession : BaseDbSession, IReadOnlySession
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="dbEngine">
        /// The db engine.
        /// </param>
        public ReadOnlySession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySession"/> class.
        /// </summary>
        /// <param name="assemblyType">
        /// The assembly type.
        /// </param>
        public ReadOnlySession(Type assemblyType)
            : base(assemblyType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public ReadOnlySession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute multiple.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="MultiEntityReader"/>.
        /// </returns>
        public MultiEntityReader ExecuteMultiple(string sql, params object[] arguments)
        {
            var dataReader =
                this.SqlDialect.ExecuteReader(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));

            return new MultiEntityReader(dataReader, this.SqlDialect);
        }

        /// <summary>
        /// The get column value.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <typeparam name="V">
        /// </typeparam>
        /// <returns>
        /// The <see cref="V"/>.
        /// </returns>
        public V GetColumnValue<TEntity, V>(
            Expression<Func<TEntity, V>> selector, 
            Expression<Func<TEntity, bool>> criteria)
        {
            return default(V);
        }

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            var objectSet =
                ((IBaseDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));

            return objectSet.FirstOrDefault();
        }

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <param name="additionalPredicate">
        /// The additional predicate.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity GetEntity<TEntity>(object primaryKey, string additionalPredicate = null)
        {
            return GetEntity<TEntity>(new[] { primaryKey }, additionalPredicate);
        }

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="primaryKeys">
        /// The primary keys.
        /// </param>
        /// <param name="additionalPredicate">
        /// The additional predicate.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        /// <exception cref="PrimaryKeyException">
        /// </exception>
        public TEntity GetEntity<TEntity>(object[] primaryKeys, string additionalPredicate = null)
        {
            if (primaryKeys == null || primaryKeys.Length == 0)
            {
                throw new PrimaryKeyException("No primary Keys provided!");
            }

            var entitySet =
                ((IBaseDbSession)this).GetEntitySet<TEntity>(
                    new SqlQuery<TEntity>(primaryKeys, additionalPredicate));

            return entitySet.SingleOrDefault();
        }

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntityReader<TEntity> GetEntityReader<TEntity>(string sql, params object[] arguments)
        {
            return
                ((IBaseDbSession)this).GetEntityReader<TEntity>(
                    new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntityReader<TEntity> GetEntityReader<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IBaseDbSession)this).GetEntityReader<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntityReader<TEntity> GetEntityReader<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            var sqlBuilder = new SelectSqlBuilder(this.SqlDialect, tableInfo);
            return ((IBaseDbSession)this).GetEntityReader<TEntity>(new SqlQuery(sqlBuilder.GetBaseSelect()));
        }

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntitySet<TEntity> GetEntitySet<TEntity>(string sql, params object[] arguments)
        {
            return
                ((IBaseDbSession)this).GetEntitySet<TEntity>(
                    new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntitySet<TEntity> GetEntitySet<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IBaseDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntitySet<TEntity> GetEntitySet<TEntity>()
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var sqlBuilder = new SelectSqlBuilder(this.SqlDialect, tableInfo);

            return ((IBaseDbSession)this).GetEntitySet<TEntity>(new SqlQuery(sqlBuilder.GetBaseSelect()));
        }

        /// <summary>
        /// The get scalar value.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity GetScalarValue<TEntity>(string sql, params object[] arguments)
        {
            return this.SqlDialect.ExecuteScalar<TEntity>(
                new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        #endregion
    }
}