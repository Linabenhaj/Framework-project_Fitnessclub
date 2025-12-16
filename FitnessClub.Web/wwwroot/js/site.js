// AJAX helper functions
function ajaxPost(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        headers: {
            'X-CSRF-TOKEN': $('input[name="__RequestVerificationToken"]').val(),
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: successCallback,
        error: errorCallback || function (xhr, status, error) {
            console.error('AJAX error:', error);
            toastr.error('Er is een fout opgetreden: ' + error);
        }
    });
}

function ajaxGet(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: successCallback,
        error: errorCallback || function (xhr, status, error) {
            console.error('AJAX error:', error);
            toastr.error('Er is een fout opgetreden: ' + error);
        }
    });
}

// Toastr configuration
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// Get CSRF token
function getToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// Initialize tooltips
$(function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
});

// Auto-hide alerts after 5 seconds
$(document).ready(function () {
    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);
});