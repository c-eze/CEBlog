$(document).ready(function(){
    $("#btnSubscribe").on("click", function(){
        submitSubcriber("#mc_email");
    });

    $("#list-subscribe").on("click", function(){
        submitSubcriber("#list-email");
    });
});

function submitSubcriber(element) {
    var formData = new FormData();
    formData.append("email", $(element).val());
    $.ajax({
        type: 'POST',
        url: '/Home/PostSubscriber',
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: successCallback(element),
        error: errorCallback
    });
}

function resetForm(element) {
    $(element).val("");
}

function successCallback(element) {
    resetForm(element);
    Swal.fire({
        icon: "success",
        position: "top-end",
        showConfirmButton: false,
        title: "Thank you for subscribing!",
        timer: 1500 
    });
}

function errorCallback() {
}