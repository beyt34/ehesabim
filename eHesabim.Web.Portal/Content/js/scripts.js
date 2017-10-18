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

function FormSearch(params, gridName) {
    var grid = $('#' + gridName).data('kendoGrid');
    grid.dataSource.page(1);  //new search. Set page size to 1
    grid.dataSource._data = [];
    grid.dataSource.read(params);
}

function DefaultButton(formName, buttonName) {
    var buttonKeys = { "EnterKey": 13 };
    $('#' + formName).bind("keydown", function (e) {
        if (e.which == buttonKeys.EnterKey) {
            $($('#' + buttonName)).click();
            return false;
        }
        return true;
    });
}

function OnlyNumberChars(evt) {
    evt = (evt) ? evt : window.event;
    var key = (evt.which) ? evt.which : evt.keyCode;

    if (evt.shiftKey || evt.altKey || evt.ctrlKey) {
        return true;
    }

    if (// Backspace and Tab and Enter
            key == 8 || key == 9 || key == 13 ||
        // Home and End            
            key == 35 || key == 36 ||
        // Left and Right Arrow
            key == 37 || key == 39 ||
        // Del/. and Ins and comma(,)
            key == 46 || key == 45 || key == 44) {
        return true;
    }

    // Numeric keypad
    if (key >= 48 && key <= 57) {
        return true;
    }

    return false;
}

function Numeric(field) {
    $("#" + field).keydown(function (event) {
        var key = event.charCode || event.keyCode || 0;
        if (event.shiftKey == true || event.altKey == true) {
            event.preventDefault();
        }

        if (!(key == 8 || key == 9 || key == 37 || key == 39 || key == 46 || (key >= 48 && key <= 57) || (key >= 96 && key <= 105))) {
            event.preventDefault();
        }
    });
}

function ParseToDecimal(price) {
    return parseFloat(price.replace(/\s/g, "").replace(",", "."));
}

function SetDecimalValue(amount, field) {
    if (!isNaN(amount)) {
        $(field).val(amount.toFixed(2).toString().replace(".", ","));
    }
    else {
        $(field).val('');
    }
}

function CloseWindow() {
    window.close();
}

function RefreshOpener() {
    window.opener.document.forms[0].submit();
    window.close();
}

function AjaxResult(data) {
    //$('#resultDiv').css({ 'color': data.Success ? '#008B00' : '#EE0000' });
    $('#resultDiv').removeClass().addClass(data.Success ? 'alert alert-success' : 'alert alert-danger');
    $('#resultDiv').html(data.Result);
}

function SetLeftMenu(li) {
    $('#' + li).addClass("active");
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    var winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    window.open(query, "_blank", winprops);
}

function ValidateForm(formName) {
    var form = $('#' + formName).removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
}

function ResetForm(formName) {
    $('#' + formName).each(function () {
        this.reset();
    });
}

function onGridDataBound() {
    var linkText = $('td[cmd=\'link\'] a').text();
    $('td[cmd=\'link\'] a').removeClass("k-button k-button-icontext").addClass("btn btn-outline btn-success").html("<i class='fa fa-link'></i>").attr("data-original-title", linkText);

    var loginText = $('td[cmd=\'login\'] a').text();
    $('td[cmd=\'login\'] a').removeClass("k-button k-button-icontext").addClass("btn btn-outline btn-primary").html("<i class='fa fa-user'></i>").attr("data-original-title", loginText);

    var editText = $('td[cmd=\'edit\'] a').text();
    $('td[cmd=\'edit\'] a').removeClass("k-button k-button-icontext").addClass("btn btn-outline btn-warning").html("<i class='fa fa-list'></i>").attr("data-original-title", editText);

    var deleteText = $('td[cmd=\'delete\'] a').text();
    $('td[cmd=\'delete\'] a').removeClass("k-button k-button-icontext").addClass("btn btn-outline btn-danger").html("<i class='fa fa-times'></i>").attr("data-original-title", deleteText);
}

function closeModal(kendoWindow) {
    $('#' + kendoWindow).data("kendoWindow").close();
}

function openModal(kendoWindow, url, id, windowType) {
    var wnd = $('#' + kendoWindow).kendoWindow({
        content: {
            url: url,
            data: { id: '' + id + '' }
        }
    }).data("kendoWindow");

    switch (windowType) {
        case "sm":
            wnd.wrapper.addClass("col-sm-6 col-xs-10 max-width-400");
            break;

        case "md":
            wnd.wrapper.addClass("col-sm-8 col-xs-10 max-width-650");
            break;

        default:
    }

    wnd.center();
    wnd.open();
}

function BlockUI() {
    $.blockUI({ message: 'Lutfen Bekleyiniz' });
}

function UnBlockUI() {
    $.unblockUI({ fadeOut: 200 });
}

function GetCountyList(cityControlName, countyControlName) {
    $.ajax({
        url: "/Common/GetCountyList",
        data: { cityId: $('#' + cityControlName).val() },
        dataType: "json",
        type: "POST",
        success: function (data) {
            var items = "";
            $.each(data, function (i, item) {
                items += "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
            });
            $('#' + countyControlName).html(items);
        }
    });
}

function GetBankCreditCardPeriodList(bankCreditCardControlName, bankCreditCardPeriodControlName) {
    $.ajax({
        url: "/Common/GetBankCreditCardPeriodList",
        data: { bankCreditCardId: $('#' + bankCreditCardControlName).val() },
        dataType: "json",
        type: "POST",
        success: function (data) {
            var items = "";
            $.each(data, function (i, item) {
                items += "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
            });
            $('#' + bankCreditCardPeriodControlName).html(items);
        }
    });
}