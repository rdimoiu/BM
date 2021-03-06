﻿function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += ClientValidation();
    validationSummary += ProviderValidation();
    validationSummary += InvoiceTypeValidation();
    validationSummary += NumberValidation();
    validationSummary += DateValidation();
    validationSummary += DueDateValidation();
    validationSummary += CheckQuantityValidation();
    validationSummary += CheckTotalValueWithoutTVAValidation();
    validationSummary += CheckTotalTVAValidation();
    validationSummary += DiscountMonthValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var dm = new Date(document.getElementById("DiscountMonth").value);
        var firstDay = new Date(dm.getFullYear(), dm.getMonth(), 1);
        var data = {
            ClientID: document.getElementById("ClientID").value,
            ProviderID: document.getElementById("ProviderID").value,
            InvoiceTypeID: document.getElementById("InvoiceTypeID").value,
            Number: document.getElementById("Number").value,
            Date: document.getElementById("Date").value,
            DueDate: document.getElementById("DueDate").value,
            CheckQuantity: document.getElementById("CheckQuantity").value,
            CheckTotalValueWithoutTVA: document.getElementById("CheckTotalValueWithoutTVA").value,
            CheckTotalTVA: document.getElementById("CheckTotalTVA").value,
            DiscountMonth: firstDay,
            //this is only for navigation
            PreviousPage: document.getElementById("PreviousPage").value
        };
        var url = "/Invoice/Create";
        var indexUrl = PreviousPage.value;
        if (indexUrl != "/Invoice/Index") {
            indexUrl += "?discountMonth=" + DiscountMonth.value;
            indexUrl += "&clientId=" + ClientID.value;
            indexUrl += "&providerId=" + ProviderID.value;
        }
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}

