function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += ClientValidation();
    validationSummary += CNPValidation();
    validationSummary += FiscalCodeValidation();
    validationSummary += NameValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Name: document.getElementById("Name").value,
            Phone: document.getElementById("Phone").value,
            Country: document.getElementById("Country").value,
            State: document.getElementById("State").value,
            City: document.getElementById("City").value,
            Street: document.getElementById("Street").value,
            Contact: document.getElementById("Contact").value,
            Email: document.getElementById("Email").value,
            IBAN: document.getElementById("IBAN").value,
            Bank: document.getElementById("Bank").value,
            CNP: document.getElementById("CNP").value,
            FiscalCode: document.getElementById("FiscalCode").value,
            ClientID: document.getElementById("ClientID").value
        };
        var url = "/SubClient/Create";
        var indexUrl = "/SubClient/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}