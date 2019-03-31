function OnChangeEvent() {
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