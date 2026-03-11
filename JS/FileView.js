const clickBtn = setTimeout('clickButton()', 3000)

$('button[id$=ViewPDFBtn]').click((e) => {
    let frame = $('iframe[id$=PreviewIFrame]')
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    })

    let IDvalue = params.Booking_Receiving_ID
    let FileNamevalue = params.FileName

    frame.attr('src', '../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/FruitUnitedMobileTest/Uploads/Booking_Receiving/File/' + IDvalue.replace('/', '_') + '/' + FileNamevalue.replace('/', '_'))
})

GridViewSize();

function GridViewSize() {
    let ModuleHeaderHeight = document.getElementById('ModuleHeader').clientHeight
    let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let GridDivHeight = document.getElementById('GridDiv').clientHeight
    let totalDivHeight = ModuleHeaderHeight + ButtonMenuHeight + DetailHeaderHeight + GridDivHeight
    let previewFrameHeight = window.innerHeight - totalDivHeight
    let previewFrame = $('iframe[id$=PreviewIFrame]')
    previewFrame.height(previewFrameHeight + 'px')
}

function clickButton() {
    $('button[id$=ViewPDFBtn]').click()
}

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
    $('#ContentPlaceHolder1_OpenSearchBtn').css('display', 'none')
    $('#ContentPlaceHolder1_CloseSearchBtn').removeAttr('style')
    $('#ContentPlaceHolder1_SearchBox').removeAttr('style')
}

function HideSearch() {
    $('#ContentPlaceHolder1_OpenSearchBtn').removeAttr('style')
    $('#ContentPlaceHolder1_CloseSearchBtn').css('display', 'none')
    $('#ContentPlaceHolder1_SearchBox').css('display', 'none')
};


function CollectionCylinderListRadioSelect(Radio) {
    $(Radio).closest("table").find("input:radio").prop("checked", false);
    $(Radio).prop("checked", true);
    document.getElementById('ContentPlaceHolder1_ButtonDelete').disabled = false;
}

function CollectionDetailRadioSelect(Radio) {
    $(Radio).closest("table").find("input:radio").prop("checked", false);
    $(Radio).prop("checked", true);
    document.getElementById('ContentPlaceHolder1_ButtonDelete').disabled = false;
}

function ValidateBarcode() {
    var BarcodeTB = document.getElementById("ContentPlaceHolder1_BarcodeTB").value;
    if (BarcodeTB.length < 1) { document.getElementById('ContentPlaceHolder1_ScanBtn').disabled = true; }
    else { document.getElementById('ContentPlaceHolder1_ScanBtn').disabled = false; }
}

$('#file').on("change", function () {
    for (i = 0; i < $('form').length; i++) {
        if ($('form').get(i)[0].value != "") /* get the file tag, you have to customize this code */ {
            var formdata = new FormData($('form').get(i));
            CallService(formdata);
            break;
        }
    }
});
function CallService(file) {
    $.ajax({
        url: '@Url.Action("Scan", "Home")',
        type: 'POST',
        data: file,
        cache: false,
        processData: false,
        contentType: false,
        success: function (barcode) {
            alert(barcode);
        },
        error: function () {
            alert("ERROR");
        }
    });
}

    function showBrowseDialog() {
        var fileuploadctrl = document.getElementById('ContentPlaceHolder1_FileUpload1');
        fileuploadctrl.click();
    }

    function upload() {
        var btn = document.getElementById('ContentPlaceHolder1_hideButton');
        btn.click();
    }


