function ValidateAll(operation) {
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
            DiscountMonth: document.getElementById("DiscountMonth").value,
            //this is only for navigation
            PreviousPage: document.getElementById("PreviousPage").value
        };
        var url = "/Invoice/Create";
        var indexUrl = "/Invoice/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
