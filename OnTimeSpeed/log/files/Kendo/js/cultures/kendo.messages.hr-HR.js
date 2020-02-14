/*
* Kendo UI v2015.2.624 (http://www.telerik.com/kendo-ui)
* Copyright 2015 Telerik AD. All rights reserved.
*
* Kendo UI commercial licenses may be obtained at
* http://www.telerik.com/purchase/license-agreement/kendo-ui-complete
* If you do not own a commercial license, this file shall be governed by the trial license terms.
*/
(function(f, define){
	define([], f);
})(function(){

	(function ($, undefined) {
		/* FlatColorPicker messages */

		if (kendo.ui.FlatColorPicker) {
			kendo.ui.FlatColorPicker.prototype.options.messages =
			$.extend(true, kendo.ui.FlatColorPicker.prototype.options.messages,{
				"apply": "Primijeni",
				"cancel": "Odustani"
			});
		}

		/* ColorPicker messages */

		if (kendo.ui.ColorPicker) {
			kendo.ui.ColorPicker.prototype.options.messages =
			$.extend(true, kendo.ui.ColorPicker.prototype.options.messages,{
				"apply": "Primijeni",
				"cancel": "Odustani"
			});
		}

		/* ColumnMenu messages */

		if (kendo.ui.ColumnMenu) {
			kendo.ui.ColumnMenu.prototype.options.messages =
			$.extend(true, kendo.ui.ColumnMenu.prototype.options.messages,{
				"sortAscending": "Poredaj Uzlazno",
				"sortDescending": "Poredaj Silazno",
				"filter": "Filter",
				"columns": "Stupci",
				"done": "Gotovo",
				"settings": "Postavke Stupaca",
				"lock": "Zaključaj",
				"unlock": "Otključaj"
			});
		}

		/* Editor messages */

		if (kendo.ui.Editor) {
			kendo.ui.Editor.prototype.options.messages =
			$.extend(true, kendo.ui.Editor.prototype.options.messages,{
				"bold": "Podebljano",
				"italic": "Nakošeno",
				"underline": "Potcrtano",
				"strikethrough": "Precrtano",
				"superscript": "Natpis",
				"subscript": "Potpis",
				"justifyCenter": "Centriraj tekst",
				"justifyLeft": "Poravnaj tekst lijevo",
				"justifyRight": "Poravnaj tekst desno",
				"justifyFull": "Poravnaj",
				"insertUnorderedList": "Umetni neuređenu listu",
				"insertOrderedList": "Umetni uređenu listu",
				"indent": "Uvuci",
				"outdent": "Izvuci",
				"createLink": "Umetni poveznicu",
				"unlink": "Ukloni poveznicu",
				"insertImage": "Umetni sliku",
				"insertFile": "Umetni datoteku",
				"insertHtml": "Umetni HTML",
				"viewHtml": "Pogledaj HTML",
				"fontName": "Odaberi font",
				"fontNameInherit": "(nasljeđeni font)",
				"fontSize": "Odaberi veličinu fonta",
				"fontSizeInherit": "(nasljeđena veličina)",
				"formatBlock": "Formatiraj",
				"formatting": "Format",
				"foreColor": "Boja",
				"backColor": "Pozadinska boja",
				"style": "Stilovi",
				"emptyFolder": "Prazna Mapa",
				"uploadFile": "Učitaj",
				"orderBy": "Poredaj po:",
				"orderBySize": "Veličina",
				"orderByName": "Naziv",
				"invalidFileType": "Odabrana datoteka \"{0}\" je nevažeća. Podržani tipovi datoteka su {1}.",
				"deleteFile": 'Jeste li sigurni da želite obrisati "{0}"?',
				"overwriteFile": 'Datoteka s imenom "{0}" već postoji u trenutnoj mapi. Želite li ju prebrisati?',
				"directoryNotFound": "Mapa s ovim imenom nije pronađena.",
				"imageWebAddress": "Web adresa",
				"imageAltText": "Alternativni tekst",
				"imageWidth": "Širina (px)",
				"imageHeight": "Visina (px)",
				"fileWebAddress": "Web adresa",
				"fileTitle": "Naslov",
				"linkWebAddress": "Web adresa",
				"linkText": "Tekst",
				"linkToolTip": "Naputak",
				"linkOpenInNewWindow": "Otvori poveznicu u novom ekranu",
				"dialogUpdate": "Osvježi",
				"dialogInsert": "Umetni",
				"dialogButtonSeparator": "ili",
				"dialogCancel": "Odustani",
				"createTable": "Kreiraj tablicu",
				"addColumnLeft": "Dpdaj stupac lijevo",
				"addColumnRight": "Dodaj stupac desno",
				"addRowAbove": "Dodaj redak iznad",
				"addRowBelow": "Dodaj redak ispod",
				"deleteRow": "Obriši redak",
				"deleteColumn": "Obriši stupac"
			});
		}

		/* FileBrowser messages */

		if (kendo.ui.FileBrowser) {
			kendo.ui.FileBrowser.prototype.options.messages =
			$.extend(true, kendo.ui.FileBrowser.prototype.options.messages,{
				"uploadFile": "Učitaj",
				"orderBy": "Poredaj po",
				"orderByName": "Naziv",
				"orderBySize": "Veličina",
				"directoryNotFound": "Mapa s ovim imenom nije pronađena.",
				"emptyFolder": "Prazna Mapa",
				"deleteFile": 'Jeste li sigurni da želite obrisati "{0}"?',
				"invalidFileType": "Odabrana datoteka \"{0}\" nije važeća. Dozvoljeni tipovi datoteka su {1}.",
				"overwriteFile": 'Datoteka s imenom "{0}" već postoji u trenutnoj mapi. Želite li ju prebrisati?',
				"dropFilesHere": "ispusti datoteku za učitavanje",
				"search": "Traži"
			});
		}

		/* FilterCell messages */

		if (kendo.ui.FilterCell) {
			kendo.ui.FilterCell.prototype.options.messages =
			$.extend(true, kendo.ui.FilterCell.prototype.options.messages,{
				"isTrue": "je istina",
				"isFalse": "je neistina",
				"filter": "Filtriraj",
				"clear": "Očisti",
				"operator": "Operator"
			});
		}

		/* FilterCell operators */

		if (kendo.ui.FilterCell) {
			kendo.ui.FilterCell.prototype.options.operators =
			$.extend(true, kendo.ui.FilterCell.prototype.options.operators,{
				"string": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"startswith": "Počinje s",
					"contains": "Sadrži",
					"doesnotcontain": "Ne sadrži",
					"endswith": "Završava s"
				},
				"number": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"gte": "Je veće ili jednako od",
					"gt": "Je veće od",
					"lte": "Je manje ili jednako od",
					"lt": "Je manje od"
				},
				"date": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"gte": "Je nakon ili jednako",
					"gt": "Je nakon",
					"lte": "Je prije ili jednako",
					"lt": "je prije"
				},
				"enums": {
					"eq": "Je jednako",
					"neq": "Nije jednako"
				}
			});
		}

		/* FilterMenu messages */

		if (kendo.ui.FilterMenu) {
			kendo.ui.FilterMenu.prototype.options.messages =
			$.extend(true, kendo.ui.FilterMenu.prototype.options.messages,{
				"info": "Prikaži elemente s vrijednošću koja:",
				"isTrue": "je istina",
				"isFalse": "je neistina",
				"filter": "Filtriraj",
				"clear": "Očisti",
				"and": "I",
				"or": "Ili",
				"selectValue": "-Odaberi vrijednost-",
				"operator": "Operator",
				"value": "Vrijednost",
				"cancel": "Odustani"
			});
		}

		/* FilterMenu operator messages */

		if (kendo.ui.FilterMenu) {
			kendo.ui.FilterMenu.prototype.options.operators =
			$.extend(true, kendo.ui.FilterMenu.prototype.options.operators,{
				"string": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"startswith": "Počinje s",
					"contains": "Sadrži",
					"doesnotcontain": "Ne sadrži",
					"endswith": "Završava s"
				},
				"number": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"gte": "Je veće ili jednako od",
					"gt": "Je veće od",
					"lte": "Je manje ili jednako od",
					"lt": "Je manje od"
				},
				"date": {
					"eq": "Je jednako",
					"neq": "Nije jednako",
					"gte": "Je nakon ili jednako",
					"gt": "Je nakon",
					"lte": "Je prije ili jednako",
					"lt": "je prije"
				},
				"enums": {
					"eq": "Je jednako",
					"neq": "Nije jednako"
				}
			});
		}

		/* FilterMultiCheck messages */

		if (kendo.ui.FilterMultiCheck) {
			kendo.ui.FilterMultiCheck.prototype.options.messages =
			$.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages,{
				"checkAll": "Označi Sve",
				"clear": "Očisti",
				"filter": "Filtriraj"
			});
		}

		/* Gantt messages */

		if (kendo.ui.Gantt) {
			kendo.ui.Gantt.prototype.options.messages =
			$.extend(true, kendo.ui.Gantt.prototype.options.messages,{
				"actions": {
					"addChild": "Dodaj Dijete",
					"append": "Dodaj Zadatak",
					"insertAfter": "Dodaj Ispod",
					"insertBefore": "Dodaj Iznad",
					"pdf": "Izvezi u PDF"
				},
				"cancel": "Odustani",
				"deleteDependencyWindowTitle": "Obriši zavisnost",
				"deleteTaskWindowTitle": "Obriši zadatak",
				"destroy": "Obriši",
				"editor": {
					"assingButton": "Dodijeli",
					"editorTitle": "Zadatak",
					"end": "Kraj",
					"percentComplete": "Završeno",
					"resources": "Resursi",
					"resourcesEditorTitle": "Resursi",
					"resourcesHeader": "Resursi",
					"start": "Početak",
					"title": "Naslov",
					"unitsHeader": "Jedinice"
				},
				"save": "Spremi",
				"views": {
					"day": "Dan",
					"end": "Kraj",
					"month": "Mjesec",
					"start": "Početak",
					"week": "Tjedan",
					"year": "Godina"
				}
			});
		}

		/* Grid messages */

		if (kendo.ui.Grid) {
			kendo.ui.Grid.prototype.options.messages =
			$.extend(true, kendo.ui.Grid.prototype.options.messages,{
				"commands": {
					"cancel": "Otkaži promjene",
					"canceledit": "Odustani",
					"create": "Dodaj novi zapis",
					"destroy": "Obriši",
					"edit": "Izmijeni",
					"excel": "Izvezi u Excel",
					"pdf": "Izvezi u PDF",
					"save": "Spremi promjene",
					"select": "Odaberi",
					"update": "Osvježi"
				},
				"editable": {
					"cancelDelete": "Odustani",
					"confirmation": "Jeste li sigurni da želite obrisati ovaj zapis?",
					"confirmDelete": "Obriši"
				},
				"noRecords": "Nema zapisa."
			});
		}

		/* Groupable messages */

		if (kendo.ui.Groupable) {
			kendo.ui.Groupable.prototype.options.messages =
			$.extend(true, kendo.ui.Groupable.prototype.options.messages,{
				"empty": "Dovuci zaglavlje kolone kako bi grupirao po toj koloni"
			});
		}

		/* NumericTextBox messages */

		if (kendo.ui.NumericTextBox) {
			kendo.ui.NumericTextBox.prototype.options =
			$.extend(true, kendo.ui.NumericTextBox.prototype.options,{
				"upArrowText": "Povećaj vrijednost",
				"downArrowText": "Smanji vrijednost"
			});
		}

		/* Pager messages */

		if (kendo.ui.Pager) {
			kendo.ui.Pager.prototype.options.messages =
			$.extend(true, kendo.ui.Pager.prototype.options.messages,{
				"display": "{0} - {1} od {2} elemenata",
				"empty": "Nema podataka za prikaz",
				"page": "Stranica",
				"of": "od {0}",
				"itemsPerPage": "elemenata po stranici",
				"first": "Idi na prvu stranicu",
				"previous": "Idi na prethodnu stranicu",
				"next": "idi na sljedeću stranicu",
				"last": "Idi na zadnju stranicu",
				"refresh": "Osvježi",
				"morePages": "Više stranica"
			});
		}

		/* PivotGrid messages */

		if (kendo.ui.PivotGrid) {
			kendo.ui.PivotGrid.prototype.options.messages =
			$.extend(true, kendo.ui.PivotGrid.prototype.options.messages,{
				"measureFields": "Ispusti Podatkovna Polja",
				"columnFields": "Ispusti Stupce",
				"rowFields": "Ispusti Retke"
			});
		}

		/* PivotFieldMenu messages */

		if (kendo.ui.PivotFieldMenu) {
			kendo.ui.PivotFieldMenu.prototype.options.messages =
			$.extend(true, kendo.ui.PivotFieldMenu.prototype.options.messages,{
				"info": "Prikaži elemente sa vrijednošću koja:",
				"filterFields": "Filter polja",
				"filter": "Filtriraj",
				"include": "Uključi Polja...",
				"title": "Uključena Polja",
				"clear": "Očisti",
				"ok": "Ok",
				"cancel": "Odustani",
				"operators": {
					"contains": "Sadrži",
					"doesnotcontain": "Ne sadrži",
					"startswith": "Počinje s",
					"endswith": "završava s",
					"eq": "Je jednaka",
					"neq": "Nije jednaka"
				}
			});
		}

		/* RecurrenceEditor messages */

		if (kendo.ui.RecurrenceEditor) {
			kendo.ui.RecurrenceEditor.prototype.options.messages =
			$.extend(true, kendo.ui.RecurrenceEditor.prototype.options.messages,{
				"frequencies": {
					"never": "Nikad",
					"hourly": "Svakog sata",
					"daily": "Dnevno",
					"weekly": "Tjedno",
					"monthly": "Mjesečno",
					"yearly": "Godišnje"
				},
				"hourly": {
					"repeatEvery": "Ponovi svaki(h): ",
					"interval": " sat(i)"
				},
				"daily": {
					"repeatEvery": "Ponovi svaki(h): ",
					"interval": " dan(a)"
				},
				"weekly": {
					"interval": " tjedan(a)",
					"repeatEvery": "Ponovi svaki(a): ",
					"repeatOn": "Ponovi na: "
				},
				"monthly": {
					"repeatEvery": "Ponovi svaki(h): ",
					"repeatOn": "Ponovi na: ",
					"interval": " mjeseca(i)",
					"day": "Day "
				},
				"yearly": {
					"repeatEvery": "Ponovi svakih: ",
					"repeatOn": "Ponovi na: ",
					"interval": " godinu(a)",
					"of": " od "
				},
				"end": {
					"label": "Kraj:",
					"mobileLabel": "Završava",
					"never": "Nikad",
					"after": "Nakon ",
					"occurrence": " ponavljanje(a)",
					"on": "Na "
				},
				"offsetPositions": {
					"first": "prvo",
					"second": "drugo",
					"third": "treće",
					"fourth": "četvrto",
					"last": "zadnje"
				},
				"weekdays": {
					"day": "dan",
					"weekday": "radni dan",
					"weekend": "vikend"
				}
			});
		}

		/* Scheduler messages */

		if (kendo.ui.Scheduler) {
			kendo.ui.Scheduler.prototype.options.messages =
			$.extend(true, kendo.ui.Scheduler.prototype.options.messages,{
				"allDay": "cijeli dan",
				"date": "Datum",
				"event": "Događaj",
				"time": "Vrijeme",
				"showFullDay": "Prikaži puni dan",
				"showWorkDay": "Prikaži radne sate",
				"today": "Danas",
				"save": "Spremi",
				"cancel": "Odustani",
				"destroy": "Obriši",
				"deleteWindowTitle": "Obriši događaj",
				"ariaSlotLabel": "odabrano od {0:t} do {1:t}",
				"ariaEventLabel": "{0} na {1:D} u {2:t}",
				"confirmation": "Jeste li sigurni da želite obrisati ovaj događaj?",
				"views": {
					"day": "Dan",
					"week": "Tjedan",
					"workWeek": "Radni Tjedan",
					"agenda": "Raspored",
					"month": "Mjesec"
				},
				"recurrenceMessages": {
					"deleteWindowTitle": "Obriši Ponavljajući Element",
					"deleteWindowOccurrence": "Obriši trenutno ponavljanje",
					"deleteWindowSeries": "Obriši seriju",
					"editWindowTitle": "Izmijeni Ponavljajući Element",
					"editWindowOccurrence": "Izmijeni trenutno ponavljanje",
					"editWindowSeries": "Izmijeni seriju",
					"deleteRecurring": "Želite li obrisati samo trenutno ponavljanje ili cijelu seriju?",
					"editRecurring": "Želite li izmjeniti samo trenutno ponavljanje ili cijelu seriju?"
				},
				"editor": {
					"title": "Naslov",
					"start": "Početak",
					"end": "Kraj",
					"allDayEvent": "Cjelodnevni događaj",
					"description": "Opis",
					"repeat": "Ponavljanje",
					"timezone": " ",
					"startTimezone": "Početna vremenska zona",
					"endTimezone": "Završna vremenska zona",
					"separateTimezones": "Koristi zasebne početne i završne vremenske zone",
					"timezoneEditorTitle": "Vremenske zone",
					"timezoneEditorButton": "Vremenska zona",
					"timezoneTitle": "Vremenske zone",
					"noTimezone": "Bez vremenske zone",
					"editorTitle": "Događaj"
				}
			});
		}

		/* Slider messages */

		if (kendo.ui.Slider) {
			kendo.ui.Slider.prototype.options =
			$.extend(true, kendo.ui.Slider.prototype.options,{
				"increaseButtonTitle": "Povećaj",
				"decreaseButtonTitle": "Smanji"
			});
		}

		/* TreeList messages */

		if (kendo.ui.TreeList) {
			kendo.ui.TreeList.prototype.options.messages =
			$.extend(true, kendo.ui.TreeList.prototype.options.messages,{
				"noRows": "Nema zapisa za prikaz",
				"loading": "Učitavanje...",
				"requestFailed": "Neusjpešan zahtjev.",
				"retry": "Probaj ponovno",
				"commands": {
					"edit": "Uredi",
					"update": "Osvježi",
					"canceledit": "Otkaži",
					"create": "Dodaj novi zapis",
					"createchild": "Dodaj podzapis",
					"destroy": "Obriši",
					"excel": "Izvezi u Excel",
					"pdf": "Izvezi u PDF"
				}
			});
		}

		/* TreeView messages */

		if (kendo.ui.TreeView) {
			kendo.ui.TreeView.prototype.options.messages =
			$.extend(true, kendo.ui.TreeView.prototype.options.messages,{
				"loading": "Učitavanje...",
				"requestFailed": "Neuspješan zahtjev.",
				"retry": "Pokušaj ponovno"
			});
		}

		/* Upload messages */

		if (kendo.ui.Upload) {
			kendo.ui.Upload.prototype.options.localization=
			$.extend(true, kendo.ui.Upload.prototype.options.localization,{
				"select": "Odaberi datoteke...",
				"cancel": "Odustani",
				"retry": "Pokuaj ponovno",
				"remove": "Ukloni",
				"uploadSelectedFiles": "Učitaj datoteke",
				"dropFilesHere": "ispusti datoteke za učitavanje",
				"statusUploading": "učitavanje",
				"statusUploaded": "učitano",
				"statusWarning": "upozorenje",
				"statusFailed": "neuspješno",
				"headerStatusUploading": "Učitavanje...",
				"headerStatusUploaded": "Gotovo"
			});
		}

		/* Validator messages */

		if (kendo.ui.Validator) {
			kendo.ui.Validator.prototype.options.messages =
			$.extend(true, kendo.ui.Validator.prototype.options.messages,{
				"required": "{0} je obavezno",
				"pattern": "{0} nije važeće",
				"min": "{0} treba biti veće ili jednako {1}",
				"max": "{0} treba biti manje ili jednako {1}",
				"step": "{0} nije važeće",
				"email": "{0} nije važeća mail adresa",
				"url": "{0} nije važeći URL",
				"date": "{0} nije važeći datum",
			  "dateCompare": "Datum kraja treba biti veći ili jednak datumu početka"
		});
	}
	})(window.kendo.jQuery);


return window.kendo;

}, typeof define == 'function' && define.amd ? define : function(_, f){ f(); });