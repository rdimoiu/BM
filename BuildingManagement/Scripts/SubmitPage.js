function Submit(operation, url, indexUrl, data) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function () {
            window.location.href = indexUrl;
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}