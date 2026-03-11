let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
let RemarksHeight = document.getElementById('Remarks').clientHeight
let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + DetailHeaderHeight + RemarksHeight
let GridDivHeight = window.innerHeight - totalDivHeight - 100
let GridDiv = $('div[id$=GridDiv]')
GridDiv.height(GridDivHeight + 'px')

$(window).resize(() => {
    GridDiv.height(GridDivHeight + 'px')
})

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
    let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let SearchIconDivHeight = document.getElementById('SearchIconDiv').clientHeight
    let SearchDivHeight = document.getElementById('SearchDiv').clientHeight
    let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + SearchIconDivHeight + SearchDivHeight
    let GridDivHeight = window.innerHeight - totalDivHeight - 100
    let GridDiv = $('div[id$=GridDiv]')
    GridDiv.height(GridDivHeight + 'px')
}

function HideSearch() {
    $('#ContentPlaceHolder1_OpenSearchBtn').removeAttr('style')
    $('#ContentPlaceHolder1_CloseSearchBtn').css('display', 'none')
    $('#ContentPlaceHolder1_SearchBox').css('display', 'none')
    let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let SearchIconDivHeight = document.getElementById('SearchIconDiv').clientHeight
    let SearchDivHeight = document.getElementById('SearchDiv').clientHeight
    let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + SearchIconDivHeight + SearchDivHeight
    let GridDivHeight = window.innerHeight - totalDivHeight - 100
    let GridDiv = $('div[id$=GridDiv]')
    GridDiv.height(GridDivHeight + 'px')
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


function ReceivingListRadioSelect(Radio) {
    $(Radio).closest("table").find("input:radio").prop("checked", false);
    $(Radio).prop("checked", true);
    document.getElementById('ContentPlaceHolder1_EditBtn').disabled = false;
}

function ValidateBarcode() {
    var BarcodeTB = document.getElementById("ContentPlaceHolder1_BarcodeTB").value;
    if (BarcodeTB.length < 1) { document.getElementById('ContentPlaceHolder1_ScanBtn').disabled = true; }
    else { document.getElementById('ContentPlaceHolder1_ScanBtn').disabled = false; }
}


function showBrowseDialog() {
    var fileuploadctrl = document.getElementById('ContentPlaceHolder1_FileUpload1');
    fileuploadctrl.click();
}

function upload() {
    var btn = document.getElementById('ContentPlaceHolder1_hideButton');
    btn.click();
}



