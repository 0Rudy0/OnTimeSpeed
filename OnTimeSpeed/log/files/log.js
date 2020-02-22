
$(function () {
    //$('title').html("Web ServerLog");
	var header = '<div class="header">' +
		'Filter:' +
		'<span class="filterItem active" data-type="all" id="filterAll">All <span class="count"></span></span>' +
		'<span class="filterItem" data-type="exception" id="filterException">Exception <span class="count"></span></span>' +
		'<span class="filterItem" data-type="cache" id="filterCache">Cache <span class="count"></span></span>' +
		'</div>' +
		'<div class="card panel-default">' +
		'<div class="card-body">';

	var logType = "server";
	if ($('body').html().indexOf('- Ajax call to') > -1) {
		logType = "client";
	}
	var badgeHolder = '<div id="badgeHolder">' + 
        '<button type="button" id="exceptionCount" class="btn btn-danger selected">Exception <span class="badge badge-light">0</span></button>' +
	    '<button type="button" id="debugCount" class="btn btn-primary selected">Debug <span class="badge badge-light">0</span></button>' +
	    '<button type="button" id="dataCount" class="btn btn-success selected">Data <span class="badge badge-light">0</span></button></div>';
	var navbar = '<nav class="navbar ' + logType + ' navbar-default">' + badgeHolder + '<div id="dateHolder"><div id="afterDateHolder">After date<input id="afterDatePicker"/></div><div id="onDateHolder">On date<input id="onDatePicker"/></div></div><input id="usernameSearch" placeholder="Select token..."/><button type="button" id="btnReset" class="btn btn-light">Clear options</button></nav>';

	$('body').remove('.header');
	if (($('.exception').length + $('.cache').length) == 0) {
		return;
	}
	$('body').prepend(navbar);	

	$("#usernameSearch").kendoComboBox({
		dataTextField: "text",
		dataValueField: "value",
		dataSource: getUsernames(),
		filter: "contains",
		suggest: true,
		index: 3,
		change: function () {
			$('#exceptionBtn').addClass("btn-primary");
			$('#cacheBtn').removeClass("btn-primary");
			$('#allBtn').removeClass("btn-primary");

			refreshPanelsVisibility();
			refreshBadgeCount();
		}
	});
	$("#usernameSearch").data("kendoComboBox").value(null)

	//$('#filterAll .count').text($('.exception').length + $('.cache').length);
	//$('#filterException .count').text($('.exception').length);
	//$('#filterCache .count').text($('.cache').length);

	$('#exceptionBtn span.badge').html($('div.exception.card').length);
	$('#cacheBtn span.badge').html($('div.cache.card').length);
	$('#allBtn span.badge').html($('div.exception.card').length + $('div.cache.card').length);

	$('#exceptionCount').click(function () {
	    if ($(this).hasClass("selected")) {
	        $(this).removeClass("selected");
	    }
	    else {
	        $(this).addClass("selected");
	    }
	    refreshPanelsVisibility();
	})
	$('#debugCount').click(function () {
	    if ($(this).hasClass("selected")) {
	        $(this).removeClass("selected");
	    }
	    else {
	        $(this).addClass("selected");
	    }
	    refreshPanelsVisibility();
	})
	$('#dataCount').click(function () {
	    if ($(this).hasClass("selected")) {
	        $(this).removeClass("selected");
	    }
	    else {
	        $(this).addClass("selected");
	    }
	    refreshPanelsVisibility();
	})

	$("#afterDatePicker").kendoDatePicker({
		max: new Date(),
		format: "dd.MM.yyyy",
		value: new Date(moment().subtract(2, 'days').format()),
		change: function () {
			$("#onDatePicker").data("kendoDatePicker").value(null);
			refreshPanelsVisibility();
			refreshBadgeCount();
		}
	});
	$("#onDatePicker").kendoDatePicker({
		max: new Date(),
		format: "dd.MM.yyyy",
		value: null,
		change: function () {
			$("#afterDatePicker").data("kendoDatePicker").value(null);
			refreshPanelsVisibility();
			refreshBadgeCount();
		}
	});

	$('.filterItem').click(function () {
		refreshPanelsVisibility();
	});

	$('#btnReset').click(function () {
		$('#exceptionCount').addClass("selected");
		$('#debugCount').addClass("selected");
		$('#dataCount').addClass("selected");
		$("#onDatePicker").data("kendoDatePicker").value(null);
		$("#afterDatePicker").data("kendoDatePicker").value(null);
		$("#usernameSearch").data("kendoComboBox").value(null);
		refreshPanelsVisibility();
		refreshBadgeCount();
	})

	refreshPanelsVisibility();
	refreshBadgeCount();
	
    $("html, body").animate({ scrollTop: $(document).height() }, 500);
    
    $('#debugCount').click();
    $('#dataCount').click();
});

function refreshPanelsVisibility() {
	var afterDate = $("#afterDatePicker").data("kendoDatePicker").value();
	var onDate = $("#onDatePicker").data("kendoDatePicker").value();
	var panels = $('div.card');
	var userName = $("#usernameSearch").data("kendoComboBox").value();

	for (var i = 0; i < panels.length; i++) {
		var currPanel = $(panels[i]);
		var panelDateFull = new Date(currPanel.find('span.timeUtc').html())
		var panelDate = new Date(panelDateFull.getFullYear(), panelDateFull.getMonth(), panelDateFull.getDate(), 0, 0, 0, 0);
		currPanel.show();

		if (userName != "" &&
			userName != undefined &&
			userName != null &&
			currPanel.find('.loggedUser').html() &&
			currPanel.find('.loggedUser').html().replace('</strong>', '').indexOf(userName) < 0) {
			currPanel.hide();
		}

		if (currPanel.find("h3.card-title").hasClass("exception") && 
			($('#exceptionCount').hasClass('selected') == false ||
			(afterDate != null && afterDate.getTime() > panelDate.getTime()) ||
			(onDate != null && onDate.getTime() != panelDate.getTime()))) {
			currPanel.hide();
		}
		else if (currPanel.find("h3.card-title").hasClass("debug") && 
			($('#debugCount').hasClass('selected') == false ||
			(afterDate != null && afterDate.getTime() > panelDate.getTime()) ||
			(onDate != null && onDate.getTime() != panelDate.getTime()))) {
			currPanel.hide();
		}
		else if (currPanel.find("h3.card-title").hasClass("data") &&
			($('#dataCount').hasClass('selected') == false ||
			(afterDate != null && afterDate.getTime() > panelDate.getTime()) ||
			(onDate != null && onDate.getTime() != panelDate.getTime()))) {
		    currPanel.hide();
		}
	}
}

function getUsernames() {
	var panels = $('div.card');
	var users = [];
	for (var i = 0; i < panels.length; i++) {
		var currPanel = $(panels[i]);
		var user = currPanel.find('.loggedUser').html();
		if (users.indexOf(user) < 0 && user != undefined) {
			users.push(user);
		}
	}

	var userObjects = [];
	for (var i = 0; i < users.length ; i++) {
		users[i] = users[i].replace('Used token: <strong>', '');
		users[i] = users[i].replace('</strong>', '');
		//users[i] = users[i].replace('(', ': ');
		//users[i] = users[i].replace(')', '');
		userObjects.push({
			text: users[i],
			value: users[i]
		})
	}
	//console.log(users.length);
	return userObjects;
}

function refreshBadgeCount() {
    var panels = $('div.card');
	var countExs = 0;
	var countDbg = 0;
	var countDat = 0;
	for (var i = 0; i < panels.length; i++) {
		var currPanel = $(panels[i]);
		if (currPanel.find("h3.card-title").hasClass("exception") && currPanel.is(":visible")) {
			countExs++;
		}
		else if (currPanel.find("h3.card-title").hasClass("debug") && currPanel.is(":visible")) {
			countDbg++;
		}
		else if (currPanel.find("h3.card-title").hasClass("data") && currPanel.is(":visible")) {
		    countDat++;
		}
	}

	$('#exceptionCount span.badge').html(countExs);
	$('#debugCount span.badge').html(countDbg);
	$('#dataCount span.badge').html(countDat);
}