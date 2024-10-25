const SENSITIVITY = 10;

$(function () {
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
    subscribeBtn.addEventListener("click", function (event) {
        event.preventDefault();
        addSubscriber();
        event.stopImmediatePropagation();
    });
});

const addSubscriber = () => {
    var emailAddress = document.getElementById("modal-email").value;
    $.ajax({
        type: 'POST',
        url: '/Home/GetSubscriber',
        data: { Email: emailAddress },
        dataType: 'json',
        success: function (response) {
            if (response.responseCode == 0) {
                $("#modal-email").val("");
                Swal.fire({
                    icon: "success",
                    position: "top-end",
                    showConfirmButton: false,
                    title: "Thank you for subscribing!",
                    timer: 1500
                });
            }
        },
        error: function (xhr, status, error) {
            console.log("Subscriber email failed.");
            console.log(status);
            console.log(error);
        }
    });
};
