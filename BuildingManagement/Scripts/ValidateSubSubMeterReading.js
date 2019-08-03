function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += IndexValidation();
    validationSummary += DateValidation();
    if (!document.getElementById("Initial").checked) {
        validationSummary += DiscountMonthValidation();
    }
    validationSummary += SubSubMeterValidation();
    validationSummary += MeterTypeValidation(true);
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
