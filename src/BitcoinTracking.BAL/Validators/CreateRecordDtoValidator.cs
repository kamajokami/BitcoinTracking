using FluentValidation;
using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.Shared.Constants;

namespace BitcoinTracking.BAL.Validators
{
    /// <summary>
    /// Validator for CreateRecordDto
    /// Ensures all required fields are valid before saving to database
    /// </summary>
    public class CreateRecordDtoValidator : AbstractValidator<CreateRecordDto>
    {
        public CreateRecordDtoValidator()
        {
            // Bitcoin price in EUR validation
            RuleFor(x => x.PriceBtcEur)
                .GreaterThan(0)
                .WithMessage("Bitcoin cena v EUR musí být větší než 0.");

            // EUR/CZK exchange rate validation
            RuleFor(x => x.ExchangeRateEurCzk)
                .GreaterThan(0)
                .WithMessage("Kurz EUR/CZK musí být větší než 0.");

            // Bitcoin price in CZK validation
            RuleFor(x => x.PriceBtcCzk)
                .GreaterThan(0)
                .WithMessage("Bitcoin cena v CZK musí být větší než 0.");

            // Note - REQUIRED by assignment
            RuleFor(x => x.Note)
                .NotEmpty()
                .WithMessage("Poznámka je povinná.")
                .MaximumLength(AppConstants.Validation.MaxNoteLength)
                .WithMessage($"Poznámka může mít maximálně {AppConstants.Validation.MaxNoteLength} znaků.");
        }
    }
}
