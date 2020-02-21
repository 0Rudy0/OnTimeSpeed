function ajaxGET(ajaxObj, callback) {
    $.ajax({
        url: appName + ajaxObj.url,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: ajaxObj.data,
        async: true,
        cache: false,
        success: callback.bind(this),
        error: function (er) {
            $('.spinner').hide();
            var tempToast = {
                html: '<span style="color: black">Greška</span>',
                classes: toastClasses,
                displayLength: toastLong
            };
            M.toast(tempToast);
        }
    });
}

function ajaxPOST(ajaxObj, callback) {
    $.ajax({
        url: appName + ajaxObj.url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(ajaxObj.data),
        async: true,
        cache: false,
        success: callback.bind(this),
        error: function (er) {
            $('.spinner').hide();

            var tempToast = {
                html: '<span style="color: black">Greška</span>',
                classes: 'red darken-2',
                displayLength: toastLong
            };
            M.toast(tempToast);
        }
    });
}

$(document).ajaxError(function (event, request, settings, thrownError) {
    //debugger;

    if (request.getResponseHeader('REQUIRES_AUTH') === '1') {
        location.reload();
    }
});

$(document).ajaxSuccess(function (event, request, settings) {
    //debugger;

    if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') === '1') {
        location.reload();
    }
    if (request.getResponseHeader('REQUIRES_AUTH_HRPRO') === '1') {
        location.reload();
    }
});