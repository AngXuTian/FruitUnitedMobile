
var canvas = document.getElementById('signature-pad');
var smallCanvas = document.getElementById('smallSignature-pad');


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

function validateData() {
    if (signaturePad.isEmpty() && smallSignaturePad.isEmpty()) {
        alert('Please sign before proceedeeee');
        return false;
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

    signaturePad.clear()
    smallSignaturePad.clear()
})
