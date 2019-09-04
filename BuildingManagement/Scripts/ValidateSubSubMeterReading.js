function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += IndexValidation();
    validationSummary += DateValidation();
    if (!document.getElementById("Initial").checked) {
        validationSummary += DiscountMonthValidation();
    }
    validationSummary += SubSubMeterValidation();
    validationSummary += MeterTypeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var dm = new Date(document.getElementById("DiscountMonth").value);
        var firstDay = new Date(dm.getFullYear(), dm.getMonth(), 1);
        var data = {
            Index: document.getElementById("Index").value,
            Date: document.getElementById("Date").value,
            DiscountMonth: firstDay,
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
