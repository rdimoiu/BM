function GetSections(idClient, idSection, operation) {
    var sectionList = $("#SectionID");
    sectionList.empty();
    var firstSection = new Option("Select...", "0");
    sectionList.append(firstSection);

    var url;
    if (operation === "Create") {
        url = "../Level/GetSectionsByClient/";
    } else {
        url = "../GetSectionsByClient/";
    }
    
    $.ajax({
        url: url,
        data: { clientId: idClient, sectionId: idSection },
        cache: false,
        type: "POST",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var item = new Option(data[i].Text, data[i].Value, false, data[i].Selected);
                sectionList.append(item);
            }
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}
