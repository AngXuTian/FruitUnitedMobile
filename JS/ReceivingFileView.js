function GridViewSize() {
    let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
    let GridDivHeight = document.getElementById('GridDiv').clientHeight
    let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + DetailHeaderHeight + GridDivHeight
    let previewFrameHeight = window.innerHeight - totalDivHeight - 200
    let previewFrame = $('iframe[id$=PreviewIFrame]')
    previewFrame.height(previewFrameHeight + 'px')
}

const clickBtn = setTimeout('clickButton()', 1500)

$('button[id$=ViewPDFBtn]').click((e) => {
    let frame = $('iframe[id$=PreviewIFrame]')
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    })

    const ReceivingID = params.ReceivingID
    const FileName = params.FileName
    if (FileName.indexOf('.pdf') !== -1) {
        frame.attr('src', '../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/FruitUnitedMobileTest/Document/' + ReceivingID + '/' + FileName);
    }

})

function clickButton() {
    $('button[id$=ViewPDFBtn]').click()
}

let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
let GridDivHeight = document.getElementById('GridDiv').clientHeight
let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + DetailHeaderHeight + GridDivHeight
let previewFrameHeight = window.innerHeight - totalDivHeight - 200
let previewFrame = $('iframe[id$=PreviewIFrame]')
previewFrame.height(previewFrameHeight + 'px')