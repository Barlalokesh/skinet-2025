using System;
using core.Entities;

namespace core.Interface;

public interface IUnitOfWork: IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity  : BaseEntity;
    Task <bool> Complete();

}
