function GetMeterTypesAndSpacesForSubSubMeter(subSubMeterId, subMeterId, operation) {
    GetMeterTypesForSubSubMeter(subSubMeterId, subMeterId, operation);
    GetSpacesForSubSubMeter(subSubMeterId, subMeterId, operation);
}

function GetMeterTypesForSubSubMeter(subSubMeterId, subMeterId, operation) {
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
                    "url": url + subSubMeterId + "&subMeterId=" + subMeterId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForSubSubMeter(subSubMeterId, subMeterId, operation) {
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
                    "url": url + subSubMeterId + "&subMeterId=" + subMeterId,
                    "dataType": "json"
                }
            }
        });
}
