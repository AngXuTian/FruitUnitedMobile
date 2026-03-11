$("#ContentPlaceHolder1_BookingDetailGrid_CheckBoxHeader").click(function (event) {
    if ($('#ContentPlaceHolder1_BookingDetailGrid_CheckBoxHeader').is(':checked')) {
        $("#ContentPlaceHolder1_BookingDetailGrid_CheckBoxHeader").prop("checked", true);
        $("#OutputGridDiv").find("[type='checkbox']").prop('checked', true);
    }
    else {
        $("#ContentPlaceHolder1_BookingDetailGrid_CheckBoxHeader").prop("checked", false);
        $("#OutputGridDiv").find("[type='checkbox']").prop('checked', false);
    }
}
);