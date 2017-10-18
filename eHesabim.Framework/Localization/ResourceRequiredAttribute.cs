using System.ComponentModel.DataAnnotations;

namespace eHesabim.Framework.Localization {
    public class ResourceRequiredAttribute : RequiredAttribute {
        public ResourceRequiredAttribute(string resourceKey) {
            ErrorMessage = resourceKey;
        }

        public override string FormatErrorMessage(string resourceKey) {
            var obj = Messages.ResourceManager.GetObject(ErrorMessage);
            return obj != null ? obj.ToString() : ErrorMessage;
        }
    }
}