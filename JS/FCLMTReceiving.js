function GridViewSize() {
    let ModuleHeaderHeight = document.getElementById('ModuleHeader').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let totalDivHeight = ModuleHeaderHeight + ButtonMenuHeight
    let DetailHeaderFrameHeight = window.innerHeight - totalDivHeight - 80
    let DetailHeader = $('div[id$=DetailHeader]')
    DetailHeader.height(DetailHeaderFrameHeight + 'px')
}

const clickBtn = setTimeout('clickButton()', 1500)

$('button[id$=ViewPDFBtn]').click((e) => {

    let frame = $('iframe[id$=PreviewIFrame]')
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    })

    let value = params.DONo
    frame.attr('src', '../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/ComnetInvoiceApproval/Reports/Invoice_Temp.pdf')

})

function clickButton() {
    $('button[id$=ViewPDFBtn]').click()
}

let ModuleHeaderHeight = document.getElementById('ModuleHeader').clientHeight
let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
let totalDivHeight = ModuleHeaderHeight + ButtonMenuHeight
let DetailHeaderFrameHeight = window.innerHeight - totalDivHeight - 80
let DetailHeader = $('div[id$=DetailHeader]')
DetailHeader.height(DetailHeaderFrameHeight + 'px')

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

var Container1DDL = document.getElementById('ContentPlaceHolder1_Container1DDL');
var Container2DDL = document.getElementById('ContentPlaceHolder1_Container2DDL');

if (Container1DDL.value) {
    document.getElementById('ContentPlaceHolder1_Container1TB').setAttribute('readonly', 'readonly');
    document.getElementById('ContentPlaceHolder1_Container1TB').value = "";
}
else {
    document.getElementById('ContentPlaceHolder1_Container1TB').removeAttribute('readonly');
    document.getElementById('ContentPlaceHolder1_Container1TB').setAttribute('style', 'border-color:LightGrey;width:100%;');
    document.getElementById('ContentPlaceHolder1_Container1TB').value = "";
}

if (Container2DDL.value) {
    document.getElementById('ContentPlaceHolder1_Container2TB').setAttribute('readonly', 'readonly');
    document.getElementById('ContentPlaceHolder1_Container2TB').value = "";
}
else {
    document.getElementById('ContentPlaceHolder1_Container2TB').removeAttribute('readonly');
    document.getElementById('ContentPlaceHolder1_Container2TB').setAttribute('style', 'border-color:LightGrey;width:100%;');
    document.getElementById('ContentPlaceHolder1_Container2TB').value = "";
}

Container1DDL.onchange = (event) => {
    var inputText = event.target.value;
    if (inputText) {
        document.getElementById('ContentPlaceHolder1_Container1TB').setAttribute('readonly', 'readonly');
        document.getElementById('ContentPlaceHolder1_Container1TB').value = "";
    }
    else {
        document.getElementById('ContentPlaceHolder1_Container1TB').removeAttribute('readonly');
        document.getElementById('ContentPlaceHolder1_Container1TB').setAttribute('style', 'border-color:LightGrey;width:100%;');
        document.getElementById('ContentPlaceHolder1_Container1TB').value = "";
    }
}

Container2DDL.onchange = (event) => {
    var inputText2 = event.target.value;
    if (inputText2) {
        document.getElementById('ContentPlaceHolder1_Container2TB').setAttribute('readonly', 'readonly');
        document.getElementById('ContentPlaceHolder1_Container2TB').value = "";
    }
    else {
        document.getElementById('ContentPlaceHolder1_Container2TB').removeAttribute('readonly');
        document.getElementById('ContentPlaceHolder1_Container2TB').setAttribute('style', 'border-color:LightGrey;width:100%;');
        document.getElementById('ContentPlaceHolder1_Container2TB').value = "";
    }
}