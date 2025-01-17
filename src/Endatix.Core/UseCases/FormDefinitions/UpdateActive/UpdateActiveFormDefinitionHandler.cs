﻿using Endatix.Core.Abstractions.Repositories;
using Endatix.Core.Entities;
using Endatix.Core.Infrastructure.Messaging;
using Endatix.Core.Infrastructure.Result;
using Endatix.Core.Specifications;

namespace Endatix.Core.UseCases.FormDefinitions.UpdateActive;

public class UpdateActiveFormDefinitionHandler(IFormsRepository formRepository) : ICommandHandler<UpdateActiveFormDefinitionCommand, Result<FormDefinition>>
{
    public async Task<Result<FormDefinition>> Handle(UpdateActiveFormDefinitionCommand request, CancellationToken cancellationToken)
    {
        var spec = new ActiveFormDefinitionByFormIdSpec(request.FormId);
        var formWithActiveDefinition = await formRepository.SingleOrDefaultAsync(spec, cancellationToken);
        var activeDefinition = formWithActiveDefinition?.ActiveDefinition;
        if (formWithActiveDefinition == null || activeDefinition == null)
        {
            return Result.NotFound("Active form definition not found.");
        }

        activeDefinition.UpdateSchema(request.JsonData);
        activeDefinition.UpdateDraftStatus(request.IsDraft);

        await formRepository.UpdateAsync(formWithActiveDefinition, cancellationToken);
        return Result.Success(activeDefinition);
    }
}
