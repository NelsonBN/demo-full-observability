﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed record UpdateUserCommand(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;


    public async Task HandleAsync(Guid id, UserRequest request, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<UpdateUserCommand>(35);

        var user = await _repository.GetAsync(id, cancellationToken);
        if(user is null)
        {
            throw new UserNotFoundException(id);
        }

        user.Update(
            request.Name,
            request.Email,
            request.Phone);

        await _repository.UpdateAsync(user, cancellationToken);
    }
}
