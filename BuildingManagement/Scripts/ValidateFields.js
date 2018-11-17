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

function ModeValidation() {
    var controlId = document.getElementById("Mode");
    if (controlId.value === "") {
        return "The Mode field is required." + "\n";
    } else {
        return "";
    }
}
