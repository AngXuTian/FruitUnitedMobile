var canvas = document.getElementById('customer-signature-pad');
var drivercanvas = document.getElementById('driver-signature-pad');

function listView() {
    var container = document.getElementById("GridViewTabControl");
    var btns = container.getElementsByClassName("btnr");
    for (var i = 0; i < btns.length; i++) {
        btns[i].addEventListener("click", function () {
            var current = document.getElementsByClassName("active");
            current[0].className = current[0].className.replace(" active", "");
            this.className += " active";
        });
    }
}

function OpenSearch() {
    $('#OpenSearchBtn').css('display', 'none')
    $('#CloseSearchBtn').removeAttr('style')
    $('#SearchBox').removeAttr('style')
}

function HideSearch() {
    $('#OpenSearchBtn').removeAttr('style')
    $('#CloseSearchBtn').css('display', 'none')
    $('#SearchBox').css('display', 'none')
};



// Adjust canvas coordinate space taking into account pixel ratio,
// to make it look crisp on mobile devices.
// This also causes canvas to be cleared.
function resizeCanvas() {
    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);

    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    drivercanvas.width = drivercanvas.offsetWidth * ratio;
    drivercanvas.height = drivercanvas.offsetHeight * ratio;
    drivercanvas.getContext("2d").scale(ratio, ratio);

}

window.onresize = resizeCanvas;
resizeCanvas();

var CustomersignaturePad = new SignaturePad(canvas, {
    backgroundColor: 'rgb(255,255,255)'
});

var DriversignaturePad = new SignaturePad(drivercanvas, {
    backgroundColor: 'rgb(255,255,255)'
});



function validateData() {
    if (CustomersignaturePad.isEmpty()) {
        alert('Customer Signature Required');
        return false;
    }
    else if (DriversignaturePad.isEmpty()) {
        alert('Driver Signature Required');
        return false;
    }
    else {

        // Get Signature data
        var data;
        if (!CustomersignaturePad.isEmpty()) {
            data = CustomersignaturePad.toDataURL()
        }

        var dataDriver;
        if (!DriversignaturePad.isEmpty()) {
            dataDriver = DriversignaturePad.toDataURL()
        }
       
        $('input[id$=CustomerSignatureHF]').val(data)
        $('input[id$=DriverSignatureHF]').val(dataDriver)
        return true;
    }
}

function onlyDotsAndNumbers(event) {
    if (event.keyCode > 47 && event.keyCode < 58 || event.keyCode == 46) {
        var txtbx = document.getElementById(txt);
        var amount = document.getElementById(txt).value;
        var present = 0;
        var count = 0;

        if (amount.indexOf(".", present) || amount.indexOf(".", present + 1));
        {
        }


        do {
            present = amount.indexOf(".", present);
            if (present != -1) {
                count++;
                present++;
            }
        }
        while (present != -1);
        if (present == -1 && amount.length == 0 && event.keyCode == 46) {
            event.keyCode = 0;
            return false;
        }

        if (count >= 1 && event.keyCode == 46) {

            event.keyCode = 0;
            return false;
        }
        if (count == 1) {
            var lastdigits = amount.substring(amount.indexOf(".") + 1, amount.length);
            if (lastdigits.length >= 2) {
                event.keyCode = 0;
                return false;
            }
        }
        return true;
    }
    else {
        event.keyCode = 0;
        return false;
    }

}

$('button[id$=HTMLClearSignatureBtn], input[id$=ClearSignatureBtn], button[id$=smallHTMLClearSignatureBtn]').click((e) => {
    // Prevent postback
    e.preventDefault()
    CustomersignaturePad.clear()
})

$('button[id$=HTMLClearSignatureBtn1], input[id$=ClearSignatureBtn1], button[id$=smallHTMLClearSignatureBtn1]').click((e) => {
    // Prevent postback
    e.preventDefault()
    DriversignaturePad.clear()
})
