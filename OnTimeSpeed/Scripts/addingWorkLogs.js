
function addLunch() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddLunchToToday'
    }, function (msg) {
        generateToastObjs(msg, "Ručak", "Za sve dane je već unesen ručak");
    });
}

function addHolidays() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddHolidays'
    }, function (msg) {
        generateToastObjs(msg, "Praznik", "Nije pronađen novi GO za unijeti");
    });
}

function addVacations() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddVacations'
    }, function (msg) {
        generateToastObjs(msg, "GO", "Nije pronađen novi GO za unijeti");
    });
}

function addPaidLeaves() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddPaidLeaves'
    }, function (msg) {
        generateToastObjs(msg, "Plaćeni dopust", "Nije pronađen novi plaćeni dopust za unijeti");
    });
}

function addNewWorkLog() {
    var data = {
        itemId: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().Id : 0,
        itemType: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().TypeString : '',
        workTypeId: viewModel.activeWorkType && viewModel.activeWorkType() ? viewModel.activeWorkType().id : 0,
        amount: this.workAmount(),
        dateFromStr: $('#customEntry .dateFrom').val(),
        dateToStr: $('#customEntry .dateTo').val(),
        description: this.description()
    }

    if (validateForm.apply(data)) {
        $('.spinner').show();
        ajaxPOST({
            url: '/Home/AddCustom',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Work log", "Nije bilo mjesta za unijeti log");
        });
    }
    else {
        var tempToast = {
            html: '<span style="color: black">Nedostaju podaci u formi za unos</span>',
            classes: toastClasses,
            displayLength: toastLong
        };
        M.toast(tempToast);
    }

    console.log(data);
}


function validateForm() {
    var isValid = true;

    if (!this.itemId) {
        isValid = false;
    }
    if (!this.workTypeId) {
        isValid = false;
    }
    if (!this.amount) {
        isValid = false;
    }
    if (!this.dateFromStr) {
        isValid = false;
    }
    return isValid;
}