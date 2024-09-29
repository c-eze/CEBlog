const SENSITIVITY = 10;

//document.addEventListener('DOMContentLoaded', () => {
$(document).ready(function () {
    const modalEle = document.getElementById('subscribeModal');
    var subscribeModal = new bootstrap.Modal(modalEle);
    const subscribeBtn = document.getElementById('modalBtn');
    const closeBtn = document.getElementById("closeBtn");

    const hasModalShown = () => {
        return sessionStorage.getItem("modalShown") === 'true';
    };

    const markModalShown = () => {
        sessionStorage.setItem("modalShown", "true");
    };

    const handleMouseLeave = (e) => {
        if (e.clientY < SENSITIVITY && !hasModalShown()) {
            document.removeEventListener('mouseleave', handleMouseLeave);

            // Show the modal
            subscribeModal.show();

            markModalShown();
        }
    };

    document.addEventListener('mouseleave', handleMouseLeave);
    subscribeBtn.addEventListener("click", function () {
        addSubscriber("#modal-email");
        closeBtn.click();
        event.stopImmediatePropagation();
    });
});



const addSubscriber = (element) => {
    var formData = new FormData();
    formData.append("email", $(element).val());

    $.ajax({
        type: 'POST',
        url: '/Home/PostSubscriber',
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: successCallback,
        error: function () { }
    });
};

function successCallback() {
    Swal.fire({
        icon: "success",
        position: "top-end",
        showConfirmButton: false,
        title: "Thank you for subscribing!",
        timer: 2500
    });
}