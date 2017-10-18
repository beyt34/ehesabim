using System.ComponentModel.DataAnnotations;

namespace eHesabim.Framework.Localization {
    public class ResourceStringLengthAttribute : StringLengthAttribute {
        public ResourceStringLengthAttribute(string resourceKey, int maximumLength, int minLenght)
            : base(maximumLength) {
            MinimumLength = minLenght;
            ErrorMessage = resourceKey;
        }

        public ResourceStringLengthAttribute(string resourceKey, int maximumLength)
            : base(maximumLength) {
            ErrorMessage = resourceKey;
        }

        public override string FormatErrorMessage(string resourceKey) {
            var obj = Messages.ResourceManager.GetObject(ErrorMessage);
            return obj != null ? obj.ToString() : ErrorMessage;
        }
    }
}