function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += CodeValidation();
    validationSummary += DistributionModeValidation();
    validationSummary += ClientValidation();
    validationSummary += MeterTypesValidation();
    validationSummary += SpacesValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            ClientID: document.getElementById("ClientID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        var url = "/Meter/Create";
        var indexUrl = "/MeterReading/Create?meterCode=" + Code.value;
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
            indexUrl = "/Meter/Index";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}
