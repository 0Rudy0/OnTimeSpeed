

function selectWorkType() {
    for (var i = 0; i < viewModel.chosenWorkTypes().length; i++) {
        viewModel.chosenWorkTypes()[i].active(false);
    }
    this.active(!this.active());
    viewModel.activeWorkType(this);
}

function addMoreWorkTypes() {
    M.Modal.getInstance($('#addMoreWorkTypesModal')[0]).open();
    $('#searchWorkTypes').focus();
}


function choseWorkType() {
    this.chosen(!this.chosen());

    if (this.chosen())
        viewModel.userSettings().chosenWorkTypes.push(this);
    else {
        for (var i = 0; i < viewModel.chosenWorkTypes().length; i++) {
            if (this.id == viewModel.userSettings().chosenWorkTypes()[i].id) {
                viewModel.userSettings().chosenWorkTypes.splice(i, 1);
                break;
            }
        }
    }
    saveSettings();
}

function unchooseWorkType() {
    for (var i = 0; i < viewModel.chosenWorkTypes().length; i++) {
        if (this.id == viewModel.userSettings().chosenWorkTypes()[i].id) {
            viewModel.userSettings().chosenWorkTypes.splice(i, 1);
            break;
        }
    }
    for (var i = 0; i < viewModel.allWorkTypes().length; i++) {
        if (this.id == viewModel.allWorkTypes()[i].id) {
            viewModel.allWorkTypes()[i].chosen(false);
            break;
        }
    }
    saveSettings();
}