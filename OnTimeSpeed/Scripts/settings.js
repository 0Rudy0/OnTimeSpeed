function openSettings() {
    M.Modal.getInstance($('#settingsModal')[0]).open();
}

function loadSettings() {
    var settingsJson = localStorage.getItem("userSettings");
    if (settingsJson) {
        var settings = JSON.parse(settingsJson);

        for (var i = 0; i < settings.chosenWorkItems.length; i++) {
            settings.chosenWorkItems[i].active = ko.observable(false);
        }

        viewModel.userSettings().chosenWorkTypes(settings.chosenWorkTypes);
        viewModel.userSettings().chosenWorkItems(settings.chosenWorkItems);
        viewModel.userSettings().mergeAutoButtons(settings.mergeAutoButtons);
        viewModel.userSettings().lunchWorkAmount(settings.lunchWorkAmount ? settings.lunchWorkAmount : 0.5);
        viewModel.userSettings().showLunch(settings.showLunch);
        viewModel.userSettings().showEducation(settings.showEducation);
        viewModel.userSettings().showCollegueSupport(settings.showCollegueSupport);
        viewModel.userSettings().showOnTime(settings.showOnTime);
        viewModel.userSettings().showInternalMeeting(settings.showInternalMeeting);
        viewModel.userSettings().showSickLeave(settings.showSickLeave);
        viewModel.userSettings().overrideFullDayCustom(settings.overrideFullDayCustom);
        viewModel.userSettings().overrideFullDaySemiAutomatic(settings.overrideFullDaySemiAutomatic);

        viewModel.userSettings().openedAccordion = settings.openedAccordion;
        for (var i = 0; i < settings.templates.length; i++) {
            viewModel.userSettings().templates.push({
                workItem: settings.templates[i].workItem,
                workType: settings.templates[i].workType,
                description: ko.observable(settings.templates[i].description),
                workAmount: ko.observable(settings.templates[i].workAmount)
            })
        }

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