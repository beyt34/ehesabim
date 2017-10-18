namespace eHesabim.Services.Models {
    public class EmailTemplateDataModel : BaseDataModel<int> {
        public string EmailTemplateName { get; set; }

        public string EmailTemplateSubject { get; set; }

        public string EmailTemplateBody { get; set; }

        public int? EmailTemplatePriority { get; set; }

        public int EmailTemplateEmailAccountId { get; set; }
    }
}
