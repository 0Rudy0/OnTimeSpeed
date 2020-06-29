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
var closeBtnHtml = '<button class="btn-flat toast-action" onclick="closeNotification(this)">X</button>';
var firstLoad = true;
var lastAjaxRequest;
var pingStack = [];

var dateFromPickers = {
    semiAutomaticDateFrom: null
}

var easeOutBounce = function (pos) {
    if ((pos) < (1 / 2.75)) {
        return (7.5625 * pos * pos);
    }
    if (pos < (2 / 2.75)) {
        return (7.5625 * (pos -= (1.5 / 2.75)) * pos + 0.75);
    }
    if (pos < (2.5 / 2.75)) {
        return (7.5625 * (pos -= (2.25 / 2.75)) * pos + 0.9375);
    }
    return (7.5625 * (pos -= (2.625 / 2.75)) * pos + 0.984375);
};

Math.easeOutBounce = easeOutBounce;

$(function () {
    var elems = document.querySelectorAll('.collapsible');
    var instances = M.Collapsible.init(elems, {
    });
   
    if (loggedIn) {
        getHolidays();
        getWorkLogs();
        initWorkLogChart([], []);        
    }
    else {
        $('.spinner.main').hide();
    }
    $('.container').show();
    //$('#addLunchBtn').click(addLunch);
    //$('#addHolidaysButton').click(addHolidays);
    //$('#addVacationButton').click(addVacations);
    //$('#addPaidLeaveButton').click(addPaidLeaves);
    //$('#addAllAutomatic').click(addAllAutomatic);


    viewModel = new ViewModel();
    ko.applyBindings(viewModel);
    //ko.applyBindings(viewModel, $('.modal'));

    loadSettings();
    var openedIndex = viewModel.userSettings().openedAccordion;
    if (loggedIn && openedIndex != null) {
        var instance = M.Collapsible.getInstance($('.collapsible')[0]);
        instance.open(openedIndex);
    }

     if (loggedIn) {
        if (!viewModel.hrproUser()) {
            var tempToast = {
                html: '<span style="color: black">Neuspješna prijava u HrNet<br>Neke funckionalnosti zbog toga nisu dostupne</span>' + closeBtnHtml,
                classes: toastClasses,
                displayLength: toastLong
            };
            M.toast(tempToast);
        }
    }   

    $('.container').removeClass('hide');
    $('.connectBtn').removeClass('hide');
    $('.modals').removeClass('hide');

    window.addEventListener("focus", function(event) { 
        var now = new Date();
        if (lastAjaxRequest == null)
            lastAjaxRequest = new Date();

        var secondsPassed = (now.getTime() - lastAjaxRequest.getTime()) / 1000;
        if (secondsPassed > inactivityIntervalSeconds) {
            //console.log("prošlo " + secondsPassed + " minuta");
            ping();
        }
        //console.log(("focused: " + new Date()).toLocaleString());
    }, false);
 
})

function ping() {
    pingStack.push(0);
    setTimeout(function() {
        if (pingStack.length > 0) {
            $('.spinner.main').show(); 
        }
    }, 200);
    ajaxGET({
        url: '/Home/Ping'
    }, function (msg) {
        pingStack = [];
        $('.spinner.main').hide(); 
    });
}


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

function deleteWorkLogs() {
    if ($('#confirmDeleteInput').val() === 'BRISANJE') {
        M.Modal.getInstance($('#confirmDeleteModal')[0]).close();
        $('.spinner.main').show();
        ajaxPOST({
            url: '/Home/DeleteWorkLogs',
            data: {
                forDate: breadCrumbs[breadCrumbs.length - 1],
                groupType: groupTypes[currGroupIndex]
            }
        }, function (msg) {
            //$('.spinner.main').hide();
            var tempToastObj = {
                html: '<span style="color: black; text-align: right">Obrisano logova: <b>' + msg.logsCount + '</b> (Obrisano sati: <b>' + msg.workAmount + '</b>)</span>' + closeBtnHtml,
                classes: toastClasses,
                displayLength: toastLong
            };
            M.toast(tempToastObj);        
            getWorkLogs();
        });
    }
    else {
        M.toast({
            html: '<span style="color: black; text-align: right">Niste upisali "BRISANJE" u input</span>' + closeBtnHtml,
            classes: toastClasses,
            displayLength: 2000
        });    
    }
}

function deleteWorkLogsValidate() {
    $('.spinner.main').show();
    ajaxGET({
        url: '/Home/DeleteWorkLogsValidate',
        data: {
            forDate: breadCrumbs[breadCrumbs.length - 1],
            groupType: groupTypes[currGroupIndex]
        }
    }, function (msg) {
        $('.spinner.main').hide();
        //console.log(msg);
        viewModel.confirmDeleteModel(msg);
        M.Modal.getInstance($('#confirmDeleteModal')[0]).open();
    });
}

function getHolidays() {
    ajaxGET({
        url: '/Home/GetHolidays',
        /*data: {
            forDate: forDate,
            groupType: groupTypes[currGroupIndex]
        }*/
    }, function (msg) {
    });
}

function getWorkLogsAction(forDate) {
    ajaxGET({
        url: '/Home/GetWorkLogs',
        data: {
            forDate: forDate,
            groupType: groupTypes[currGroupIndex]
        }
    }, function (msg) {
        workLogData = msg;
        initWorkLogChart(msg.Categories_names, msg.Series)

        $('.spinner.main').hide();

        if (toastObj) {
            M.toast(toastObj);
            toastObj = null;
        }

        if (firstLoad) {
            firstLoad = false;

            M.Datepicker.init(document.querySelectorAll('#semiAutomaticEntry .datepicker.dateFrom'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                defaultDate: new Date(),
                setDefaultDate: true,
                onSelect: onDateFromSelect.bind(viewModel.semiAutomaticEntry())
            });
            M.Datepicker.init(document.querySelectorAll('#semiAutomaticEntry .datepicker.dateTo'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                onSelect: onDateToSelect.bind(viewModel.semiAutomaticEntry())
            });

            M.Datepicker.init(document.querySelectorAll('#customEntry .datepicker.dateFrom'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                defaultDate: new Date(),
                setDefaultDate: true,
                onSelect: onDateFromSelect.bind(viewModel.semiAutomaticEntry())
            });
            M.Datepicker.init(document.querySelectorAll('#customEntry .datepicker.dateTo'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                onSelect: onDateToSelect.bind(viewModel.customEntry())
            });

            M.Datepicker.init(document.querySelectorAll('#templates .datepicker.dateFrom'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                defaultDate: new Date(),
                setDefaultDate: true,
                onSelect: onDateFromSelect.bind(viewModel.templateModel())
            });
            M.Datepicker.init(document.querySelectorAll('#templates .datepicker.dateTo'), {
                autoClose: true,
                format: 'dd.mm.yyyy',
                firstDay: 1,
                showDaysInNextAndPreviousMonths: true,
                showClearBtn: true,
                onSelect: onDateToSelect.bind(viewModel.customEntry())
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
            var authPerformed = sessionStorage.getItem("reauthPreformed");
            if (authPerformed) {
                sessionStorage.removeItem("reauthPreformed");
                /*var tempToast = {
                    html: '<span style="color: black">Ponovno ste prijavljeni u aplikaciju.<br>Ponovite zadnju akciju</span>' + closeBtnHtml,
                    classes: toastClasses,
                    displayLength: toastLong
                };
                M.toast(tempToast);*/
            }
        }

        //console.log(instances);
    });
}
function initWorkLogChart(categories, series) {
    var tooltipFormatter = function () {
        var txt = '<h4 class="workLogsTableTitle">' + this.x + '</h4><table class="totalsWorkLogTable striped blue-grey lighten-2"><tr><th>Predmet</th><th>Work type</th><th>Opis</th><th>Broj sati</th></tr>';
        var total = 0;
        for (var prop in workLogData.WorkLogs) {
            if (Object.prototype.hasOwnProperty.call(workLogData.WorkLogs, prop)) {
                if (prop == this.x) {
                    for (var i = 0; i < workLogData.WorkLogs[prop].length; i++) {
                        var log = workLogData.WorkLogs[prop][i];
                        log.Description = log.Description == null ? '' : log.Description;
                        txt += '<tr><td>' + log.ItemName + '</td><td>' + log.WorkType + '</td><td>' + log.Descripton + '</td><td>' + log.Amount + ' hrs' + '</td></tr>';
                        total += log.Amount;
                    }
                }
            }
        }
        if (total < 8 || true) {
            txt += '<tr class="totalWorkLog"><td>UKUPNO</td><td></td><td></td><td>' + total + ' hrs' + '</td></tr>';
        }
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

    var planiranoBellowColor = 'rgb(216, 233, 206)';
    var planiranoExactColor = 'rgb(216, 233, 206)';
    var planiranoOverColor = 'rgb(216, 233, 206)';

    var ostvarenoBellowColor = 'rgba(207, 84, 84, 0.9)';
    var ostvarenoExactColor = 'rgba(102, 187, 106, 0.9)';
    var ostvarenoOverColor = 'rgba(213, 145, 77, 0.8)';

    
    var planiranoSeriesData = [];
    if (series.length > 1) {
        for (var i = 0; i < series[0].ValuesArray.length; i++)  {
            planiranoSeriesData.push({
                x: i,
                y: series[0].ValuesArray[i],
                color: series[0].ValuesArray[i] > series[1].ValuesArray[i] ? planiranoBellowColor :
                series[0].ValuesArray[i] < series[1].ValuesArray[i] ? planiranoOverColor : planiranoExactColor
            })
        }
    }

    var ostvarenoSeriesData = [];
    if (series.length > 1) {
        for (var i = 0; i < series[1].ValuesArray.length; i++)  {
            ostvarenoSeriesData.push({
                x: i,
                y: series[1].ValuesArray[i],
                color:  series[0].ValuesArray[i] > series[1].ValuesArray[i] ? ostvarenoBellowColor:
                    series[0].ValuesArray[i] < series[1].ValuesArray[i] ? ostvarenoOverColor : ostvarenoExactColor
            })
        }
    }
    //console.log(ostvarenoSeriesData);


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
                            else {
                                var newDayModel = {
                                    dayText: this.category,
                                    workLogs: []
                                }
                                for (var prop in workLogData.WorkLogs) {
                                    if (Object.prototype.hasOwnProperty.call(workLogData.WorkLogs, prop)) {
                                        if (prop == this.category) {
                                            for (var i = 0; i < workLogData.WorkLogs[prop].length; i++) {
                                                var log = workLogData.WorkLogs[prop][i];
                                                newDayModel.workLogs.push(log);
                                            }
                                        }
                                    }
                                }
                                //console.log(newDayModel);
                                viewModel.workDayModel(newDayModel);
                                M.Modal.getInstance($('#workLogsForDayModal')[0]).open();
                            }
                        }
                    }
                }
            }
        },
        series: [{
            name: 'Planirano',
            color: 'rgb(200, 230, 201)',
            data: planiranoSeriesData,
            pointPadding: 0.3,
            dataLabels: {
                enabled: false
            },
            animation: {
                duration: 500,
                 //Uses jQuery.easing['swing']
                easing: 'easeOutBounce2'
            }
        }, {
            name: 'Ostvareno',
            color: 'rgba(102, 187, 106, 0.9)',
            //data: series.length > 1 ? series[1].ValuesArray : [],
            data: ostvarenoSeriesData,
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
            },
            // animation: {
            //     duration: 1000,
            //     // Uses Math.easeOutBounce
            //     easing: 'easeOutBounce'
            // }
        }]
    });
}




function onAccordionOpen(index) {
    //console.log(this);
    //console.log(index);
    viewModel.userSettings().openedAccordion = index;
    saveSettings();
    //$(this).scrollTop($(this).height());

    setTimeout(function () {
        if ($(window).scrollTop() < $($(".collapsible li")[index]).offset().top + 50) {
            $([document.documentElement, document.body]).animate({
                scrollTop: $($(".collapsible li")[index]).offset().top + 50
            }, 1000);
        }
    }, 0);
    //M.Collapsible.getInstance($('.collapsible')[0]).open(2);
}

function closeNotification(element) {
    $(element).parent().hide();
    //console.log(event.targetElement);
}

function openAutomaticEntryModalInfo(a, event) {
    event.stopPropagation();
    M.Modal.getInstance($('#automaticEntryModalInfo')[0]).open();
}

function openSemiAutomaticEntryModalInfo(a, event) {
    event.stopPropagation();
    M.Modal.getInstance($('#semiAutomaticEntryModalInfo')[0]).open();
}

function openCustomEntryModalInfo(a, event) {
    event.stopPropagation();
    M.Modal.getInstance($('#customEntryModalInfo')[0]).open();
}

function openTemplateEntryModalInfo(a, event) {
    event.stopPropagation();
    M.Modal.getInstance($('#templateEntryModalInfo')[0]).open();
}