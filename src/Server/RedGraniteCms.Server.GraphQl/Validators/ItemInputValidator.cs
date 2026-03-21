using FluentValidation;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.GraphQl.Types;

namespace RedGraniteCms.Server.GraphQl.Validators;

/// <summary>
/// FluentValidation validator for ItemInput.
/// </summary>
public class ItemInputValidator : AbstractValidator<ItemInput>
{
    public ItemInputValidator()
    {
        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .MaximumLength(100).WithMessage("Content type must not exceed 100 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

        RuleFor(x => x.Summary)
            .MaximumLength(2000).WithMessage("Summary must not exceed 2000 characters.");

        RuleFor(x => x.Language)
            .MaximumLength(10).WithMessage("Language code must not exceed 10 characters.");

        RuleFor(x => x.Slug)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug must be lowercase alphanumeric words separated by hyphens.");

        RuleFor(x => x.Status)
            .Must(s => s is null || Enum.TryParse<ItemStatus>(s, true, out _))
            .WithMessage("Invalid status value. Allowed: Draft, Published, Archived.");

        RuleFor(x => x.Visibility)
            .Must(v => v is null || Enum.TryParse<ItemVisibility>(v, true, out _))
            .WithMessage("Invalid visibility value. Allowed: Public, Authenticated, Restricted, Private.");
    }
}

/// <summary>
/// Validator for update operations that require an ID.
/// </summary>
public class ItemInputUpdateValidator : AbstractValidator<ItemInput>
{
    public ItemInputUpdateValidator()
    {
        Include(new ItemInputValidator());

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item ID is required for updates.");
    }
}
