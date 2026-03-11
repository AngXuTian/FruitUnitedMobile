
function validateData() {
    var Reason = document.getElementById("ContentPlaceHolder1_ReasonText").value;
    if (Reason.length ===0) {
        alert('Please Enter Reason Before Proceed.');
        return false;
    }
    return true;
}

