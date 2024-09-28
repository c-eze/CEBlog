const SENSITIVITY = 10;

document.addEventListener('DOMContentLoaded', () => {
    const modalEle = document.getElementById('subscribeModal');
    var subscribeModal = new bootstrap.Modal(modalEle);

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
});
