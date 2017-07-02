$(document).ready(function () {
    barcode128();
});

function barcode128() {

    var divs = document.getElementsByName("barcode");
    var divLength = divs.length;
    for (var k = 0; k < divLength; k++) {
        var divDom = divs[k];
        var barcode = "";
        barcode = divDom.getAttribute('barcodeValue');
        var id = divDom.getAttribute('id');
        if (barcode && barcode != '') {
            $("div[id='" + id + "']").empty().barcode(barcode, "code128", { barWidth: 2, barHeight: 90, showHRI: false });
        }
    }
}
