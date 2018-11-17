function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += NameValidation();
    validationSummary += PhoneValidation();
    validationSummary += AddressValidation();
    validationSummary += ContactValidation();
    validationSummary += EmailValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Name: document.getElementById("Name").value,
            Phone: document.getElementById("Phone").value,
            Address: document.getElementById("Address").value,
            Contact: document.getElementById("Contact").value,
            Email: document.getElementById("Email").value
        };
        var url = "/Client/Create";
        var indexUrl = "/Client/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
