function LevelValidateAll(operation) {
    var validationSummary = "";
    validationSummary += SectionValidation();
    validationSummary += NumberValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitLevel(operation);
        return true;
    }
}

function SectionValidation() {
    var controlId = document.getElementById("SectionID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Section field is required." + "\n";
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

function SubmitLevel(operation) {
    var levelData;
    var url;
    if (operation === "Create") {
        levelData = {
            Number: document.getElementById("Number").value,
            SectionID: document.getElementById("SectionID").value
        };
        url = "/Level/CreateLevel";
    } else {
        levelData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Number: document.getElementById("Number").value,
            SectionID: document.getElementById("SectionID").value
        };
        url = "../EditLevel";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(levelData),
        success: function () {
            window.location.href = "/Level/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}

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