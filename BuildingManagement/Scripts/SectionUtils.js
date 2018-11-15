function SectionValidateAll(operation) {
    var validationSummary = "";
    validationSummary += ClientValidation();
    validationSummary += NumberValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitSection(operation);
        return true;
    }
}

function ClientValidation() {
    var controlId = document.getElementById("ClientID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Client field is required." + "\n";
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

function SubmitSection(operation) {
    var sectionData;
    var url;
    if (operation === "Create") {
        sectionData = {
            Number: document.getElementById("Number").value,
            ClientID: document.getElementById("ClientID").value
        };
        url = "/Section/CreateSection";
    } else {
        sectionData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Number: document.getElementById("Number").value,
            ClientID: document.getElementById("ClientID").value
        };
        url = "../EditSection";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(sectionData),
        success: function () {
            window.location.href = "/Section/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}