$(function () {
	$("#btnSubscribe").on("click", function () {
		var emailAddress = $("#mc-email").val();
		$.ajax({
			type: 'POST',
			url: '/Home/GetSubscriber', /*'@Url.Action("GetSubscriber", "Home")',*/
			data: { Email: emailAddress },
			dataType: 'json',
			success: function (response) {
				console.log(response);
				if (response.responseCode == 0) {
					$("#mc-email").val("");
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
			}
		});
	});

	$("#list-subscribe").on("click", function () {
		var emailAddress = $("#list-email").val();
		$.ajax({
			type: 'POST',
			url: '/Home/GetSubscriber', //'@Url.Action("GetSubscriber", "Home")',
			data: { Email: emailAddress },
			dataType: 'json',
			success: function (response) {
				console.log(response);
				if (response.responseCode == 0) {
					$("#list-email").val("");
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
			}
		});
	});

});