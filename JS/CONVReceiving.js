function GridViewSize() {
    let ModuleHeaderHeight = document.getElementById('ModuleHeader').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let totalDivHeight = ModuleHeaderHeight + ButtonMenuHeight
    let DetailHeaderFrameHeight = window.innerHeight - totalDivHeight - 80
    let DetailHeader = $('div[id$=DetailHeader]')
    DetailHeader.height(DetailHeaderFrameHeight + 'px')
}

GridViewSize()

function isFloatNumber(e, t) {
    var n;
    var r;
    if (navigator.appName == "Microsoft Internet Explorer" || navigator.appName == "Netscape") {
        n = t.keyCode;
        r = 1;
        if (navigator.appName == "Netscape") {
            n = t.charCode;
            r = 0
        }
    } else {
        n = t.charCode;
        r = 0
    }
    if (r == 1) {
        if (!(n >= 48 && n <= 57 || n == 46)) {
            t.returnValue = false
        }
    } else {
        if (!(n >= 48 && n <= 57 || n == 0 || n == 46)) {
            t.preventDefault()
        }
        
    }
}

function CalculateGoodConditionValue() {
    var TOTALValue = 0;
    if (document.getElementById('ContentPlaceHolder1_TotalTB').value)
    {
        TOTALValue = document.getElementById('ContentPlaceHolder1_TotalTB').value;
    }
    var DamageValue = 0;
    if (document.getElementById('ContentPlaceHolder1_DamageTB').value) {
        DamageValue = document.getElementById('ContentPlaceHolder1_DamageTB').value;
    }
    document.getElementById('ContentPlaceHolder1_GoodConditionTB').value = isNaN(isNaN(parseInt(TOTALValue) ? 0 : parseInt(TOTALValue)) - isNaN(parseInt(DamageValue) ? 0 : parseInt(DamageValue))) ? 0 : parseInt(TOTALValue) - parseInt(DamageValue);
}
