function GetSections(idClient, idSection, operation) {
    var levelList = $("#LevelID");
    levelList.empty();
    var firstLevel = new Option("Select...", "0");
    levelList.append(firstLevel);

    var sectionList = $("#SectionID");
    sectionList.empty();
    var firstSection = new Option("Select...", "0");
    sectionList.append(firstSection);

    var url;
    if (operation === "Create") {
        url = "../Space/GetSectionsByClient/";
    } else {
        url = "../GetSectionsByClient/";
    }
    
    $.ajax({
        url: url,
        data: { clientId: idClient, sectionId: idSection },
        cache: false,
        type: "POST",
        success: function(data) {
            for (var i = 0; i < data.length; i++) {
                var item = new Option(data[i].Text, data[i].Value, false, data[i].Selected);
                sectionList.append(item);
            }
            if (idClient === "") {
                GetLevels("", "", operation);
            }
        },
        error: function(reponse) {
            alert("error : " + reponse);
        }
    });
}

function GetLevels(idSection, idLevel, operation) {
    var levelList = $("#LevelID");
    levelList.empty();
    var firstLevel = new Option("Select...", "0");
    levelList.append(firstLevel);

    var url;
    if (operation === "Create") {
        url = "../Space/GetLevelsBySection/";
    } else {
        url = "../GetLevelsBySection/";
    }

    $.ajax({
        url: url,
        data: { sectionId: idSection, levelId: idLevel },
        cache: false,
        type: "POST",
        success: function(data) {
            for (var i = 0; i < data.length; i++) {
                var item = new Option(data[i].Text, data[i].Value, false, data[i].Selected);
                levelList.append(item);
            }
        },
        error: function(reponse) {
            alert("error : " + reponse);
        }
    });
}
