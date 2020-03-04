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
        }
    });
}

$(document).ajaxError(function (event, request, settings, thrownError) {
    //debugger;

    if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') === '1') {
        $('.spinner').show();
        sessionStorage.setItem("reauthPreformed", 1);
        location.reload();
    }
    if (request.getResponseHeader('REQUIRES_AUTH_HRPRO') === '1') {
        $('.spinner').show();
        sessionStorage.setItem("reauthPreformed", 1);
        location.reload();
    }
    else if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') != '1' && request.getResponseHeader('REQUIRES_AUTH_HRPRO') != '1') {
        var tempToast = {
            html: '<span style="color: white">Dogodila se greška</span>' + closeBtnHtml,
            classes: 'red darken-2',
            displayLength: toastLong
        };
        M.toast(tempToast);
    }
});

$(document).ajaxSuccess(function (event, request, settings) {
    //debugger;

    if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') === '1') {
        $('.spinner').show();
        sessionStorage.setItem("reauthPreformed", 1);
        location.reload();
    }
    if (request.getResponseHeader('REQUIRES_AUTH_HRPRO') === '1') {
        $('.spinner').show();
        sessionStorage.setItem("reauthPreformed", 1);
        location.reload();
    }
});