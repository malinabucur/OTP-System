﻿function CreateToast(toastId, header, message, icon, isAlert = false, isError = false, delay = 30, containerId = "toast_wrapper") {
    $("#" + toastId).remove();

    var toast = $("<div style='z-index: 11'></div>");
    toast.attr("id", toastId);
    toast.addClass("toast");
    toast.attr("role", isAlert ? "alert" : "assertive");
    toast.attr("status", isError ? "status" : "polite");
    toast.attr("aria-atomic", false);
    toast.attr("data-bs-autohide", false);

    var bgColor = isError ? 'bg-danger' : (isAlert ? 'bg-warning' : 'bg-success');
    var textColor = 'text-light';
    var iconColor = 'text-light';
    var btnClose = 'btn-close btn-close-white';

    var toastHeader = $("<div></div>");
    toastHeader.addClass("toast-header " + bgColor + " " + textColor);
    toastHeader.append("<span class='bi " + icon + " " + iconColor + " me-2'></span>");
    toastHeader.append("<strong class='me-auto'>" + header + "</strong>");
    toastHeader.append("<small id='timer' class='" + textColor + "'></small>");
    toastHeader.append("<button type='button' class='" + btnClose + "' data-bs-dismiss='toast' aria-label='Close'></button>");

    var toastBody = $("<div></div>");
    toastBody.addClass("toast-body");
    toastBody.append(message);

    toast.append(toastHeader);
    toast.append(toastBody);

    $("#" + containerId).append(toast);

    var interval = setInterval(function () {
        delay -= 1;
        toastHeader.find("#timer").text(delay + "s left");
        if (delay === 0) {
            // Clear the interval
            clearInterval(interval); 
            toast.fadeOut(400, function () {
                // Remove the toast after fade out
                toast.remove(); 
            });
        }
    }, 1000);

    toast.toast("show");
}

function SuccessToast(toastId, message, header = "Success!", delay) {
    CreateToast(toastId, header, message, "", false, false, delay);
}

function ErrorToast(toastId, message, header = "Error!", delay) {
    CreateToast(toastId, header, message, "", false, true, delay);
}