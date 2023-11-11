﻿
using ErrorOr;
using RhondaLibraryPOC.Application.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Users.Queries;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface IUserRepository
{
    Task<ErrorOr<UserDTO>> GetUserById(GetUserDetailsQuery query, CancellationToken cancellationToken);
    Task<ErrorOr<UserDTO>> AddUser(User command, CancellationToken cancellationToken);
    Task<ErrorOr<UserDTO>> UpdateUser(User command, CancellationToken cancellationToken);
}
