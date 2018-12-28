function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,6})+$/;
    return regex.test(email);
}

function isPhone(phone) {
    var regex = /^\(?(\d{3})\)?[-\. ]?(\d{3})[-\. ]?(\d{4})$/;
    return regex.test(phone);
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
    } else if (!isPhone(controlId.value)) {
        return "The Phone is invalid." + "\n";
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
    } else if (!isEmail(controlId.value)) {
        return "The Email is invalid." + "\n";
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

function CodeValidation() {
    var controlId = document.getElementById("Code");
    if (controlId.value === "") {
        return "The Code field is required." + "\n";
    } else {
        return "";
    }
}

function InitialIndexValidation() {
    var controlId = document.getElementById("InitialIndex");
    if (controlId.value === "") {
        return "The InitialIndex field is required." + "\n";
    } else {
        return "";
    }
}

function DistributionModeValidation() {
    var controlId = document.getElementById("DistributionModeID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The DistributionMode field is required." + "\n";
    } else {
        return "";
    }
}

function IndexValidation() {
    var controlId = document.getElementById("Index");
    if (controlId.value === "") {
        return "The Index field is required." + "\n";
    } else {
        return "";
    }
}

function MeterValidation() {
    var controlId = document.getElementById("MeterID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Meter field is required." + "\n";
    } else {
        return "";
    }
}

function MeterTypeValidation() {
    var controlId = document.getElementById("MeterTypeID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The MeterType field is required." + "\n";
    } else {
        return "";
    }
}

function FiscalCodeValidation() {
    var controlId = document.getElementById("FiscalCode");
    if (controlId.value === "") {
        return "The FiscalCode field is required." + "\n";
    } else {
        return "";
    }
}

function isFiscalCode(phone) {
    var regex = /^\(?(\d{3})\)?[-\. ]?(\d{3})[-\. ]?(\d{4})$/;
    return regex.test(phone);
}

function isCNP(phone) {
    var regex = /^\(?(\d{3})\)?[-\. ]?(\d{3})[-\. ]?(\d{4})$/;
    return regex.test(phone);
}

function CNPOrFiscalCodeValidation() {
    var cnpId = document.getElementById("CNP");
    var fiscalCodeId = document.getElementById("FiscalCode");
    if (cnpId.value === "" && fiscalCodeId.value === "") {
        return "The CNP or FiscalCode field is required." + "\n";
    } else {
        if (!isFiscalCode(fiscalCodeId.value)) {
            return "The FiscalCode is invalid." + "\n";
        }
        if (!isCNP(cnpId.value)) {
            return "The CNP is invalid." + "\n";
        }
    }
    return "";
}

function TradeRegisterValidation() {
    var controlId = document.getElementById("TradeRegister");
    if (controlId.value === "") {
        return "The TradeRegister field is required." + "\n";
    } else {
        return "";
    }
}

function BankAccountValidation() {
    var controlId = document.getElementById("BankAccount");
    if (controlId.value === "") {
        return "The BankAccount field is required." + "\n";
    } else {
        return "";
    }
}

function BankValidation() {
    var controlId = document.getElementById("Bank");
    if (controlId.value === "") {
        return "The Bank field is required." + "\n";
    } else {
        return "";
    }
}

function InvoiceValidation() {
    var controlId = document.getElementById("InvoiceID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The Invoice field is required." + "\n";
    } else {
        return "";
    }
}

function UnitValidation() {
    var controlId = document.getElementById("Unit");
    if (controlId.value === "") {
        return "The Unit field is required." + "\n";
    } else {
        return "";
    }
}

function SurfaceValidation() {
    var controlId = document.getElementById("Surface");
    if (controlId.value === "") {
        return "The Surface field is required." + "\n";
    } else {
        return "";
    }
}

function PeopleValidation() {
    var controlId = document.getElementById("People");
    if (controlId.value === "") {
        return "The People field is required." + "\n";
    } else {
        return "";
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

function SubMeterValidation() {
    var controlId = document.getElementById("SubMeterID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The SubMeter field is required." + "\n";
    } else {
        return "";
    }
}

function SubSubMeterValidation() {
    var controlId = document.getElementById("SubSubMeterID");
    if (controlId.value === "" || controlId.value === "0") {
        return "The SubSubMeter field is required." + "\n";
    } else {
        return "";
    }
}
