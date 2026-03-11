function GridViewSize() {
    let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
    let GridViewTabHeight = document.getElementById('GridViewTab').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let totalDivHeight = DetailHeaderHeight + GridViewTabHeight + ButtonMenuHeight
    let GridDivHeight = window.innerHeight - totalDivHeight - 120
    let GridDiv = $('div[id$=GridDiv]')
    GridDiv.height(GridDivHeight + 'px')
}
GridViewSize();
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
