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

function CountedOnChangeEvent() {
    var counted = document.getElementById("Counted");
    var distributionModeGroup = document.getElementById("DistributionModeGroup");
    var meterTypeGroup = document.getElementById("MeterTypeGroup");
    if (counted.checked) {
        distributionModeGroup.style.display = "none";
        meterTypeGroup.style.display = "initial";
    } else {
        distributionModeGroup.style.display = "initial";
        meterTypeGroup.style.display = "none";
    }
};

function DisableFieldsForRestService() {
    var parentID = document.getElementById("ParentID").value;
    if (parentID > 0) {
        document.getElementById("InvoiceID").disabled = true;
        document.getElementById("Name").disabled = true;
        document.getElementById("Quantity").disabled = true;
        document.getElementById("Unit").disabled = true;
        document.getElementById("Price").disabled = true;
        document.getElementById("QuotaTVA").disabled = true;
        document.getElementById("Fixed").disabled = true;
        document.getElementById("Inhabited").disabled = true;
        document.getElementById("Counted").disabled = true;
    }
}

