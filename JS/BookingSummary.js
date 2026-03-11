GridViewSize();
function showBrowseDialog() {
    var fileuploadctrl = document.getElementById('ContentPlaceHolder1_FileUpload1');
    fileuploadctrl.click();
}

function upload() {
    var btn = document.getElementById('ContentPlaceHolder1_hideButton');
    btn.click();
}

function GridViewSize() {
    //let ModuleHeaderHeight = document.getElementById('ModuleHeader').clientHeight
    //let TitleMenuHeight = document.getElementById('TitleMenu').clientHeight
    //let SearchMenuHeight = document.getElementById('SearchMenu').clientHeight
    //let SearchDivHeight = document.getElementById('SearchDiv').clientHeight
    //let SearchIconDivHeight = document.getElementById('SearchIconDiv').clientHeight
    //let StatusTabHeight = document.getElementById('StatusTab').clientHeight
    //let totalDivHeight = ModuleHeaderHeight + TitleMenuHeight + SearchMenuHeight + SearchDivHeight + SearchIconDivHeight + StatusTabHeight
    //let DetailHeaderFrameHeight = window.innerHeight - totalDivHeight - 70
    let GridDiv = $('div[id$=GridDiv2]')
    GridDiv.height('150px')
}

function OpenSearch() {
    $('#ContentPlaceHolder1_OpenSearchBtn').css('display', 'none')
    $('#ContentPlaceHolder1_CloseSearchBtn').removeAttr('style')
    $('#SearchDiv').removeAttr('style')
    GridViewSize();
}

function HideSearch() {
    $('#ContentPlaceHolder1_OpenSearchBtn').removeAttr('style')
    $('#ContentPlaceHolder1_CloseSearchBtn').css('display', 'none')
    $('#SearchDiv').css('display', 'none')
    GridViewSize();
};