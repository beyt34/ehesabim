// jQuery validation fixes
$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
};

$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
};

$.validator.methods.date = function (value, element) {
    return this.optional(element) || /^\d{1,2}[\/\.]\d{1,2}[\/\.]\d{4}$/.test(value) || /^\d{1,2}[\/\.]\d{1,2}[\/\.]\d{4}[\s+]\d{2}:\d{2}$/.test(value);
};

//soner
function FormSearch(params, gridName) {
    var grid = $('#' + gridName).data('kendoGrid');
    grid.dataSource.page(1);  //new search. Set page size to 1
    grid.dataSource._data = [];
    grid.dataSource.read(params);
}

//soner
function DefaultButton(formName, buttonName) {
    var buttonKeys = { "EnterKey": 13 };
    $('#' + formName).bind("keydown", function (e) {
        if (e.which == buttonKeys.EnterKey) {
            $($('#' + buttonName)).click();
            return false;
        }
    });
}

function OnlyNumberChars(evt, allowOne) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (allowOne == false && charCode == 49) {
        return false;
    }
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        if (charCode == 44) {
            return true;
        }
        if (allowOne == "false") {
            return false;
        }
        return false;
    }

    return true;
}

//soner
function CloseWindow() {
    window.close();
}

//soner
function AjaxResult(data) {
    $('#resultDiv').css({ 'color': data.Success ? '#008B00' : '#EE0000' });
    $('#resultDiv').html(data.Result);
}

//soner
function OpenPasswordWindow(userId) {
    var wnd = $("#PasswordChangeDeleteWindow").kendoWindow({
        title: "Accounts",
        content: {
            url: "/User/PasswordChange",
            data: { userId: '' + userId + '' }
        }
    }).data("kendoWindow");

    wnd.center();
    wnd.open();
}
//soner
function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    var winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    window.open(query, "_blank", winprops);
}

//soner
function ValidateForm(formName) {
    var form = $('#' + formName).removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
}