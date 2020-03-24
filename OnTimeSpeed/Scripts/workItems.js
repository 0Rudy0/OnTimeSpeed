
function searchWorkItemsPush() {
    searchStack.push('0');
    setTimeout(function () {
        searchStack.pop();
        var searchStr = $('#searchWorkItems').val();
        if (searchStack.length == 0 && searchStr.length > 2)
            searchItems(searchStr);
    }, 1000);
}

function searchItems(searchStr) {
    $('#addMoreWorkItemsModal .spinner.main').show();

    ajaxGET({
        url: '/Home/SearchWorkItems',
        data: {
            searchStr: searchStr
        }
    }, function (msg) {
        $('#addMoreWorkItemsModal .spinner.main').hide();

        viewModel.searchResultWorkItems.splice(0);
        for (var i = 0; i < msg.length; i++) {
            msg[i].active = ko.observable(false);
            msg[i].chosen = ko.observable(false);
            for (var j = 0; j < viewModel.userSettings().chosenWorkItems().length; j++) {
                if (msg[i].Id == viewModel.userSettings().chosenWorkItems()[j].Id) {
                    msg[i].chosen(true);
                    break;
                }
            }
            viewModel.searchResultWorkItems.push(msg[i]);
        }
    });
}

function selectWorkItem() {
    for (var i = 0; i < viewModel.userSettings().chosenWorkItems().length; i++) {
        viewModel.userSettings().chosenWorkItems()[i].active(false);
    }
    this.active(!this.active());
    viewModel.activeWorkItem(this);
    $('#workItemsList')
}


function choseWorkItem() {
    this.chosen(!this.chosen());

    if (this.chosen())
        viewModel.userSettings().chosenWorkItems.push(this);
    else {
        for (var i = 0; i < viewModel.userSettings().chosenWorkItems().length; i++) {
            if (this.Id == viewModel.userSettings().chosenWorkItems()[i].Id) {
                viewModel.userSettings().chosenWorkItems.splice(i, 1);
                break;
            }
        }
    }

    saveSettings();
}

function unchooseWorkItem() {
    for (var i = 0; i < viewModel.userSettings().chosenWorkItems().length; i++) {
        if (this.Id == viewModel.userSettings().chosenWorkItems()[i].Id) {
            viewModel.userSettings().chosenWorkItems.splice(i, 1);
            break;
        }
    }
    saveSettings();
}


function addMoreWorkItems() {
    M.Modal.getInstance($('#addMoreWorkItemsModal')[0]).open();
    $('#searchWorkItems').focus();
}