﻿using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;

namespace Moonglade.Auth;

public record LogSuccessLoginCommand(Guid Id, string IpAddress) : IRequest;

public class LogSuccessLoginCommandHandler : AsyncRequestHandler<LogSuccessLoginCommand>
{
    private readonly IRepository<LocalAccountEntity> _accountRepo;
    public LogSuccessLoginCommandHandler(IRepository<LocalAccountEntity> accountRepo) => _accountRepo = accountRepo;

    protected override async Task Handle(LogSuccessLoginCommand request, CancellationToken cancellationToken)
    {
        var (id, ipAddress) = request;

        var entity = await _accountRepo.GetAsync(id);
        if (entity is not null)
        {
            entity.LastLoginIp = ipAddress.Trim();
            entity.LastLoginTimeUtc = DateTime.UtcNow;
            await _accountRepo.UpdateAsync(entity, cancellationToken);
        }
    }
}