$(document).ready(function () {
    // Get all anchor elements with a specific class name
    var anchors = document.getElementsByClassName("reply"); 

    // Add click event listener to each anchor
    Array.from(anchors).forEach(function (anchor) {
        anchor.addEventListener("click", function (event) { 
            //unhide reply form
            var replyForm = anchor.nextElementSibling;
            replyForm.removeAttribute("hidden");

            //add in author name to text body
            var replyBox = anchor.nextElementSibling.querySelector("div > div > textarea");
            var commenterName = anchor.previousElementSibling.innerHTML;
            console.log(`Reply: ${replyBox}`);
            replyBox.innerHTML = `@${commenterName} `;
        });
    });
});