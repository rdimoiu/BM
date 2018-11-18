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
        var url = "/MeterType/Create";
        var indexUrl = "/MeterType/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
