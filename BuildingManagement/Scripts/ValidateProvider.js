function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += NameValidation();
    validationSummary += FiscalCodeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Name: document.getElementById("Name").value,
            FiscalCode: document.getElementById("FiscalCode").value,
            TradeRegister: document.getElementById("TradeRegister").value,
            Address: document.getElementById("Address").value,
            Phone: document.getElementById("Phone").value,
            Email: document.getElementById("Email").value,
            BankAccount: document.getElementById("BankAccount").value,
            Bank: document.getElementById("Bank").value,
            TVAPayer: document.getElementById("TVAPayer").checked
        };
        var url = "/Provider/Create";
        var indexUrl = "/Provider/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
