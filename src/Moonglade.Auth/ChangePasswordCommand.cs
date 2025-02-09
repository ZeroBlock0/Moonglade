﻿using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using Moonglade.Utils;

namespace Moonglade.Auth;

public record ChangePasswordCommand(Guid Id, string ClearPassword) : IRequest;

public class ChangePasswordCommandHandler : AsyncRequestHandler<ChangePasswordCommand>
{
    private readonly IRepository<LocalAccountEntity> _accountRepo;
    public ChangePasswordCommandHandler(IRepository<LocalAccountEntity> accountRepo) => _accountRepo = accountRepo;

    protected override async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ClearPassword))
        {
            throw new ArgumentNullException(nameof(request.ClearPassword), "value must not be empty.");
        }

        var account = await _accountRepo.GetAsync(request.Id);
        if (account is null)
        {
            throw new InvalidOperationException($"LocalAccountEntity with Id '{request.Id}' not found.");
        }

        account.PasswordHash = Helper.HashPassword(request.ClearPassword);
        await _accountRepo.UpdateAsync(account, cancellationToken);
    }
}