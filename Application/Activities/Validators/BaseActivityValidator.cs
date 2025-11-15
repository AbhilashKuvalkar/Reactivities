using System;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators;

public class BaseActivityValidator<T, TDto> : AbstractValidator<T> where TDto
    : BaseActivityDto
{
    public BaseActivityValidator(Func<T, TDto> selector)
    {
        RuleFor(x => selector(x).Title)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Title)} is required.")
            .MaximumLength(100).WithMessage($"{nameof(BaseActivityDto.Title)} must not exceed 100 characters.");

        RuleFor(x => selector(x).Description)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Description)} is required.");

        RuleFor(x => selector(x).Date)
            .GreaterThan(DateTime.UtcNow).WithMessage($"{nameof(BaseActivityDto.Date)} must be in the future.");

        RuleFor(x => selector(x).Category)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Category)} is required.");

        RuleFor(x => selector(x).City)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.City)} is required.");

        RuleFor(x => selector(x).Venue)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Venue)} is required.");

        RuleFor(x => selector(x).Latitude)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Latitude)} is required.")
            .InclusiveBetween(-90, 90).WithMessage($"{nameof(BaseActivityDto.Latitude)} must be between -90 and 90.");

        RuleFor(x => selector(x).Longitude)
            .NotEmpty().WithMessage($"{nameof(BaseActivityDto.Longitude)} is required.")
            .InclusiveBetween(-180, 180).WithMessage($"{nameof(BaseActivityDto.Longitude)} must be between -180 and 180.");

    }
}
