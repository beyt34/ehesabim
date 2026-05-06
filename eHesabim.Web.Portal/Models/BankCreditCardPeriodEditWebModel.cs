using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models
{
    public class BankCreditCardPeriodEditWebModel : BaseWebModel<Guid>, IValidatableObject
    {
        [ResourceRequired("RequiredBankCreditCard")]
        public Guid BankCreditCardId { get; set; }

        public SelectList BankCreditCardList { get; set; }

        [ResourceRequired("RequiredStartDate")]
        public DateTime StartDate { get; set; }

        [ResourceRequired("RequiredEndDate")]
        public DateTime EndDate { get; set; }

        [ResourceRequired("RequiredPaymentDate")]
        public DateTime PaymentDate { get; set; }

        public bool SetExpense { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate != DateTime.MinValue && StartDate != DateTime.MinValue && EndDate <= StartDate)
            {
                yield return new ValidationResult(Messages.EndDateMustBeAfterStartDate, new[] { nameof(EndDate) });
            }

            if (PaymentDate != DateTime.MinValue && EndDate != DateTime.MinValue && PaymentDate <= EndDate)
            {
                yield return new ValidationResult(Messages.PaymentDateMustBeAfterEndDate, new[] { nameof(PaymentDate) });
            }
        }
    }
}