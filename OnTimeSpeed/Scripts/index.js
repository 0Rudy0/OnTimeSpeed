var workLogData;
var groupTypes = [null, 2, 3];
var currGroupIndex = 0;
var breadCrumbs = [];

$(function () {
    if (loggedIn) {
        getWorkLogs();
    }
})

function getWorkLogs(forDate, groupIndex) {
    $('#workLogChart').html('');
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
                    categories: msg.Categories_names
                },
                credits: {
                    enabled: false
                },
                yAxis: [{
                    min: 0,
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
                    data: msg.Series[0].ValuesArray,
                    pointPadding: 0.3,
                    dataLabels: {
                        enabled: false
                    },
                }, {
                    name: 'Ostvareno',
                    color: 'rgba(102, 187, 106, 0.9)',
                    data: msg.Series[1].ValuesArray,
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
                }],
                //drilldown: {
                //    series: [{
                //        name: 'Microsoft Internet Explorer',
                //        id: 'Microsoft Internet Explorer',
                //        data: [{
                //            name: 'M1',
                //            y: 22,
                //            drilldown: 'M1'
                //        }]
                //    }, {
                //        name: 'Chrome',
                //        id: 'Chrome',
                //        data: [
                //            [
                //                'v40.0',
                //                5]
                //        ]
                //    }, {
                //        id: 'M1',
                //        data: [
                //            [
                //                'v8.0',
                //                17.2],
                //            [
                //                '1.0',
                //                25.2]
                //        ]
                //    }]
                //}
            });
        },
        error: function (err) {

        }
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