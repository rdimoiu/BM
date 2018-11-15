function GetSpacesForService(modelId, invoiceId, operation) {
    var url;
    if (operation === "Create") {
        url = "../Service/GetSpacesTreeData/?serviceId=";
    } else {
        url = "../GetSpacesTreeData/?serviceId=";
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
                    "url": url + modelId + "&invoiceId=" + invoiceId,
                    "dataType": "json"
                }
            }
        });
}

function ServiceValidateAll(operation) {
    var validationSummary = "";
    validationSummary += InvoiceValidation();
    validationSummary += NameValidation();
    validationSummary += UnitValidation();
    validationSummary += DistributionModeValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitService(operation);
        return true;
    }
}

function InvoiceValidation() {
    var controlId = document.getElementById("InvoiceID");
    if (controlId.value === "") {
        return "The Invoice field is required." + "\n";
    } else {
        return "";
    }
}

function NameValidation() {
    var controlId = document.getElementById("Name");
    if (controlId.value === "") {
        return "The Name field is required." + "\n";
    } else {
        return "";
    }
}

function UnitValidation() {
    var controlId = document.getElementById("Unit");
    if (controlId.value === "") {
        return "The Unit field is required." + "\n";
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

function SubmitService(operation) {
    var serviceData;
    var url;
    if (operation === "Create") {
        serviceData = {
            PreviousPage: document.getElementById("PreviousPage").value,
            InvoiceID: document.getElementById("InvoiceID").value,
            Name: document.getElementById("Name").value,
            Quantity: document.getElementById("Quantity").value,
            Unit: document.getElementById("Unit").value,
            Price: document.getElementById("Price").value,
            //TVA: document.getElementById("TVA").value,
            //ValueWithoutTVA: document.getElementById("ValueWithoutTVA").value,
            QuotaTVA: document.getElementById("QuotaTVA").value,
            Fixed: document.getElementById("Fixed").checked,
            Inhabited: document.getElementById("Inhabited").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            Counted: document.getElementById("Counted").checked,
            ServiceSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "/Service/CreateService";
    } else {
        serviceData = {
            PreviousPage: document.getElementById("PreviousPage").value,
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            InvoiceID: document.getElementById("InvoiceID").value,
            Name: document.getElementById("Name").value,
            Quantity: document.getElementById("Quantity").value,
            Unit: document.getElementById("Unit").value,
            Price: document.getElementById("Price").value,
            //TVA: document.getElementById("TVA").value,
            //ValueWithoutTVA: document.getElementById("ValueWithoutTVA").value,
            QuotaTVA: document.getElementById("QuotaTVA").value,
            Fixed: document.getElementById("Fixed").checked,
            Inhabited: document.getElementById("Inhabited").checked,
            DistributionModeID: document.getElementById("DistributionModeID").value,
            Counted: document.getElementById("Counted").checked,
            ServiceSLSSelected: $("#spacesTree").jstree("get_selected")
        };
        url = "../EditService";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(serviceData),
        success: function () {
            window.location.href = "/Service/Index";
        },
        error: function (response) {
            alert("error : " + response);
        }
    });
}
