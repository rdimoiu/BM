function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += ModeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Mode: document.getElementById("Mode").value
        };
        var url = "/DistributionMode/Create";
        var indexUrl = "/DistributionMode/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
