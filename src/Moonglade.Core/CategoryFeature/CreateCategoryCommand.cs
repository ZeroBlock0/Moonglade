﻿using MediatR;
using Moonglade.Caching;
using Moonglade.Data;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonglade.Core.CategoryFeature
{
    public class CreateCategoryCommand : IRequest
    {
        public CreateCategoryCommand(EditCategoryRequest payload)
        {
            Payload = payload;
        }

        public EditCategoryRequest Payload { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly IRepository<CategoryEntity> _catRepo;
        private readonly IBlogAudit _audit;
        private readonly IBlogCache _cache;

        public CreateCategoryCommandHandler(IRepository<CategoryEntity> catRepo, IBlogAudit audit, IBlogCache cache)
        {
            _catRepo = catRepo;
            _audit = audit;
            _cache = cache;
        }

        public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var exists = _catRepo.Any(c => c.RouteName == request.Payload.RouteName);
            if (exists) return Unit.Value;

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                RouteName = request.Payload.RouteName.Trim(),
                Note = request.Payload.Note?.Trim(),
                DisplayName = request.Payload.DisplayName.Trim()
            };

            await _catRepo.AddAsync(category);
            _cache.Remove(CacheDivision.General, "allcats");

            await _audit.AddEntry(BlogEventType.Content, BlogEventId.CategoryCreated, $"Category '{category.RouteName}' created");
            return Unit.Value;
        }
    }
}
