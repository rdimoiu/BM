function ClientValidateAll(operation) {
    var validationSummary = "";
    validationSummary += NameValidation();
    validationSummary += PhoneValidation();
    validationSummary += AddressValidation();
    validationSummary += ContactValidation();
    validationSummary += EmailValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        SubmitClient(operation);
        return true;
    }
}

function NameValidation() {
    var controlId = document.getElementById("Name");
    if (controlId.value === "") {
        return "The Name field is required." + "\n";
    } else {
        return "";
    }
}

function PhoneValidation() {
    var controlId = document.getElementById("Phone");
    if (controlId.value === "") {
        return "The Phone field is required." + "\n";
    } else {
        return "";
    }
}

function AddressValidation() {
    var controlId = document.getElementById("Address");
    if (controlId.value === "") {
        return "The Address field is required." + "\n";
    } else {
        return "";
    }
}

function ContactValidation() {
    var controlId = document.getElementById("Contact");
    if (controlId.value === "") {
        return "The Contact field is required." + "\n";
    } else {
        return "";
    }
}

function EmailValidation() {
    var controlId = document.getElementById("Email");
    if (controlId.value === "") {
        return "The Email field is required." + "\n";
    } else {
        return "";
    }
}

function SubmitClient(operation) {
    var clientData;
    var url;
    if (operation === "Create") {
        clientData = {
            Name: document.getElementById("Name").value,
            Phone: document.getElementById("Phone").value,
            Address: document.getElementById("Address").value,
            Contact: document.getElementById("Contact").value,
            Email: document.getElementById("Email").value
        };
        url = "/Client/CreateClient";
    } else {
        clientData = {
            //ID field is needed for edit
            ID: document.getElementById("ID").value,
            Name: document.getElementById("Name").value,
            Phone: document.getElementById("Phone").value,
            Address: document.getElementById("Address").value,
            Contact: document.getElementById("Contact").value,
            Email: document.getElementById("Email").value
        };
        url = "../EditClient";
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(clientData),
        success: function () {
            window.location.href = "/Client/Index";
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}