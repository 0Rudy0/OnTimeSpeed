function addNewTemplate() {
    viewModel.userSettings().templates.push({
        workItem: viewModel.activeWorkItem(),
        workType: viewModel.activeWorkType(),
        description: ko.observable(viewModel.customEntry().description()),
        workAmount: ko.observable(viewModel.customEntry().workAmount())
    });

    saveSettings();
}

function removeTemplate() {
    viewModel.userSettings().templates.remove(this);
    saveSettings();
}