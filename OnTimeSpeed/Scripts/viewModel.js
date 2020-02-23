function ViewModel() {
    var self = this;

    self.userSettings = ko.observable({
        chosenWorkTypes: ko.observableArray([]),
        chosenWorkItems: ko.observableArray([]),
        templates: ko.observableArray([]),
        openedAccordion: null
    });

    self.breadCrumbs = ko.observableArray([]);
    self.ontimeUser = ko.observable(JSON.parse(onTimeUserSerialized));
    self.hrproUser = ko.observable(JSON.parse(hrproUserSerialized));

    self.activeWorkItem = ko.observable();
    self.activeWorkType = ko.observable();

    self.searchResultWorkItems = ko.observableArray([]);
    self.allWorkTypes = ko.observableArray(allWorkTypes);

    self.workTypeSearchStr = ko.observable();

    self.searchResultWorkTypes = ko.computed(function () {
        return self.allWorkTypes().filter(function (type) {
            if (!self.workTypeSearchStr() || type.name.toLowerCase().indexOf(self.workTypeSearchStr().toLowerCase()) !== -1)
                return type;
        });
    }, this);

    self.chosenWorkTypes = ko.computed(function () {
        var result = [];
        for (var i = 0; i < self.allWorkTypes().length; i++) {
            var item = self.allWorkTypes()[i];
            for (var j = 0; j < self.userSettings().chosenWorkTypes().length; j++) {
                if (item.id == self.userSettings().chosenWorkTypes()[j].id) {
                    result.push(item);
                    break;
                }
            }
        }
        return result;
    });

    self.automaticEntry = ko.observable({
        styleClass: 'cyan',
        styleType: 'darken-3',
        textClass: 'white-text',
        textType: '',
        buttonType: 'darken-4',
        titleType: 'darken-4'
    });

    self.semiAutomaticEntry = ko.observable({
        elementId: 'semiAutomaticEntry',
        styleClass: 'teal',
        styleType: 'darken-3',
        textClass: 'grey-text',
        textType: 'text-darken-4',
        buttonType: 'darken-4',
        titleType: 'darken-4',
        preselectsType: 'darken-2',
        workAmount: ko.observable(1),
        description: ko.observable('')
    });

    self.customEntry = ko.observable({
        elementId: 'customEntry',
        styleClass: 'blue-grey',
        styleType: 'darken-3',
        textClass: 'white-text',
        textType: '',
        buttonType: 'darken-4',
        titleType: 'darken-4',
        preselectsType: 'darken-2',
        workAmount: ko.observable(1),
        description: ko.observable('')
    });
}