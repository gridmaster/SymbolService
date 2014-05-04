using System;
using System.Data.Entity;

namespace SymbolService.Models.Contracts
{
    public interface IDbContext : IDisposable
    {
        IDbSet<T> CreateDbSet<T>() where T : class;
        void SaveChanges();
    }
}