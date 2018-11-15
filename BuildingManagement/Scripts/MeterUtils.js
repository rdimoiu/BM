function GetMeterTypesForMeter(modelId, operation) {
    var url;
    if (operation === "Create") {
        url = "../Meter/GetMeterTypesTreeData/?meterId=";
    } else {
        url = "../GetMeterTypesTreeData/?meterId=";
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
                    "url": url + modelId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForMeter(modelId, clientId, operation) {
    var url;
    if (operation === "Create") {
        url = "../Meter/GetSpacesTreeData/?meterId=";
    } else {
        url = "../GetSpacesTreeData/?meterId=";
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
                    "url": url + modelId + "&clientId=" + clientId,
                    "dataType": "json"
                }
            }
        });
}

function MeterValidateAll(operation) {
    var validationSummary = "";
    validationSummary += CodeValidation();
    validationSummary += InitialIndexValidation();
    validationSummary += DistributionModeValidation();
    validationSummary += ClientValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitMeter(operation);
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

function ClientValidation() {
    var controlId = document.getElementById("ClientID");
    if (controlId.value === "") {
        return "The Client field is required." + "\n";
    } else {
        return "";
    }
}

function SubmitMeter(operation) {
    var meterData;
    var url;
    if (operation === "Create") {
        meterData = {
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            ClientID: document.getElementById("ClientID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "/Meter/CreateMeter";
    } else {
        meterData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Code: document.getElementById("Code").value,
            Details: document.getElementById("Details").value,
            InitialIndex: document.getElementById("InitialIndex").value,
            Defect: document.getElementById("Defect").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            ClientID: document.getElementById("ClientID").value,
            MeterTypesSelected: $("#meterTypesTree").jstree("get_selected"),
            MeterSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "../EditMeter";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(meterData),
        success: function () {
            window.location.href = "/Meter/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}