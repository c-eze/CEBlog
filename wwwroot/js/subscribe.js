function submitSubcriber() {
    var formData = new FormData();
    formData.append("email", $("#mc_email").val());
    $.ajax({
        type: 'POST',
        url: @Url.Action("PostSubscriber", "Home"),
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: successCallback,
        error: errorCallback
    });
    function resetForm() {
        $("#email").val("");
    }
    function errorCallback() {
        alert('Failed to receive the Data');
    }
    function successCallback(response) {
        if (response.ResponseCode == 0) {
            resetForm();
            alert('Successfully received Data');
        }
        else {
            alert(response.ResponseMessage);
        }
    }
} 