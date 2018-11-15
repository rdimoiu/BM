function SpaceValidateAll(operation) {
    var validationSummary = "";
    validationSummary += LevelValidation();
    validationSummary += SpaceTypeValidation();
    validationSummary += SubClientValidation();
    validationSummary += NumberValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitSpace(operation);
        return true;
    }
}

function LevelValidation() {
    var controlId = document.getElementById("LevelID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Level field is required." + "\n";
    } else {
        return "";
    }
}

function SpaceTypeValidation() {
    var controlId = document.getElementById("SpaceTypeID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The SpaceType field is required." + "\n";
    } else {
        return "";
    }
}

function SubClientValidation() {
    var controlId = document.getElementById("SubClientID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The SubClient field is required." + "\n";
    } else {
        return "";
    }
}

function NumberValidation() {
    var controlId = document.getElementById("Number");
    if (controlId.value === "") {
        return "The Number field is required." + "\n";
    } else {
        return "";
    }
}

function SubmitSpace(operation) {
    var spaceData;
    var url;
    if (operation === "Create") {
        spaceData = {
            Number: document.getElementById("Number").value,
            Surface: document.getElementById("Surface").value,
            People: document.getElementById("People").value,
            Inhabited: document.getElementById("Inhabited").value,
            SpaceTypeID: document.getElementById("SpaceTypeID").value,
            SubClientID: document.getElementById("SubClientID").value,
            LevelID: document.getElementById("LevelID").value
        };
        url = "/Space/CreateSpace";
    } else {
        spaceData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Number: document.getElementById("Number").value,
            Surface: document.getElementById("Surface").value,
            People: document.getElementById("People").value,
            Inhabited: document.getElementById("Inhabited").value,
            SpaceTypeID: document.getElementById("SpaceTypeID").value,
            SubClientID: document.getElementById("SubClientID").value,
            LevelID: document.getElementById("LevelID").value
        };
        url = "../EditSpace";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(spaceData),
        success: function () {
            window.location.href = "/Space/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}

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