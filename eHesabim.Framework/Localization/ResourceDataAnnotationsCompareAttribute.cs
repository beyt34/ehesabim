namespace eHesabim.Framework.Localization {
    public class ResourceDataAnnotationsCompareAttribute : System.ComponentModel.DataAnnotations.CompareAttribute {
        public ResourceDataAnnotationsCompareAttribute(string otherProperty, string resourceKey)
            : base(otherProperty) {
            ErrorMessage = resourceKey;
        }

        public override string FormatErrorMessage(string resourceKey) {
            var obj = Messages.ResourceManager.GetObject(ErrorMessage);
            return obj != null ? obj.ToString() : ErrorMessage;
        }
    }
}
