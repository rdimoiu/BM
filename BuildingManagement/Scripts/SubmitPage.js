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
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            if (xhr.status === 409) {
                alert(customErrorMessage);
            }
        }
    });
}