using System;
using FluentValidation;
using MediatR;

namespace Application.Core;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validator is null) return await next(cancellationToken);

        var validationRequest = await validator.ValidateAsync(request, cancellationToken);

        if (validationRequest.IsValid) return await next(cancellationToken);

        throw new ValidationException(validationRequest.Errors);
    }
}
