function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += IndexValidation();
    validationSummary += DateValidation();
    validationSummary += SubSubMeterValidation();
    validationSummary += MeterTypeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Index: document.getElementById("Index").value,
            Date: document.getElementById("Date").value,
            SubSubMeterID: document.getElementById("SubSubMeterID").value,
            MeterTypeID: document.getElementById("MeterTypeID").value
        };
        var url = "/SubSubMeterReading/Create";
        var indexUrl = "/SubSubMeterReading/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
