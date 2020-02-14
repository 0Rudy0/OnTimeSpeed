var workLogData;
var groupTypes = [null, 2, 3];
var currGroupIndex = 0;
var breadCrumbs = [];
var toastObj = null;
var toastShort = 5000;
var toastLong = 15000;
var maxDetailsLng = 20;
var toastClasses = 'cyan lighten-4';

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

    var elems = document.querySelectorAll('.datepicker');
    var instances = M.Datepicker.init(elems, {
        autoClose: true,
        format: 'dd.mm.yyyy',
        firstDay: 1,
        showDaysInNextAndPreviousMonths: true,
        showClearBtn: true
    });
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

    $.ajax({
        url: appName + '/Home/getWorkLogs',
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {
            forDate: forDate,
            groupType: groupTypes[currGroupIndex]
        },
        async: true,
        cache: false,
        success: function (msg) {
            workLogData = msg;
            initWorkLogChart(msg.Categories_names, msg.Series)

            $('.spinner').hide();

            if (toastObj) {
                M.toast(toastObj);
                toastObj = null;
            }
        },
        error: function (err) {

        }
    });
}

function initWorkLogChart(categories, series) {
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
            max: currGroupIndex == 0 ? 200 : currGroupIndex == 1 ? 40 : 8,
            minorTickInterval: currGroupIndex == 0 ? 25 : currGroupIndex == 1 ? 5 : 1,
            title: {
                text: 'Broj sati'
            }
        }],
        legend: {
            shadow: false
        },
        tooltip: {
            shared: true
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

function addLunch() {
    $('.spinner').show();
    $.ajax({
        url: appName + '/Home/AddLunchToToday',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        cache: false,
        success: function (msg) {
            if (msg.length > maxDetailsLng) {
                toastObj = {
                    html: '<span style="color: black; text-align: right">Ručak je unesen <b>' + msg.length + '</b> puta</span>',
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
                    html: '<span style="color: black; text-align: right">Ručak je unesen <b>' + msg.length + '</b> puta za sljedeće datume:' + str + '</span>',
                    classes: toastClasses,
                    displayLength: toastLong
                };
            }
            if (msg.length > 1)
                getWorkLogs();
            else {
                $('.spinner').hide();
                toastObj = {
                    html: '<span style="color: black; text-align: right">Za sve dane je već unesen ručak</span>',
                    classes: toastClasses,
                    displayLength: toastShort
                };
                M.toast(toastObj);
            }
        }
    });
}

function addHolidays() {
    $('.spinner').show();
    $.ajax({
        url: appName + '/Home/AddHolidays',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        cache: false,
        success: function (msg) {
            if (msg.length > maxDetailsLng) {
                toastObj = {
                    html: '<span style="color: black; text-align: right">Praznik je unesen <b>' + msg.length + '</b> puta</span>',
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
                    html: '<span style="color: black; text-align: right">Praznik je unesen <b>' + msg.length + '</b> puta za sljedeće datume:' + str + '</span>',
                    classes: toastClasses,
                    displayLength: toastLong
                };
            }
            else {
            }
            if (msg.length > 1)
                getWorkLogs();
            else {
                $('.spinner').hide();
                toastObj = {
                    html: '<span style="color: black; text-align: right">Nije pronađen ni jedan praznik za unijeti</span>',
                    classes: toastClasses,
                    displayLength: toastShort
                };
                M.toast(toastObj);
            }
        }
    });
}

function addVacations() {
    $('.spinner').show();
    $.ajax({
        url: appName + '/Home/AddVacations',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        cache: false,
        success: function (msg) {
            if (msg.length > maxDetailsLng) {
                toastObj = {
                    html: '<span style="color: black; text-align: right">GO je unesen <b>' + msg.length + '</b> puta</span>',
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
                    html: '<span style="color: black; text-align: right">GO je unesen <b>' + msg.length + '</b> puta za sljedeće datume:' + str + '</span>',
                    classes: toastClasses,
                    displayLength: toastLong
                };
            }
            else {
            }
            if (msg.length > 1)
                getWorkLogs();
            else {
                $('.spinner').hide();
                toastObj = {
                    html: '<span style="color: black; text-align: right">Nije pronađen novi GO za unijeti</span>',
                    classes: toastClasses,
                    displayLength: toastShort
                };
                M.toast(toastObj);
            }
        }
    });
}

function addPaidLeaves() {
    $('.spinner').show();
    $.ajax({
        url: appName + '/Home/AddPaidLeaves',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        cache: false,
        success: function (msg) {
            if (msg.length > maxDetailsLng) {
                toastObj = {
                    html: '<span style="color: black; text-align: right">Plaćeni dopust je unesen <b>' + msg.length + '</b> puta</span>',
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
                    html: '<span style="color: black; text-align: right">Plaćeni dopust je unesen <b>' + msg.length + '</b> puta za sljedeće datume:' + str + '</span>',
                    classes: toastClasses,
                    displayLength: toastLong
                };
            }
            else {
            }
            if (msg.length > 1)
                getWorkLogs();
            else {
                $('.spinner').hide();
                toastObj = {
                    html: '<span style="color: black; text-align: right">Nije pronađen novi plaćeni dopust za unijeti</span>',
                    classes: toastClasses,
                    displayLength: toastShort
                };
                M.toast(toastObj);
            }
        }
    });
}


function preselectRange(range) {
    var dateFromPicker = M.Datepicker.getInstance($('#dateFromPicker')[0]);
    var dateToPicker = M.Datepicker.getInstance($('#dateToPicker')[0]);

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
    $('#dateFromPicker').val(dateFromPicker.toString());
    $('#dateToPicker').val(dateToPicker.toString());
}

function toDate(mom) {
    return new Date(mom._d.getTime());
}