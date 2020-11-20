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

function fillWorkAmount(dayLogs, viewModel, dayLog) {
    var totalHours = 0;
    for (var i = 0; i < dayLogs.workLogs.length; i++) {
        var wl = dayLogs.workLogs[i];
        totalHours += wl.Amount()
    }
    var overflow = totalHours - 8;
    dayLog.Amount(dayLog.Amount() - overflow);    
    updateWorkLog.call(dayLog);
}