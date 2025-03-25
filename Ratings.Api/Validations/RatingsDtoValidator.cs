using FluentValidation;
using Ratings.Api.DTOs;

namespace Ratings.Api.Validations;

public class RatingDtoValidator : AbstractValidator<RatingDto>
{
    public RatingDtoValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ProviderId)
            .NotNull().WithMessage("ProvideId cannot be null")
            .NotEmpty().WithMessage("ProviderId cannot be empty.");

        RuleFor(x => x.CustomerId)
            .NotNull().WithMessage("CustomerId cannot be null")
            .NotEmpty().WithMessage("CustomerId cannot be empty.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 10)
            .WithMessage("Score must be between 1 and 10. You entered {PropertyValue}!");

        RuleFor(x => x.Comment)
            .MinimumLength(5)
            .WithMessage("Comment must be between 5 and 500 characters!")
            .MaximumLength(500)
            .WithMessage("Comment must be between 5 and 500 characters!")
            .Matches(@"^[a-zA-Z0-9öçşğüıÖÇŞĞÜİ .\-]*$")
            .WithMessage("Comment can only contain alphanumeric and following characters : .\\-");
    }
}