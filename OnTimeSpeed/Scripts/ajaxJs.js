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
            $('.spinner.main').hide();            
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
            $('.spinner.main').hide();
        }
    });
}

$(document).ajaxError(function (event, request, settings, thrownError) {
    //debugger;
    lastAjaxRequest = new Date();
    //console.log(thrownError);

    if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') === '1') {
        $('.spinner.main').show();
        sessionStorage.setItem("reauthPreformed", 1);
        recoverUser();
        //location.reload();
    }
    if (request.getResponseHeader('REQUIRES_AUTH_HRPRO') === '1') {
        $('.spinner.main').show();
        sessionStorage.setItem("reauthPreformed", 1);
        recoverUser();
        //location.reload();
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
    lastAjaxRequest = new Date();

    if (request.getResponseHeader('REQUIRES_AUTH_ONTIME') === '1') {
        $('.spinner.main').show();
        sessionStorage.setItem("reauthPreformed", 1);
        recoverUser();
        //location.reload();
    }
    if (request.getResponseHeader('REQUIRES_AUTH_HRPRO') === '1') {
        $('.spinner.main').show();
        sessionStorage.setItem("reauthPreformed", 1);
        recoverUser();
        //location.reload();
    }
});