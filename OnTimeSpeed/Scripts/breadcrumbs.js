var breadCrumbs = [];

function addBreadCrumb(forDate) {
    breadCrumbs.push(forDate);
    refreshBreadCrumbs();
    viewModel.breadCrumbs(breadCrumbs);
}

function removeBreadCrumb(toString) {
    while (breadCrumbs.length > 0 && breadCrumbs[breadCrumbs.length - 1] != toString) {
        breadCrumbs.pop();
    }
    viewModel.breadCrumbs(breadCrumbs);
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
    if (viewModel)
        viewModel.breadCrumbs(breadCrumbs);
}
