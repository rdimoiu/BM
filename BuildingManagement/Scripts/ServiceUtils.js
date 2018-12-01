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
