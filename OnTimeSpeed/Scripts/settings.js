function loadSettings() {
    var settingsJson = localStorage.getItem("userSettings");
    if (settingsJson) {
        var settings = JSON.parse(settingsJson);

        for (var i = 0; i < settings.chosenWorkItems.length; i++) {
            settings.chosenWorkItems[i].active = ko.observable(false);
        }

        viewModel.userSettings().chosenWorkTypes(settings.chosenWorkTypes);
        viewModel.userSettings().chosenWorkItems(settings.chosenWorkItems);
        viewModel.userSettings().openedAccordion = settings.openedAccordion;
        viewModel.userSettings().templates(settings.templates);

        for (var i = 0; i < viewModel.allWorkTypes().length; i++) {
            var item = viewModel.allWorkTypes()[i];
            for (var j = 0; j < viewModel.userSettings().chosenWorkTypes().length; j++) {
                if (item.id == viewModel.userSettings().chosenWorkTypes()[j].id) {
                    item.chosen(true);
                }
            }
        }

    }
}

function saveSettings() {
    var settings = ko.mapping.toJS(viewModel.userSettings);
    localStorage.setItem("userSettings", JSON.stringify(settings));
}