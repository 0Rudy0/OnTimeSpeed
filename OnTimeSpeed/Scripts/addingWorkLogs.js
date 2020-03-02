
function generateToastObjs(msg, title, noNewEntryMsg) {
    var realCounter = 0;
    for (var i = 0; i < msg.length; i++) {
        if (msg[i].indexOf("Dan je već popunjen") < 0)
            realCounter++;
    }
    if (msg.length > maxDetailsLng) {
        toastObj = {
            html: '<span style="color: black; text-align: right">' + title + ' <b>' + realCounter + '</b> puta</span><button class="btn-flat toast-action" onclick="closeNotification(this)">X</button>',
            classes: toastClasses,
            displayLength: toastLong
        };

    }
    else if (msg.length > 0) {
        var str = '<span style="font-size:12px">';
        for (var i = 0; i < msg.length; i++) {
            str += '</br>' + msg[i];
        }
        str += '</span>';
        toastObj = {
            html: '<span style="color: black; text-align: right">' + title + ' <b>' + realCounter + '</b> puta za sljedeće datume:' + str + '</span><button class="btn-flat toast-action" onclick="closeNotification(this)">X</button>',
            classes: toastClasses,
            displayLength: toastLong
        };
    }
    if (msg.length > 0)
        getWorkLogs();
    else {
        $('.spinner').hide();
        toastObj = {
            html: '<span style="color: black; text-align: right">' + noNewEntryMsg + '</span><button class="btn-flat toast-action" onclick="closeNotification(this)">X</button>',
            classes: toastClasses,
            displayLength: 99999999
        };
        M.toast(toastObj);
    }
}


//AUTOMATIC
function addLunch() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddLunchToToday',
        data: {
            amount: viewModel.userSettings().lunchWorkAmount()
        }
    }, function (msg) {
        generateToastObjs(msg, "Ručak je unesen", "Za sve dane je već unesen ručak ili nije bilo mjesta za dodati ručak");
    });
}

function addHolidays() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddHolidays'
    }, function (msg) {
        generateToastObjs(msg, "Praznici su uneseni", "Nije pronađen novi praznik za unijeti");
    });
}

function addVacations() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddVacations'
    }, function (msg) {
        generateToastObjs(msg, "GO je unesen", "Nije pronađen novi GO za unijeti");
    });
}

function addPaidLeaves() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddPaidLeaves'
    }, function (msg) {
        generateToastObjs(msg, "Plaćeni dopust je unesen", "Nije pronađen novi plaćeni dopust za unijeti");
    });
}

function addAllAutomatic() {
    $('.spinner').show();
    ajaxPOST({
        url: '/Home/AddAllAutomatic',
        data: {
            amount: viewModel.userSettings().lunchWorkAmount()
        }
    }, function (msg) {
        generateToastObjs(msg, "Novi unosi su dodani", "Ništa novo nije bilo za unijeti");
    });
}


//SEMI AUTOMATIC
function addLunchSemi() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }

    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddLunch',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Ručak", "Nije bilo mjesta za unijeti ručak");
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
}

function addSickLeave() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }

    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddSickLeave',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Bolovanje", "Nije bilo mjesta za unijeti bolovanje");
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
}

function addInternalMeeting() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }

    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddInternalMeeting',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Interni sastanak", "Nije bilo mjesta za unijeti interni sastanak");
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
}

function addOnTimeEntry() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }

    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddOnTimeEntry',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "onTime unos sati", "Nije bilo mjesta za unijeti ontime unos sati");
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
}

function addColegueSupport() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }
    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddColegueSupport',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Podrška kolegi", "Nije bilo mjesta za unijeti podršku kolegi");
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
}

function addEducationButton() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDaySemiAutomatic()
    }
    if (validateSemiAutomaticForm.apply(data)) {
        ajaxPOST({
            url: '/Home/AddEducation',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Edukacija/školovanje", "Nije bilo mjesta za unijeti edukaciju");
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
}

function validateSemiAutomaticForm() {
    var isValid = true;

    if (!this.amount) {
        isValid = false;
    }
    if (!this.dateFromStr) {
        isValid = false;
    }
    return isValid;
}


//CUSTOM
function addNewWorkLog() {
    var data = {
        itemId: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().Id : 0,
        itemName: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().Name : 0,
        itemType: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().TypeString : '',
        workTypeId: viewModel.activeWorkType && viewModel.activeWorkType() ? viewModel.activeWorkType().id : 0,
        amount: this.workAmount(),
        dateFromStr: $('#customEntry .dateFrom').val(),
        dateToStr: $('#customEntry .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDayCustom()
    }

    addWorkLogDo.apply(data);
}

function addNewTemplateWorkLog() {
    var data = {
        itemId: this.workItem.Id,
        itemName: this.workItem.Name,
        itemType: this.workItem.TypeString,
        workTypeId: this.workType.id,
        amount: this.workAmount(),
        dateFromStr: $('#templates .dateFrom').val(),
        dateToStr: $('#templates .dateTo').val(),
        description: this.description(),
        ignoreFullDays: viewModel.userSettings().overrideFullDayCustom()
    }

    addWorkLogDo.apply(data);
}

function validateForm() {
    var isValid = true;
    var invalidFields = '';

    if (!this.itemId) {
        isValid = false;
        invalidFields += '<br>Work item';
    }
    if (!this.workTypeId) {
        isValid = false;
        invalidFields += '<br>Work type';
    }
    if (!this.amount) {
        isValid = false;
        invalidFields += '<br>Iznos sati';
    }
    if (!this.dateFromStr) {
        isValid = false;
        invalidFields += '<br>Datum od';
    }
    return isValid;
}

function addWorkLogDo() {
    var data = this;

    if (validateForm.apply(data)) {
        $('.spinner').show();
        ajaxPOST({
            url: '/Home/AddCustom',
            data: data
        }, onAddWorkLogSuccess.bind(this));
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

function onAddWorkLogSuccess(msg) {
    var data = this;
    generateToastObjs(msg, this.itemName + ' - ', "Nije bilo mjesta za unijeti log");
    saveSettings();
}