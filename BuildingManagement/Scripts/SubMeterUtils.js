function GetMeterTypesAndSpacesForSubMeter(subMeterId, meterId, operation) {
    GetMeterTypesForSubMeter(subMeterId, meterId, operation);
    GetSpacesForSubMeter(subMeterId, meterId, operation);
}

function GetMeterTypesForSubMeter(subMeterId, meterId, operation) {
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
                    "url": url + subMeterId + "&meterId=" + meterId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForSubMeter(subMeterId, meterId, operation) {
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
                    "url": url + subMeterId + "&meterId=" + meterId,
                    "dataType": "json"
                }
            }
        });
}
