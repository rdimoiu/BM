function InitialOnChangeEvent() {
    var initial = document.getElementById("Initial");
    var estimated = document.getElementById("Estimated");
    var estimatedLabel = document.getElementById("EstimatedLabel");
    var discountMonth = document.getElementById("DiscountMonth");
    var discountMonthLabel = document.getElementById("DiscountMonthLabel");
    if (initial.checked) {
        estimated.style.display = "none";
        estimatedLabel.style.display = "none";
        estimated.checked = false;
        discountMonthLabel.style.display = "none";
        discountMonth.style.display = "none";
        discountMonth.value = (new Date('0001-01-01')).toUTCString();

    } else {
        estimated.style.display = "initial";
        estimatedLabel.style.display = "initial";
        discountMonthLabel.style.display = "initial";
        discountMonth.style.display = "initial";
    }
}

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