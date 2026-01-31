using FluentValidation;
using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.Shared.Constants;


namespace BitcoinTracking.BAL.Validators
{
    /// <summary>
    /// Validator for UpdateNoteDto
    /// Ensures note is valid when updating a saved record
    /// </summary>
    public class UpdateNoteDtoValidator : AbstractValidator<UpdateRecordNoteDto>
    {
        public UpdateNoteDtoValidator()
        {
            // Record ID validation
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("ID záznamu musí být větší než 0.");

            // Note - REQUIRED by assignment
            RuleFor(x => x.Note)
                .NotEmpty()
                .WithMessage("Poznámka je povinná.")
                .MaximumLength(AppConstants.Validation.MaxNoteLength)
                .WithMessage($"Poznámka může mít maximálně {AppConstants.Validation.MaxNoteLength} znaků.");
        }
    }
}
