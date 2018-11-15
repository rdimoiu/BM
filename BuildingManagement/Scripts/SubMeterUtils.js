function GetMeterTypesAndSpacesForSubMeter(modelId, meterId, operation) {
    GetMeterTypesForSubMeter(modelId, meterId, operation);
    GetSpacesForSubMeter(modelId, meterId, operation);
}

function GetMeterTypesForSubMeter(modelId, meterId, operation) {
    var url;
    if (operation === "Create") {
        url = "../SubMeter/GetMeterTypesTreeData/?subMeterId=";
    } else {
        url = "../GetMeterTypesTreeData/?subMeterId=";
    }
    $("#meterTypesTree").jstree("destroy").empty();
    $("#meterTypesTree")
        .jstree({
            "plugins": ["defaults", "checkbox"],
            "core": {
                "data": {
                    "themes": {
                        "responsive": true
                    },
                    "url": url + modelId + "&meterId=" + meterId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForSubMeter(modelId, meterId, operation) {
    var url;
    if (operation === "Create") {
        url = "../SubMeter/GetSpacesTreeData/?subMeterId=";
    } else {
        url = "../GetSpacesTreeData/?subMeterId=";
    }
    $("#spacesTree").jstree("destroy").empty();
    $("#spacesTree")
        .jstree({
            "plugins": ["defaults", "checkbox"],
            "core": {
                "data": {
                    "themes": {
                        "responsive": true
                    },
                    "url": url + modelId + "&meterId=" + meterId,
                    "dataType": "json"
                }
            }
        });
}

function SubMeterValidateAll(operation) {
    var validationSummary = "";
    validationSummary += CodeValidation();
    validationSummary += InitialIndexValidation();
    validationSummary += DistributionModeValidation();
    validationSummary += MeterValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitSubMeter(operation);
        return true;
    }
}

function CodeValidation() {
    var controlId = document.getElementById("Code");
    if (controlId.value === "") {
        return "The Code field is required." + "\n";
    } else {
        return "";
    }
}

function InitialIndexValidation() {
    var controlId = document.getElementById("InitialIndex");
    if (controlId.value === "") {
        return "The InitialIndex field is required." + "\n";
    } else {
        return "";
    }
}

function DistributionModeValidation() {
    var controlId = document.getElementById("DistributionModeID");
    if (controlId.value === "") {
        return "The DistributionMode field is required." + "\n";
    } else {
        return "";
    }
}

function MeterValidation() {
    var controlId = document.getElementById("MeterID");
    if (controlId.value === "") {
        return "The Meter field is required." + "\n";
    } else {
        return "";
    }
}

function SubmitSubMeter(operation) {
    var subMeterData;
    var url;
    if (operation === "Create") {
        subMeterData = {
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            MeterID: document.getElementById("MeterID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "/SubMeter/CreateSubMeter";
    } else {
        subMeterData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            MeterID: document.getElementById("MeterID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "../EditSubMeter";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(subMeterData),
        success: function () {
            window.location.href = "/SubMeter/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}