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

function ClientValidation() {
    var controlId = document.getElementById("ClientID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Client field is required." + "\n";
    } else {
        return "";
    }
}

function ProviderValidation() {
    var controlId = document.getElementById("ProviderID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Provider field is required." + "\n";
    } else {
        return "";
    }
}

function InvoiceTypeValidation() {
    var controlId = document.getElementById("InvoiceTypeID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The InvoiceType field is required." + "\n";
    } else {
        return "";
    }
}

function NumberValidation() {
    var controlId = document.getElementById("Number");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Number field is required." + "\n";
    } else {
        return "";
    }
}

function DateValidation() {
    var controlId = document.getElementById("Date");
    if (controlId.value === "") {
        return "The Date field is required." + "\n";
    } else {
        return "";
    }
}

function DueDateValidation() {
    var controlId = document.getElementById("DueDate");
    if (controlId.value === "") {
        return "The DueDate field is required." + "\n";
    } else {
        return "";
    }
}

function CheckQuantityValidation() {
    var controlId = document.getElementById("CheckQuantity");
    if (controlId.value === "") {
        return "The CheckQuantity field is required." + "\n";
    } else {
        return "";
    }
}

function CheckTotalValueWithoutTVAValidation() {
    var controlId = document.getElementById("CheckTotalValueWithoutTVA");
    if (controlId.value === "") {
        return "The CheckTotalValueWithoutTVA field is required." + "\n";
    } else {
        return "";
    }
}

function CheckTotalTVAValidation() {
    var controlId = document.getElementById("CheckTotalTVA");
    if (controlId.value === "") {
        return "The CheckTotalTVA field is required." + "\n";
    } else {
        return "";
    }
}

function DiscountMonthValidation() {
    var controlId = document.getElementById("DiscountMonth");
    if (controlId.value === "") {
        return "The DiscountMonth field is required." + "\n";
    } else {
        return "";
    }
}

function TypeValidation() {
    var controlId = document.getElementById("Type");
    if (controlId.value === "") {
        return "The Type field is required." + "\n";
    } else {
        return "";
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
