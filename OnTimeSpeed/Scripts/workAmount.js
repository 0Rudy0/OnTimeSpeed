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