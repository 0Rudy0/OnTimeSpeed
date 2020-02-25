
//AUTOMATIC
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


//SEMI AUTOMATIC
function addSickLeave() {
    $('.spinner').show();
    var data = {
        amount: this.workAmount(),
        dateFromStr: $('#semiAutomaticEntry .dateFrom').val(),
        dateToStr: $('#semiAutomaticEntry .dateTo').val(),
        description: this.description()
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
        description: this.description()
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
        description: this.description()
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
        description: this.description()
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
        description: this.description()
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
        description: this.description()
    }

    addWorkLogDo.apply(data);
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