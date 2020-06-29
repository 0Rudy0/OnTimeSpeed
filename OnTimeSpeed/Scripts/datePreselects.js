

function onDateFromSelect() {
    //var dateFromPicker = M.Datepicker.getInstance($(selectorDateFrom)[0]);
    //var dateToPicker = M.Datepicker.getInstance($(selectorDateTo)[0]);

    //dateFromPicker.setDate(dateFrom);
    //dateToPicker.setDate(dateTo);
}

function onDateToSelect() {

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
            d = moment(new Date(d.getFullYear(), d.getMonth(), 1));
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
            d = moment(new Date(d.getFullYear(), d.getMonth(), 1));
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