var canvas = document.getElementById('signature-pad');
var smallCanvas = document.getElementById('smallSignature-pad');

// Adjust canvas coordinate space taking into account pixel ratio,
// to make it look crisp on mobile devices.
// This also causes canvas to be cleared.
function resizeCanvas() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);

    smallCanvas.width = smallCanvas.offsetWidth * ratio;
    smallCanvas.height = smallCanvas.offsetHeight * ratio;
    smallCanvas.getContext("2d").scale(ratio, ratio);
}

window.onresize = resizeCanvas;
resizeCanvas();

var signaturePad = new SignaturePad(canvas, {
    backgroundColor: 'rgb(255,255,255)'
});

var smallSignaturePad = new SignaturePad(smallCanvas, {
    backgroundColor: 'rgb(255,255,255)'
});

function validateData() {
    let collectorNameElement = $('input[id$=CustomerName]')
    let collectorName = $('input[id$=CustomerName]').val()
    let errorMsg = []

    if (signaturePad.isEmpty() && smallSignaturePad.isEmpty()) {
        alert('Please sign before proceedeeee');
        return
    }
    else {
        
        // Get Signature data
        var data;
        if (!signaturePad.isEmpty()) {
            data = signaturePad.toDataURL()
        }
        else if (!smallSignaturePad.isEmpty()) {
            data = smallSignaturePad.toDataURL()
        }

        $('input[id$=SignatureHF]').val(data)
        
    }

    if (!collectorName) {

        if (!collectorName) {
            //errorMsg.push('Name is mandatory')
            collectorNameElement.addClass('error')
        }
        if (errorMsg) {
            $('span[id$=ErrorLbl]').text('Please fill in mandatory(*) fields')
        }
        return false
    }
    else if (collectorName && collectorID) {
        return true
    }
}

function isNumberKey(txt, evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 46) {
        //Check if the text already contains the . character
        if (txt.value.indexOf('.') === -1) {
            return true;
        } else {
            return false;
        }
    } else {
        if (charCode > 31 &&
            (charCode < 48 || charCode > 57))
            return false;
    }
    return true;
}


$('button[id$=HTMLClearSignatureBtn], input[id$=ClearSignatureBtn], button[id$=smallHTMLClearSignatureBtn]').click((e) => {
    // Prevent postback
    e.preventDefault()

    signaturePad.clear()
    smallSignaturePad.clear()
})
