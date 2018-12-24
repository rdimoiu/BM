function GetMeterTypes(idMeter) {
    var meterTypeList = $("#MeterTypeID");
    if (idMeter === "") {
        meterTypeList.empty();
        var firstItem = new Option("Select...", "0");
        meterTypeList.append(firstItem);
        return;
    }
    $.ajax({
        url: "/MeterReading/GetMeterTypes/",
        data: { meterId: idMeter },
        cache: false,
        type: "POST",
        success: function (data) {
            meterTypeList.empty();
            var first = new Option("Select...", "0");
            meterTypeList.append(first);
            for (var i = 0; i < data.length; i++) {
                var item = new Option(data[i].Text, data[i].Value);
                meterTypeList.append(item);
            }
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}