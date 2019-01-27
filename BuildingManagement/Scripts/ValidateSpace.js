function ValidateAll(operation) {
    var validationSummary = "";
    validationSummary += LevelValidation();
    validationSummary += SpaceTypeValidation();
    validationSummary += NumberValidation();
    validationSummary += SubClientValidation();
    validationSummary += SurfaceValidation();
    validationSummary += PeopleValidation();
    if (validationSummary !== "") {
        alert(validationSummary);
        return false;
    } else {
        var data = {
            Number: document.getElementById("Number").value,
            Surface: document.getElementById("Surface").value,
            People: document.getElementById("People").value,
            Inhabited: document.getElementById("Inhabited").checked,
            SpaceTypeID: document.getElementById("SpaceTypeID").value,
            SubClientID: document.getElementById("SubClientID").value,
            LevelID: document.getElementById("LevelID").value
        };
        var url = "/Space/Create";
        var indexUrl = "/Space/Index";
        if (operation === "Edit") {
            data["ID"] = document.getElementById("ID").value;
            url = "../Edit";
        }
        Submit(operation, url, indexUrl, data);
        return true;
    }
}