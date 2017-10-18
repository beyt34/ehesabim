using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace eHesabim.Framework.Controllers {
    public enum FormValueRequirement {
        Equal,

        StartsWith
    }

    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute {
        private readonly string[] submitButtonNames;

        private readonly FormValueRequirement requirement;

        public FormValueRequiredAttribute(params string[] submitButtonNames) :
            this(FormValueRequirement.Equal, submitButtonNames) {
        }

        public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames) {
            ////at least one submit button should be found
            this.submitButtonNames = submitButtonNames;
            this.requirement = requirement;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            foreach (string buttonName in submitButtonNames) {
                try {
                    string value = string.Empty;
                    switch (requirement) {
                        case FormValueRequirement.Equal: {
                                ////do not iterate because "Invalid request" exception can be thrown
                                value = controllerContext.HttpContext.Request.Form[buttonName];
                            }

                            break;
                        case FormValueRequirement.StartsWith: {
                                foreach (var formValue in controllerContext.HttpContext.Request.Form.AllKeys) {
                                    if (formValue.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase)) {
                                        value = controllerContext.HttpContext.Request.Form[formValue];
                                        break;
                                    }
                                }
                            }

                            break;
                    }

                    if (!string.IsNullOrEmpty(value)) {
                        return true;
                    }
                }
                catch (Exception exc) {
                    ////try-catch to ensure that 
                    Debug.WriteLine(exc.Message);
                }
            }

            return false;
        }
    }
}