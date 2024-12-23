$(function () {
  initCalendar();
});

function reloadCalendar() {
  calendar.refetchEvents();
}

function initCalendar() {
  var calendarEl = document.getElementById("calendar");
  calendar = new FullCalendar.Calendar(calendarEl, {
    customButtons: {
      addItem: {
        text: "Novo Evento",
        click: function () {
          var url = urlCreate + "?clickDate=add";
          $.popUpUrl(url, 600, 900, function () {
            reloadCalendar();
          });
        },
      },
    },
    plugins: ["bootstrap", "interaction", "dayGrid", "timeGrid", "list"],
    themeSystem: "spacelab",
    header: {
      left: "prev,next today addItem",
      center: "title",
      right: "dayGridMonth,timeGridWeek,timeGridDay,listMonth",
    },
    navLinks: true, // can click day/week names to navigate views
    editable: false,
    eventLimit: true, // allow "more" link when too many events
    locale: "pt-br",
    lazyFetching: true,
    nowIndicator: true,
    allDaySlot: false,
    events: function (info, successCallback, failureCallback) {
      var requestData = {
        start: info.startStr,
        end: info.endStr,
      };

      $.ajaxPost(urlGetEventos, requestData, function (responseData) {
        var evt = [];
        eventos = responseData.eventos;
        for (var x = 0; x < eventos.length; x++) {
          let dtIni = new Date(eventos[x].DataInicio);
          let hrIni = eventos[x].HoraInicio.split(":");
          if ((hrIni.length = 3)) dtIni.setHours(hrIni[0], hrIni[1], hrIni[2]);
          else if ((hrIni.length = 2)) dtIni.setHours(hrIni[0], hrIni[1]);
          else if ((hrIni.length = 1)) dtIni.setHours(hrIni[0]);

          let dtFim = new Date(eventos[x].DataFim);
          let hrFin = eventos[x].HoraFinal.split(":");
          if ((hrFin.length = 3)) dtFim.setHours(hrFin[0], hrFin[1], hrFin[2]);
          else if ((hrFin.length = 2)) dtFim.setHours(hrFin[0], hrFin[1]);
          else if ((hrFin.length = 1)) dtFim.setHours(hrFin[0]);

          var item = {
            id: eventos[x].Id,
            start: dtIni,
            end: dtFim,
            title:
              eventos[x].AlertarEventoMesmoDia === true || eventos[x].Tipo === 3
                ? "* " + eventos[x].Descricao
                : eventos[x].Descricao,
            color: eventos[x].BgColor,
            tipo: eventos[x].Tipo,
          };
          evt.push(item);
        }
        $("#lblFeriado").hide();
        $("#tFeriados").hide();

        if (
          responseData.feriados !== null &&
          responseData.feriados !== undefined &&
          responseData.feriados.length > 0
        ) {
          $("#tFeriados").show();

          var tableRef = $("#tFeriados tbody");
          tableRef.empty();

          for (var i = 0; i < responseData.feriados.length; i++) {
            var data = new Date(responseData.feriados[i].DataFeriado);
            var dia = data.getDate().toString().padStart(2, "0"),
              mes = (data.getMonth() + 1).toString().padStart(2, "0"), //+1 pois no getMonth Janeiro começa com zero.
              ano = data.getFullYear();
            var newRow = tableRef[0].insertRow(tableRef[0].rows.length);
            var newCell1 = newRow.insertCell(0);
            newCell1.innerHTML = "<b>" + dia + "/" + mes + "/" + ano + "</b>";
            newCell1.style.width = "20%";

            var newCell2 = newRow.insertCell(1);
            newCell2.innerHTML =
              "<b>" + responseData.feriados[i].Descricao + "</b>";
            newCell2.style.width = "80%";
          }
        } else {
          $("#lblFeriado").show();
        }
        successCallback(evt);
      });
    },
    eventClick: function (info) {
      var eventObj = info.event;
      var agendaId = eventObj.id;
      var tipoEvento = eventObj.extendedProps.tipo;
      var url = urlEdit + "?id=" + agendaId + "&tipoEvento=" + tipoEvento;
      $.popUpUrl(url, 600, 900, function () {
        reloadCalendar();
      });
    },
    dateClick: function (info) {
      if (info.date < new Date())
        $.bsMessageError("Atenção", "Data escolhida é menor que a Data Atual!");
      else {
        var url = urlCreate + "?clickDate=" + info.dateStr;
        $.popUpUrl(url, 600, 900, function () {
          reloadCalendar();
        });
      }
    },
  });
  calendar.render();
}

function FormatarData(data) {
  var dtStr = data.substr(0, 10).split("-");
  var hrStr = data.substr(11, 19).split(":");
  return new Date(
    parseInt(dtStr[0]),
    parseInt(dtStr[1]),
    parseInt(dtStr[2]),
    parseInt(hrStr[0]),
    parseInt(hrStr[1]),
    parseInt(hrStr[2])
  );
}
