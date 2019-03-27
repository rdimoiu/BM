function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += IndexValidation();
    validationSummary += DateValidation();
    validationSummary += SubMeterValidation();
    validationSummary += MeterTypeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Index: document.getElementById("Index").value,
            Date: document.getElementById("Date").value,
            DiscountMonth: document.getElementById("DiscountMonth").value,
            Initial: document.getElementById("Initial").checked,
            Estimated: document.getElementById("Estimated").checked,
            SubMeterID: document.getElementById("SubMeterID").value,
            MeterTypeID: document.getElementById("MeterTypeID").value
        };
        var url = "/SubMeterReading/Create";
        var indexUrl = "/SubMeterReading/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
