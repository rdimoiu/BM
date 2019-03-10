function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += InvoiceValidation();
    validationSummary += NameValidation();
    validationSummary += UnitValidation();
    validationSummary += QuantityValidation();
    validationSummary += PriceValidation();
    validationSummary += QuotaTVAValidation();
    validationSummary += DistributionModeOrMeterTypeValidation();
    validationSummary += SpacesValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            InvoiceID: document.getElementById("InvoiceID").value,
            Name: document.getElementById("Name").value,
            Quantity: document.getElementById("Quantity").value,
            Unit: document.getElementById("Unit").value,
            Price: document.getElementById("Price").value,
            //TVA: document.getElementById("TVA").value,
            //ValueWithoutTVA: document.getElementById("ValueWithoutTVA").value,
            QuotaTVA: document.getElementById("QuotaTVA").value,
            Fixed: document.getElementById("Fixed").checked,
            Inhabited: document.getElementById("Inhabited").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            MeterTypeID: document.getElementById("MeterTypeID").value,
            Counted: document.getElementById("Counted").checked,
            ServiceSLSSelected: $("#spacesTree").jstree("get_selected"),
            //this is only for navigation
            PreviousPage: document.getElementById("PreviousPage").value
        };
        if (data.Counted) {
            data.DistributionModeID = null;
        } else {
            data.MeterTypeID = null;
        }
        var url = "/Service/Create";
        var indexUrl = PreviousPage.value;
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
