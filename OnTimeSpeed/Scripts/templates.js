function addNewTemplate() {
     var data = {
        workItem: viewModel.activeWorkItem(),
        workType: viewModel.activeWorkType(),
        description: ko.observable(viewModel.customEntry().description()),
        workAmount: ko.observable(viewModel.customEntry().workAmount()),   
        collapsed: ko.observable(false)
    }

    if (!data.workItem) {
         var tempToast = {
            html: '<span style="color: black">Predmet treba biti odabran</span>' + closeBtnHtml,
            classes: toastClasses,
            displayLength: toastShort
        };
        M.toast(tempToast); 
        return;
    }
    else {

        viewModel.userSettings().templates.push(data);
        saveSettings();

        var tempToast = {
            html: '<span style="color: black">Predložak dodan</span>' + closeBtnHtml,
            classes: toastClasses,
            displayLength: toastShort
        };
        M.toast(tempToast); 

        if (viewModel.userSettings().templates.length == 0) {
         M.Datepicker.init(document.querySelectorAll('#templates .datepicker.dateFrom'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                defaultDate: new Date(),
                setDefaultDate: true,
                onSelect: onDateFromSelect.bind(viewModel.templateModel())
            });
            M.Datepicker.init(document.querySelectorAll('#templates .datepicker.dateTo'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                onSelect: onDateToSelect.bind(viewModel.customEntry())
            });
        }

        /*if (validateForm.apply(data)) {    
            viewModel.userSettings().templates.push(data);
            saveSettings();
        }
        else {
            var tempToast = {
                html: '<span style="color: black">Nedostaju podaci u formi za unos</span>',
                classes: toastClasses,
                displayLength: toastLong
            };
            M.toast(tempToast);        
        }*/
    }
}

function removeTemplate() {
    viewModel.userSettings().templates.remove(this);
    saveSettings();
}

function collapseTemplate() {
    this.collapsed(true);
    saveSettings();
}

function expandTemplate() {
    this.collapsed(false);
    saveSettings();
}