var workLogData;
var groupTypes = [null, 2, 3];
var currGroupIndex = 0;
var breadCrumbs = [];
var toastObj = null;
var toastShort = 5000;
var toastLong = 15000;
var maxDetailsLng = 20;
var toastClasses = 'cyan lighten-4';
var viewModel;
var searchStack = [];

$(function () {
    if (loggedIn) {
        getWorkLogs();
        initWorkLogChart([], []);
    }
    $('.container').show();
    $('#addLunchBtn').click(addLunch);
    $('#addHolidaysButton').click(addHolidays);
    $('#addVacationButton').click(addVacations);
    $('#addPaidLeaveButton').click(addPaidLeaves);


    viewModel = new ViewModel();
    ko.applyBindings(viewModel);
    //ko.applyBindings(viewModel, $('.modal'));

    loadSettings();
})

function getWorkLogs(forDate, groupIndex) {
    if (groupIndex) {
        if (groupIndex < currGroupIndex) {
            removeBreadCrumb(forDate);
            //for (var i = 0; i < (currGroupIndex - groupIndex); i++) {
            //    removeBreadCrumb();
            //}
        }
        currGroupIndex = groupIndex;
    }
    else {
        currGroupIndex = 0;
        breadCrumbs = [];
    }

    refreshBreadCrumbs();

    ajaxGET({
        url: '/Home/getWorkLogs',
        data: {
            forDate: forDate,
            groupType: groupTypes[currGroupIndex]
        }
    }, function (msg) {
        workLogData = msg;
        initWorkLogChart(msg.Categories_names, msg.Series)

        $('.spinner').hide();

        if (toastObj) {
            M.toast(toastObj);
            toastObj = null;
        }
        var elems = document.querySelectorAll('.datepicker');
        var instances = M.Datepicker.init(elems, {
            autoClose: true,
            format: 'dd.mm.yyyy',
            firstDay: 1,
            showDaysInNextAndPreviousMonths: true,
            showClearBtn: true
        });

        $('.modal').modal();

        console.log(instances);
    });
}

function initWorkLogChart(categories, series) {
    var tooltipFormatter = function () {
        var txt = '<table class="totalsWorkLogTable striped"><tr><th>Predmet</th><th>Work type</th><th>Opis</th><th>Broj sati</th></tr>';
        var total = 0;
        for (var prop in workLogData.WorkLogs) {
            if (Object.prototype.hasOwnProperty.call(workLogData.WorkLogs, prop)) {
                if (prop == this.x) {
                    for (var i = 0; i < workLogData.WorkLogs[prop].length; i++) {
                        var log = workLogData.WorkLogs[prop][i];
                        txt += '<tr><td>' + log.ItemName + '</td><td>' + log.WorkType + '</td><td>' + log.Descripton + '</td><td>' + log.Amount + ' hrs' + '</td></tr>';
                        total += log.Amount;
                    }
                }
            }
        }
        txt += '<tr class="totalWorkLog"><td>UKUPNO</td><td></td><td></td><td>' + total + ' hrs' + '</td></tr>';
        txt += '</table>'
        //for (var i = 0; i < workLogData.WorkLogs.length; i++) {
        //    if ()
        //    var log = workLogData.WorkLogs[i];
        //    txt += log.ItemName + ' ' + log.Amount + 'hrs' + '<br>';
        //}
        return txt;
    };

    if (currGroupIndex == null || currGroupIndex < 2) {
        tooltipFormatter = null;
    }
    $('#workLogChart').html('');
    Highcharts.chart('workLogChart', {
        chart: {
            type: 'column',
            events: {
                click: function (e) {
                    var a = this;
                }
            }
        },
        title: {
            text: 'OnTime ispunjenost'
        },
        xAxis: {
            categories: categories
        },
        credits: {
            enabled: false
        },
        yAxis: [{
            min: 0,
            max: currGroupIndex == 0 ? 200 : currGroupIndex == 1 ? 50 : 10,
            minorTickInterval: currGroupIndex == 0 ? 25 : currGroupIndex == 1 ? 5 : 1,
            title: {
                text: 'Broj sati'
            }
        }],
        legend: {
            shadow: false
        },
        tooltip: {
            shared: true,
            formatter: tooltipFormatter,
            useHTML: true
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0
            },
            series: {
                point: {
                    events: {
                        click: function (e) {
                            var a = this;
                            if (currGroupIndex < 2) {
                                currGroupIndex++;
                                getWorkLogs(this.category, currGroupIndex);
                                addBreadCrumb(this.category);
                            }
                        }
                    }
                }
            }
        },
        series: [{
            name: 'Planirano',
            color: 'rgb(200, 230, 201)',
            data: series.length > 0 ? series[0].ValuesArray : [],
            pointPadding: 0.3,
            dataLabels: {
                enabled: false
            },
        }, {
            name: 'Ostvareno',
            color: 'rgba(102, 187, 106, 0.9)',
            data: series.length > 1 ? series[1].ValuesArray : [],
            pointPadding: 0.4,
            dataLabels: {
                enabled: true,
                formatter: function (a, b) {
                    var ostvareno = this.y;
                    var planirano = 0;
                    for (var i = 0; i < this.series.xAxis.categories.length; i++) {
                        if (this.series.xAxis.categories[i] == this.x) {
                            planirano = this.series.yAxis.series[0].yData[i];
                            break;
                        }
                    }
                    if (planirano == 0)
                        return "N/A";
                    else
                        return (Math.round((ostvareno / planirano) * 100)) + '%';
                }
            }
        }]
    });
}

function addBreadCrumb(forDate) {
    breadCrumbs.push(forDate);
    refreshBreadCrumbs();
}

function removeBreadCrumb(toString) {
    while (breadCrumbs.length > 0 && breadCrumbs[breadCrumbs.length - 1] != toString) {
        breadCrumbs.pop();
    }
}

function refreshBreadCrumbs() {
    $('#breadCrumbs').html('<a href="#!" onclick="getWorkLogs()" class="breadcrumb clickable">Sve</a>');

    for (var i = 0; i < breadCrumbs.length; i++) {
        var html = '<a href="#!" onclick="getWorkLogs(#forDate1#,' + (i + 1) + ')" class="breadcrumb clickable">#forDate2#</a>';
        if (i == breadCrumbs.length - 1)
            html = '<a href="#!" class="breadcrumb nonClickable">#forDate2#</a>';

        html = html.replace('#forDate1#', '\'' + breadCrumbs[i] + '\'');
        html = html.replace('#forDate2#', breadCrumbs[i]);

        $('#breadCrumbs').append(html)
    }
}

function generateToastObjs(msg, title, noNewEntryMsg) {
    if (msg.length > maxDetailsLng) {
        toastObj = {
            html: '<span style="color: black; text-align: right">' + title + ' je unesen <b>' + msg.length + '</b> puta</span>',
            classes: toastClasses,
            displayLength: toastLong
        };

    }
    else if (msg.length > 1) {
        var str = '<span style="font-size:12px">';
        for (var i = 0; i < msg.length; i++) {
            str += '</br>' + msg[i];
        }
        str += '</span>';
        toastObj = {
            html: '<span style="color: black; text-align: right">' + title + ' je unesen <b>' + msg.length + '</b> puta za sljedeće datume:' + str + '</span>',
            classes: toastClasses,
            displayLength: toastLong
        };
    }
    if (msg.length > 1)
        getWorkLogs();
    else {
        $('.spinner').hide();
        toastObj = {
            html: '<span style="color: black; text-align: right">' + noNewEntryMsg + '</span>',
            classes: toastClasses,
            displayLength: toastShort
        };
        M.toast(toastObj);
    }
}

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

function preselectRange(range, a, b) {
    var parentId = this.elementId;
    var selectorDateFrom = '#' + parentId + ' .dateFrom';
    var selectorDateTo = '#' + parentId + ' .dateTo';

    var dateFromPicker = M.Datepicker.getInstance($(selectorDateFrom)[0]);
    var dateToPicker = M.Datepicker.getInstance($(selectorDateTo)[0]);

    var dateFrom = moment();
    var dateTo = moment();
    switch (range) {
        case 'lastM':
            var d = dateFrom.subtract(1, 'months')._d;
            d = moment(new Date(d.getFullYear(), d.getMonth() + 1, 1));
            dateFrom = toDate(d);
            dateTo = toDate(d.add(1, 'months').subtract(1, 'days'));
            break;

        case 'lastW':
            var d = dateFrom.subtract(7, 'days')._d;
            while (d.getDay() > 1) {
                d = moment(d).subtract(1, 'days')._d;
            }
            dateFrom = d;
            dateTo = toDate(moment(d).add(7, 'days').subtract(1, 'days'));
            break;

        case 'thisW':
            var d = dateFrom._d;
            while (d.getDay() > 1) {
                d = moment(d).subtract(1, 'days')._d;
            }
            dateFrom = d;
            dateTo = toDate(moment(d).add(7, 'days').subtract(1, 'days'));
            break;

        case 'thisM':
            var d = dateFrom._d;
            d = moment(new Date(d.getFullYear(), d.getMonth() + 1, 1));
            dateFrom = toDate(d);
            dateTo = toDate(d.add(1, 'months').subtract(1, 'days'));
            break;

        default:
            break;

    }
    var d = new Date();

    dateFromPicker.setDate(dateFrom);
    dateToPicker.setDate(dateTo);
    $(selectorDateFrom).val(dateFromPicker.toString());
    $(selectorDateTo).val(dateToPicker.toString());
}

function toDate(mom) {
    return new Date(mom._d.getTime());
}

function selectWorkItem() {
    for (var i = 0; i < viewModel.userSettings().chosenWorkItems().length; i++) {
        viewModel.userSettings().chosenWorkItems()[i].active(false);
    }
    this.active(!this.active());
    viewModel.activeWorkItem(this);
    $('#workItemsList')
}

function selectWorkType() {
    for (var i = 0; i < viewModel.chosenWorkTypes().length; i++) {
        viewModel.chosenWorkTypes()[i].active(false);
    }
    this.active(!this.active());
    viewModel.activeWorkType(this);
}

function searchItems(searchStr) {
    $('#addMoreWorkItemsModal .spinner').show();

    ajaxGET({
        url: '/Home/SearchWorkItems',
        data: {
            searchStr: searchStr
        }
    }, function (msg) {
        $('#addMoreWorkItemsModal .spinner').hide();

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

function addMoreWorkItems() {
    M.Modal.getInstance($('#addMoreWorkItemsModal')[0]).open();
    $('#searchWorkItems').focus();
}

function addMoreWorkTypes() {
    M.Modal.getInstance($('#addMoreWorkTypesModal')[0]).open();
    $('#searchWorkTypes').focus();
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

function searchWorkItemsPush() {
    searchStack.push('0');
    setTimeout(function () {
        searchStack.pop();
        var searchStr = $('#searchWorkItems').val();
        if (searchStack.length == 0 && searchStr.length > 4)
            searchItems(searchStr);
    }, 1000);
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

function addNewWorkLog() {
    var data = {
        itemId: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().Id : 0,
        itemType: viewModel.activeWorkItem && viewModel.activeWorkItem() ? viewModel.activeWorkItem().TypeString : '',
        workTypeId: viewModel.activeWorkType && viewModel.activeWorkType() ? viewModel.activeWorkType().id : 0,
        amount: this.workAmount(),
        dateFromStr: $('#customEntry .dateFrom').val(),
        dateToStr: $('#customEntry .dateTo').val(),
        description: this.description()
    }

    if (validateForm.apply(data)) {
        $('.spinner').show();
        ajaxPOST({
            url: '/Home/AddCustom',
            data: data
        }, function (msg) {
            generateToastObjs(msg, "Work log", "Nije bilo mjesta za unijeti log");
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

    console.log(data);
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

function increaseWorkAmount() {
    if (this.workAmount() <= 0)
        this.workAmount(0);

    this.workAmount(Math.min(this.workAmount() + 0.5), 8);
}

function decreaseWorkAmount() {
    if (this.workAmount() >= 8)
        this.workAmount(8);

    this.workAmount(Math.max(0, this.workAmount() - 0.5));
}

function maxWorkAmount() {
    if (this.workAmount() > 8) {
        this.workAmount(0);
    }
    else {
        this.workAmount(99);
    }
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