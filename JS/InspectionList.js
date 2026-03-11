

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
    if (BarcodeTB.length < 1) { document.getElementById('ContentPlaceHolder1_ButtonRefill').disabled = true; }
    else { document.getElementById('ContentPlaceHolder1_ButtonRefill').disabled = false; }
}


function InsertIntoBCTB(Text) {
    document.getElementById("ContentPlaceHolder1_BarcodeTB").value = Text;
    ValidateBarcode();
}

function ConvertingCylider_Changed() {
    if (document.getElementById("ContentPlaceHolder1_ConvertingCylinderRadioButtonList_0").checked) {
        document.getElementById("ContentPlaceHolder1_ddlNewGasType").disabled = false;
    }
    else {
        document.getElementById("ContentPlaceHolder1_ddlNewGasType").disabled = true;
    }
}



