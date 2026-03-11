let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
let totalDivHeight = TitleMenuHeight 
let GridDivHeight = window.innerHeight - totalDivHeight - 100
let GridDiv = $('div[id$=GridDiv]')
GridDiv.height(GridDivHeight + 'px')

$(window).resize(() => {
    GridDiv.height(GridDivHeight + 'px')
})

function GridViewSize() {
    let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    let SearchIconDivHeight = document.getElementById('SearchIconDiv').clientHeight
    let totalDivHeight = TitleMenuHeight
    let GridDivHeight = window.innerHeight - totalDivHeight - 100
    let GridDiv = $('div[id$=GridDiv]')
    GridDiv.height(GridDivHeight + 'px')
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
    GridViewSize();
}

function HideSearch() {
    $('#ContentPlaceHolder1_OpenSearchBtn').removeAttr('style')
    $('#ContentPlaceHolder1_CloseSearchBtn').css('display', 'none')
    $('#ContentPlaceHolder1_SearchBox').css('display', 'none')
    GridViewSize();
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


