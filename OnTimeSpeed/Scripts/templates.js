function addNewTemplate() {
     var data = {
        workItem: viewModel.activeWorkItem(),
        workType: viewModel.activeWorkType(),
        description: ko.observable(viewModel.customEntry().description()),
        workAmount: ko.observable(viewModel.customEntry().workAmount())
    }

    viewModel.userSettings().templates.push(data);
    saveSettings();

    var tempToast = {
        html: '<span style="color: black">Predložak dodan</span><button class="btn-flat toast-action" onclick="closeNotification(this)">X</button>',
        classes: toastClasses,
        displayLength: toastShort
    };
    M.toast(tempToast); 

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

function removeTemplate() {
    viewModel.userSettings().templates.remove(this);
    saveSettings();
}