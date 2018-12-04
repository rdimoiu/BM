function GetMeterTypesForMeter(meterId, operation) {
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
                    "url": url + meterId,
                    "dataType": "json"
                }
            }
        });
}

function GetSpacesForMeter(meterId, clientId, operation) {
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
                    "url": url + meterId + "&clientId=" + clientId,
                    "dataType": "json"
                }
            }
        });
}
