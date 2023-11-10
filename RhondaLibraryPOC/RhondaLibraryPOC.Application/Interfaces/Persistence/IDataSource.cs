using System.Data;

namespace RhondaLibraryPOC.Application.Interfaces.Persistence;

public interface IDataSource
{
    public Task<IDbConnection> CreateConnection();
}
