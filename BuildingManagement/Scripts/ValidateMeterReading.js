function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += IndexValidation();
    validationSummary += DateValidation();
    validationSummary += MeterValidation();
    validationSummary += MeterTypeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Index: document.getElementById("Index").value,
            Date: document.getElementById("Date").value,
            MeterID: document.getElementById("MeterID").value,
            MeterTypeID: document.getElementById("MeterTypeID").value
        };
        var url = "/MeterReading/Create";
        var indexUrl = "/MeterReading/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}