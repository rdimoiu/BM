function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += SectionValidation();
    validationSummary += NumberValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            SectionID: document.getElementById("SectionID").value,
            Number: document.getElementById("Number").value
        };
        var url = "/Level/Create";
        var indexUrl = "/Level/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}