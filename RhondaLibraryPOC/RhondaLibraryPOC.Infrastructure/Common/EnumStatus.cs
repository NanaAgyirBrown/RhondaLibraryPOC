
namespace RhondaLibraryPOC.Infrastructure.Common;

public enum EnumStatus
{
    Success = 200,
    Created = 201,
    Activated = 202,
    Deactivated = 203,
    Updated = 204,
    BadOperation = 400,
    Suspended = 401,
    NotFound = 404,
    ExistingRecord = 409,
    DbException = 500,
    Unsupported = 501,
    Unavailable = 503,
    Timeout = 504
}
