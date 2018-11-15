function GetMeterTypesAndSpacesForSubSubMeter(modelId, subMeterId, operation) {
    GetMeterTypesForSubSubMeter(modelId, subMeterId, operation);
    GetSpacesForSubSubMeter(modelId, subMeterId, operation);
}

function GetMeterTypesForSubSubMeter(modelId, subMeterId, operation) {
    var url;
    if (operation === "Create") {
        url = "../SubSubMeter/GetMeterTypesTreeData/?subSubMeterId=";
    } else {
        url = "../GetMeterTypesTreeData/?subSubMeterId=";
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
                    "url": url + modelId + "&subMeterId=" + subMeterId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForSubSubMeter(modelId, subMeterId, operation) {
    var url;
    if (operation === "Create") {
        url = "../SubSubMeter/GetSpacesTreeData/?subSubMeterId=";
    } else {
        url = "../GetSpacesTreeData/?subSubMeterId=";
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
                    "url": url + modelId + "&subMeterId=" + subMeterId,
                    "dataType": "json"
                }
            }
        });
}

function SubSubMeterValidateAll(operation) {
    var validationSummary = "";
    validationSummary += CodeValidation();
    validationSummary += InitialIndexValidation();
    validationSummary += DistributionModeValidation();
    validationSummary += SubMeterValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitSubSubMeter(operation);
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

function SubMeterValidation() {
    var controlId = document.getElementById("SubMeterID");
    if (controlId.value === "") {
        return "The SubMeter field is required." + "\n";
    } else {
        return "";
    }
}

function SubmitSubSubMeter(operation) {
    var subSubMeterData;
    var url;
    if (operation === "Create") {
        subSubMeterData = {
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            SubMeterID: document.getElementById("SubMeterID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "/SubSubMeter/CreateSubSubMeter";
    } else {
        subSubMeterData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            SubMeterID: document.getElementById("SubMeterID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "../EditSubSubMeter";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(subSubMeterData),
        success: function () {
            window.location.href = "/SubSubMeter/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}