function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += InvoiceValidation();
    validationSummary += NameValidation();
    validationSummary += UnitValidation();
    validationSummary += DistributionModeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            PreviousPage: document.getElementById("PreviousPage").value,
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
            Counted: document.getElementById("Counted").checked,
            ServiceSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        var url = "/Service/Create";
        var indexUrl = "/Service/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
