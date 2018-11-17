function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += TypeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Type: document.getElementById("Type").value
        };
        var url = "/InvoiceType/Create";
        var indexUrl = "/InvoiceType/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
