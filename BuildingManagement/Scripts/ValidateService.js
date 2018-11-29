﻿function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += ClientValidation();
    validationSummary += NumberValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            ClientID: document.getElementById("ClientID").value,
            Number: document.getElementById("Number").value
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
