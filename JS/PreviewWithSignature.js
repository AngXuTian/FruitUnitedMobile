let fileUploadDivH = document.getElementById('FileUploadInfo').clientHeight
let btnContainerH = document.getElementById('ButtonContainer') ? document.getElementById('ButtonContainer').clientHeight : 0
let htmlBtnContainerH = document.getElementById('HTMLBtnContainer') ? document.getElementById('HTMLButtonContainer').clientHeight : 0
let moduleHeaderH = document.getElementById('ModuleHeader').clientHeight
let viewPDFHeightBtn = document.getElementById('ViewPDFBtnDiv').clientHeight

let totalHeightTaken = fileUploadDivH + btnContainerH + htmlBtnContainerH + moduleHeaderH + 100

let previewFrame = document.getElementsByTagName("iframe")
let preview = $('#iFrameDiv')

$('iframe[id$=PreviewIFrame]').height((window.innerHeight - totalHeightTaken) + 'px')

const clickBtn = setTimeout('clickButton()', 3000)

$('button[id$=ViewPDFBtn]').click((e) => {

    let frame = $('iframe[id$=PreviewIFrame]')
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    })

    let value = params.DONo
    frame.attr('src', '../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/FruitUnitedDO/Uploads/Reports/' + value.replace('/', '_') + '_Final.pdf')

})

function clickButton() {
    $('button[id$=ViewPDFBtn]').click()
}


$(window).resize(() => {
    $('iframe[id$=PreviewIFrame]').height((window.innerHeight - totalHeightTaken) + 'px')
})