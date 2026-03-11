function GridViewSize() {
    let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
    let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
    let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + DetailHeaderHeight
    let previewFrameHeight = window.innerHeight - totalDivHeight - 150
    let previewFrame = $('iframe[id$=PreviewIFrame]')
    previewFrame.height(previewFrameHeight + 'px')
}

const clickBtn = setTimeout('clickButton()', 1500)

$(document).ready(function () {
    let empID = document.getElementById("empIDField").value; // Get EmpID from hidden field
    let safeEmpID = empID.replace(/\//g, "_"); // Replace slashes with underscores
    $('button[id$=ViewPDFBtn]').click((e) => {
        let frame = $('iframe[id$=PreviewIFrame]');
        let pdfUrl = `../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/FruitUnitedMobileTest/Uploads/WorkerSchedule/${safeEmpID}.pdf`;

        frame.attr('src', pdfUrl);
    });
});

function clickButton() {
    $('button[id$=ViewPDFBtn]').click()
}

let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
let ButtonMenuHeight = document.getElementById('ButtonMenu').clientHeight
let DetailHeaderHeight = document.getElementById('DetailHeader').clientHeight
let totalDivHeight = TitleMenuHeight + ButtonMenuHeight + DetailHeaderHeight
let previewFrameHeight = window.innerHeight - totalDivHeight - 150
let previewFrame = $('iframe[id$=PreviewIFrame]')
previewFrame.height(previewFrameHeight + 'px')