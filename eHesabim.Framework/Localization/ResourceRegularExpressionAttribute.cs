using System.ComponentModel.DataAnnotations;

namespace eHesabim.Framework.Localization {
    public class ResourceRegularExpressionAttribute : RegularExpressionAttribute {
        public ResourceRegularExpressionAttribute(string resourceKey, string pattern)
            : base(pattern) {
            ErrorMessage = resourceKey;
        }

        public override string FormatErrorMessage(string resourceKey) {
            var obj = Messages.ResourceManager.GetObject(ErrorMessage);
            return obj != null ? obj.ToString() : ErrorMessage;
        }
    }
}
