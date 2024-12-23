$(function () {
  atualizarGrafico();
  $("#grdMembrosPendentes").jtable({
    paging: true, //Enable paging
    pageSize: 10, //Set page size (default: 10)
    sorting: true, //Enable sorting
    pageSizeChangeArea: false,
    defaultSorting: "Id ASC",
    actions: {
      listAction: urlMembroPend,
    },
    messages: {
      noDataAvailable: "Não existem Membros a serem exibidos no momento!",
    },
    fields: {
      Id: {
        title: "Código",
        width: "6%",
        key: true,
        columnResizable: false,
        display: function (data) {
          if (data.record.Status === "Não Aprovado")
            return `<a href='${urlDet}/${data.record.Id}'>${data.record.Id}</a>`;
          else
            return `<a href='${urlAprovar}/${data.record.Id}'>${data.record.Id}</a>`;
        },
      },
      Nome: {
        title: "Nome",
        width: "18%",
        key: false,
        columnResizable: false,
      },
      Congregacao: {
        title: "Congregação",
        width: "18%",
        key: false,
        columnResizable: false,
      },
      Status: {
        title: "Status",
        width: "10%",
        key: false,
        columnResizable: true,
        display: function (data) {
          if (data.record.Status === "Não Aprovado")
            return `<span class="label label-danger">${data.record.Status}</span>`;
          else
            return `<span class="label label-warning">${data.record.Status}</span>`;
        },
      },
    },
  });
  $("#grdMembrosPendentes").jtable("load");

  $("#grdCarta").jtable({
    paging: true, //Enable paging
    pageSize: 10, //Set page size (default: 10)
    sorting: true, //Enable sorting
    pageSizeChangeArea: false,
    defaultSorting: "Id DESC",
    actions: {
      listAction: urlCartaPend,
    },
    messages: {
      noDataAvailable: "Não existem Cartas a serem exibidos no momento!",
    },
    fields: {
      Id: {
        title: "Código",
        width: "6%",
        key: true,
        columnResizable: false,
        display: function (data) {
          return `<a href='${urlAprovCarta}/${data.record.Id}'>${data.record.Id}</a>`;
        },
      },
      Nome: {
        title: "Nome",
        width: "20%",
        key: false,
        columnResizable: false,
      },
      CongregacaoDestino: {
        title: "Congregação Destino",
        width: "15%",
        key: false,
        columnResizable: false,
      },
    },
  });
  $("#grdCarta").jtable("load");

  var tabela = {
    paging: false, //Enable paging
    pageSize: 10, //Set page size (default: 10)
    sorting: false, //Enable sorting
    pageSizeChangeArea: false,
    actions: {
      listAction: urlEvento,
    },
    messages: {
      noDataAvailable: "Não existem Eventos a serem exibidos no momento!",
    },
    fields: {
      CongregacaoNome: {
        title: "Congregação",
        width: "15%",
        key: false,
        columnResizable: false,
        display: function (data) {
          if (
            data.record.AlertarEventoMesmoDia === true ||
            data.record.Tipo === 3
          )
            return `${data.record.CongregacaoNome} <span class="label label-danger"> Obrigatório </span>`;
          else return data.record.CongregacaoNome;
        },
      },
      TipoEventoDescr: {
        title: "Tipo",
        width: "10%",
        key: false,
        columnResizable: false,
      },
      Descricao: {
        title: "Descrição",
        width: "45%",
        key: true,
        columnResizable: false,
      },
      DataInicio: {
        title: "Data",
        width: "10%",
        key: false,
        display: function (data) {
          var d = data.record.DataInicio.substring(0, 10).split("-");
          return `${d[2]}/${d[1]}/${d[0]}`;
        },
        columnResizable: false,
      },
      HoraInicioStr: {
        title: "Inicio",
        width: "10%",
        key: false,
        columnResizable: false,
      },
      HoraFinalStr: {
        title: "Fim",
        width: "10%",
        key: false,
        columnResizable: false,
      },
    },
  };
  $("#gridEventos").jtable(tabela);
  $("#gridEventos").jtable("load", { tipoPesq: "N" });

  $("#gridEventosSede").jtable(tabela);
  $("#gridEventosSede").jtable("load", { tipoPesq: "S" });

  var calendarEl = document.getElementById("calendar");

  var calendar = new FullCalendar.Calendar(calendarEl, {
    plugins: ["timeGrid"],
    defaultView: "timeGrid",
    locale: "pt-br",
    header: {
      left: "",
      center: "title",
      right: "",
    },
    allDaySlot: false,
    nowIndicator: true,
    visibleRange: function (currentDate) {
      // Generate a new date for manipulating in the next step
      var startDate = new Date(currentDate.valueOf());
      var endDate = new Date(currentDate.valueOf());

      // Adjust the start & end dates, respectively
      startDate.setDate(startDate.getDate()); // One day in the past
      endDate.setDate(endDate.getDate() + 6); // Two days into the future

      return { start: startDate, end: endDate };
    },
    events: function (info, successCallback) {
      var request = {
        tipoPesq: "",
      };
      $.ajax({
        cache: false,
        type: "POST",
        url: "/Secretaria/Dashboard/Eventos",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(request),
        error: function (xhr, status, error) {
          console.log(error);
          console.log(xhr.responseText);
        },
        success: function (responseData) {
          var evt = [];
          eventos = responseData.Records;
          for (var x = 0; x < eventos.length; x++) {
            let dtIni = new Date(eventos[x].DataInicio);
            let hrIni = eventos[x].HoraInicio.split(":");
            if ((hrIni.length = 3))
              dtIni.setHours(hrIni[0], hrIni[1], hrIni[2]);
            else if ((hrIni.length = 2)) dtIni.setHours(hrIni[0], hrIni[1]);
            else if ((hrIni.length = 1)) dtIni.setHours(hrIni[0]);

            let dtFim = new Date(eventos[x].DataFim);
            let hrFin = eventos[x].HoraFinal.split(":");
            if ((hrFin.length = 3))
              dtFim.setHours(hrFin[0], hrFin[1], hrFin[2]);
            else if ((hrFin.length = 2)) dtFim.setHours(hrFin[0], hrFin[1]);
            else if ((hrFin.length = 1)) dtFim.setHours(hrFin[0]);

            var item = {
              id: eventos[x].Id,
              start: new Date(dtIni),
              end: new Date(dtFim),
              title:
                eventos[x].AlertarEventoMesmoDia === true ||
                eventos[x].Tipo === 3
                  ? "* " + eventos[x].Descricao
                  : eventos[x].Descricao,
              color: eventos[x].BgColor,
              tipo: eventos[x].Tipo,
            };
            evt.push(item);
          }
          successCallback(evt);
        },
      });
    },
  });
  calendar.render();
});

function atualizarGrafico() {
  $.ajaxPost(
    "/Secretaria/Dashboard/RetornaGrafico",
    null,
    function (partialViewResult) {
      $("#divGraficos").html(partialViewResult, CarregarGrafico);
    },
    "html"
  );
}

function CarregarGrafico() {
  var dados = sit;
  var pieChartCanvas = $("#pieChart")[0].getContext("2d");
  var pieChart = new Chart(pieChartCanvas);

  var PieData = [];
  $.each(dados, function (i, v) {
    switch (v.Situacao) {
      case 1:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#00c0ef",
            highlight: "#00c0ef",
            label: "Ativos",
          });
        }
        break;
      case 2:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#00a65a",
            highlight: "#00a65a",
            label: "Inativo",
          });
        }
        break;
      case 3:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#f39c12",
            highlight: "#f39c12",
            label: "Pendente Aprovação",
          });
        }
        break;
      case 4:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#f56954",
            highlight: "#f56954",
            label: "Não Aprovado",
          });
        }
        break;
      case 5:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#3c8dbc",
            highlight: "#3c8dbc",
            label: "Mudou-se",
          });
        }
        break;
      case 6:
        {
          PieData.push({
            value: v.Quantidade,
            color: "#d2d6de",
            highlight: "#d2d6de",
            label: "Falecido",
          });
        }
        break;
    }
  });

  var pieOptions = {
    // Boolean - Whether we should show a stroke on each segment
    segmentShowStroke: true,
    // String - The colour of each segment stroke
    segmentStrokeColor: "#fff",
    // Number - The width of each segment stroke
    segmentStrokeWidth: 1,
    // Number - The percentage of the chart that we cut out of the middle
    percentageInnerCutout: 50, // This is 0 for Pie charts
    // Number - Amount of animation steps
    animationSteps: 100,
    // String - Animation easing effect
    animationEasing: "easeOutBounce",
    // Boolean - Whether we animate the rotation of the Doughnut
    animateRotate: true,
    // Boolean - Whether we animate scaling the Doughnut from the centre
    animateScale: false,
    // Boolean - whether to make the chart responsive to window resizing
    responsive: true,
    // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
    maintainAspectRatio: false,
    // String - A legend template
    legendTemplate:
      "<ul class='<%=name.toLowerCase()%>-legend'><% for (var i=0; i<segments.length; i++){%><li><span style='background-color:<%=segments[i].fillColor%>'></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
    // String - A tooltip template
    tooltipTemplate: "<%=value %> Membro(s) na Situação <%=label%>",
  };
  pieChart.Doughnut(PieData, pieOptions);
}
