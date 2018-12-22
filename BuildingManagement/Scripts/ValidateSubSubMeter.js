﻿function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += CodeValidation();
    validationSummary += InitialIndexValidation();
    validationSummary += DistributionModeValidation();
    validationSummary += SubMeterValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            SubMeterID: document.getElementById("SubMeterID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        var url = "/SubSubMeter/Create";
        var indexUrl = "/SubSubMeter/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}