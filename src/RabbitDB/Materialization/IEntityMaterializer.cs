using System;
using System.Collections.Generic;
using System.Data;

namespace RabbitDB.Materialization
{
    interface IEntityMaterializer
    {
        TEntity Materialize<TEntity>(IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord);
        IEnumerable<TEntity> Materialize<TEntity>(Func<IDataReader, IEnumerable<TEntity>> materializer, IDataReader dataReader);
        TEntity Materialize<TEntity>(TEntity entity, IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord);
    }
}
