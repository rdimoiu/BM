function GetMeterTypes(idSubMeter, idMeterType) {
    var meterTypeList = $("#MeterTypeID");
    if (idSubMeter === "") {
        meterTypeList.empty();
        var firstItem = new Option("Select...", "0");
        meterTypeList.append(firstItem);
        return;
    }
    $.ajax({
        url: "/SubMeterReading/GetMeterTypes/",
        data: { subMeterId: idSubMeter, meterTypeId: idMeterType },
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