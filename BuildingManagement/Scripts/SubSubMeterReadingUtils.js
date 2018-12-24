function GetMeterTypes(idSubSubMeter, idMeterType) {
    var meterTypeList = $("#MeterTypeID");
    if (idSubSubMeter === "") {
        meterTypeList.empty();
        var firstItem = new Option("Select...", "0");
        meterTypeList.append(firstItem);
        return;
    }
    $.ajax({
        url: "/SubSubMeterReading/GetMeterTypes/",
        data: { subSubMeterId: idSubSubMeter, meterTypeId: idMeterType },
        cache: false,
        type: "POST",
        success: function (data) {
            meterTypeList.empty();
            var first = new Option("Select...", "0");
            meterTypeList.append(first);
            for (var i = 0; i < data.length; i++) {
                var item = new Option(data[i].Text, data[i].Value, false, data[i].Selected);
                meterTypeList.append(item);
            }
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}