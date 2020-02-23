var workLogData;
var groupTypes = [null, 2, 3];
var currGroupIndex = 0;
var toastObj = null;
var toastShort = 5000;
var toastLong = 15000;
var maxDetailsLng = 20;
var toastClasses = 'cyan lighten-4';
var viewModel;
var searchStack = [];

var dateFromPickers = {
    semiAutomaticDateFrom: null
}

$(function () {
    var elems = document.querySelectorAll('.collapsible');
    var instances = M.Collapsible.init(elems, {
    });
   

    if (loggedIn) {
        getWorkLogs();
        initWorkLogChart([], []);
    }
    else {
        $('.spinner').hide();
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
    var openedIndex = viewModel.userSettings().openedAccordion;
    if (openedIndex != null) {
        var instance = M.Collapsible.getInstance($('.collapsible')[0]);
        instance.open(openedIndex);
    }
})


function getWorkLogs(forDate, groupIndex) {
    if (groupIndex) {
        if (groupIndex < currGroupIndex) {
            removeBreadCrumb(forDate);
        }
        currGroupIndex = groupIndex;
    }
    else {
        currGroupIndex = 0;
        breadCrumbs = [];
    }

    refreshBreadCrumbs();

    getWorkLogsAction(forDate);
}

function getWorkLogsAction(forDate) {
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
        M.Datepicker.init(document.querySelectorAll('#semiAutomaticEntry .datepicker.dateFrom'), {
            autoClose: true,
            format: 'dd.mm.yyyy',
            firstDay: 1,
            showDaysInNextAndPreviousMonths: true,
            showClearBtn: true,
            defaultDate: new Date(),
            setDefaultDate: true,
            onSelect: onDateFromSelect.bind(viewModel.semiAutomaticEntry)
        });
        M.Datepicker.init(document.querySelectorAll('#semiAutomaticEntry .datepicker.dateTo'), {
            autoClose: true,
            format: 'dd.mm.yyyy',
            firstDay: 1,
            showDaysInNextAndPreviousMonths: true,
            showClearBtn: true,
            onSelect: onDateToSelect.bind(viewModel.semiAutomaticEntry)
        });

        M.Datepicker.init(document.querySelectorAll('#customEntry .datepicker.dateFrom'), {
            autoClose: true,
            format: 'dd.mm.yyyy',
            firstDay: 1,
            showDaysInNextAndPreviousMonths: true,
            showClearBtn: true,
            defaultDate: new Date(),
            setDefaultDate: true,
            onSelect: onDateFromSelect.bind(viewModel.semiAutomaticEntry)
        });
        M.Datepicker.init(document.querySelectorAll('#customEntry .datepicker.dateTo'), {
            autoClose: true,
            format: 'dd.mm.yyyy',
            firstDay: 1,
            showDaysInNextAndPreviousMonths: true,
            showClearBtn: true,
            onSelect: onDateToSelect.bind(viewModel.customEntry)
        });

        //var instances = M.Datepicker.init(document.querySelectorAll('.datepicker.dateTo'), {
        //    autoClose: true,
        //    format: 'dd.mm.yyyy',
        //    firstDay: 1,
        //    showDaysInNextAndPreviousMonths: true,
        //    showClearBtn: true,
        //    onSelect: onDateToSelect
        //});

        $('.modal').modal();

        console.log(instances);
    });
}

function onDateFromSelect() {
    var dateFromPicker = M.Datepicker.getInstance($(selectorDateFrom)[0]);
    var dateToPicker = M.Datepicker.getInstance($(selectorDateTo)[0]);

    //dateFromPicker.setDate(dateFrom);
    //dateToPicker.setDate(dateTo);
}

function onDateToSelect() {

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

function generateToastObjs(msg, title, noNewEntryMsg) {
    if (msg.length > maxDetailsLng) {
        toastObj = {
            html: '<span style="color: black; text-align: right">' + title + ' je unesen <b>' + msg.length + '</b> puta</span>',
            classes: toastClasses,
            displayLength: toastLong
        };

    }
    else if (msg.length > 0) {
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
    if (msg.length > 0)
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



function onAccordionOpen(index) {
    //console.log(this);
    //console.log(index);
    viewModel.userSettings().openedAccordion = index;
    saveSettings();
    //M.Collapsible.getInstance($('.collapsible')[0]).open(2);
}
