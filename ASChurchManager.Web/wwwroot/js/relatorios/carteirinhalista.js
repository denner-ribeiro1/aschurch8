var table;
var colunas = [
  {
    width: "2%",
    data: null,
    searchable: false,
    render: DataTable.render.select(),
  },
  { width: "5%", data: "Id", title: "Cód.Membro" },
  { width: "28%", data: "Nome", title: "Nome" },
  { width: "10%", data: "Cpf", title: "CPF" },
  { width: "25%", data: "Congregacao", title: "Congregação" },
  { width: "20%", data: "Cargo", title: "Cargo" },
  {
    width: "10%",
    data: "DataValidadeCarteirinha",
    searchable: false,
    title: "Data de Validade",
  },
];

$(function () {
  $("#divBatismo").hide();
  $("#divMembro").show();
  $("#divPesquisar").show();
  $("select").selectpicker();

  table = $("#grdMembros").DataTable({
    search: true,
    language: {
      url: "/lib/plugins/datatables/pt_br.json",
    },
    lengthMenu: [],
    aaData: [],
    columns: colunas,
    select: {
      info: false,
    },
  });

  $("#rdoMembro").change(function () {
    LimparCampos(true);
  });

  $("#rdoBatismo").change(function () {
    LimparCampos(false);
  });

  $("#CongregacaoMembro").change(function () {
    table.clear().draw();
  });

  $("#Cargos").change(function () {
    table.clear().draw();
    $("#divObrCongr").show();
    if (
      $("#Cargos").val().length == 0 ||
      parseInt(
        $("#Cargos")
          .val()
          .find((carg) => {
            return parseInt(carg) == 0;
          })
      ) == 0
    ) {
      $("#divObrCongr").hide();
    }
  });

  $("#DataBatismo").change(function () {
    table.clear().draw();
  });

  $("#CongregacaoBatismo").change(function () {
    table.clear().draw();
  });

  $("#btnPesquisar").click(function (event) {
    event.preventDefault();

    if ($("#rdoMembro")[0].checked) {
      var errorCongr = $(document).find(
        "[data-valmsg-for='CongregacaoMembro']"
      );
      errorCongr.html("");

      let congr = $("#CongregacaoMembro").val();
      if (congr.length == 0) {
        errorCongr.html(
          '<span id="CongregacaoMembro-error" class="">Congregação é de preenchimento obrigatório</span>'
        );
        $.alertError(
          "Carteirinha - Membros",
          "Congregação é de preenchimento obrigatório"
        );
        return false;
      }

      ListarCarteirinhas(
        $("#rdoMembro")[0].checked,
        $("#CongregacaoMembro").val(),
        $("#Cargos").val(),
        0,
        $('#chkObrCongr').is(':visible') && $("#chkObrCongr").prop("checked")
      );
    } else if ($("#rdoBatismo")[0].checked) {
      var errorData = $(document).find("[data-valmsg-for='DataBatismo']");
      errorData.html("");

      let idBatismo = $("option:selected", $("#DataBatismo")).val();
      if (idBatismo == undefined || idBatismo == "") {
        errorData.html(
          '<span id="DataBatismo-error" class="">Data de Batismo é de preenchimento obrigatório</span>'
        );
        $.alertError(
          "Carteirinha - Membros",
          "Data de Batismo é de preenchimento obrigatório"
        );
        return false;
      }

      ListarCarteirinhas(
        $("#rdoMembro")[0].checked,
        $("#CongregacaoBatismo").val(),
        null,
        idBatismo,
        $('#chkObrCongr').is(':visible') && $("#chkObrCongr").prop("checked")
      );
    } else {
      $.alertError("Carteirinha - Membros", "Selecione um filtro válido");
      return false;
    }
  });

  $("#btnCarteirinha").click(function (event) {
    event.preventDefault();
    let qtd = table.rows({ selected: true }).data().toArray().length;
    if (qtd < 501) {
      if (qtd > 0)
        if ($("#ckAtualizar")[0].checked) {
          Swal.fire({
            title: "<strong>Membro</strong>",
            html: `Imprimir a${qtd > 1 ? "s" : ""} carteirinha${
              qtd > 1 ? "s" : ""
            } selecionada${qtd > 1 ? "s" : ""} (${qtd})?`,
            icon: "question",
            input: "checkbox",
            inputValue: 1,
            inputPlaceholder:
              "&nbsp;&nbsp;Atualizar a Data de Validade das Carteirinhas dos membros.",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: '<i class="fa fa-check-circle"></i> Sim',
            cancelButtonText: '<i class="fa fa-ban"></i> Não',
            width: 500,
            allowOutsideClick: false,
          }).then((result) => {
            if (result.value || result.value === 0 || result.value === 1) {
              DownloadArquivo(result.value === 1);
            }
          });
        } else {
          $.showYesNoAlert(
            `Imprimir a${qtd > 1 ? "s" : ""} carteirinha${
              qtd > 1 ? "s" : ""
            } selecionada${qtd > 1 ? "s" : ""} (${qtd})?`,
            "Membro",
            function () {
              DownloadArquivo(false);
            }
          );
        }
      else {
        $.alertError(
          "Carteirinhas",
          "Não há membros selecionados para a geração das Carteirinhas!"
        );
      }
    } else {
      $.alertError(
        "Carteirinhas",
        `Quantidade de carteirinhas (${qtd}) selecionadas maior que a quantidade máxima (500 membros) permitida.`
      );
    }
  });
});

function LimparCampos(membro) {
  $("#DataBatismo option").prop("selected", function () {
    return this.defaultSelected;
  });

  $("#CongregacaoMembro").selectpicker("val", "");
  $("#CongregacaoMembro").selectpicker("refresh");
  $("#Cargos").selectpicker("val", "");
  $("#Cargos").selectpicker("refresh");
  $("#DataBatismo").selectpicker("val", "");
  $("#DataBatismo").selectpicker("refresh");
  $("#CongregacaoBatismo").selectpicker("val", "");
  $("#CongregacaoBatismo").selectpicker("refresh");

  $(document).find("[data-valmsg-for='CongregacaoMembro']").html("");
  $(document).find("[data-valmsg-for='DataBatismo']").html("");

  if (table != null) table.clear().draw();

  if (membro) {
    $("#divBatismo").hide();
    $("#divMembro").show();
    $("#divPesquisar").show();
  } else {
    $("#divBatismo").show();
    $("#divMembro").hide();
    $("#divPesquisar").show();
  }
}

function ListarCarteirinhas(
  pesqMembro,
  congregacao,
  cargo,
  idBatismo,
  imprimirObrVinc
) {
  var requestData = {
    pesqMembro,
    congregacao,
    cargo,
    idBatismo,
    imprimirObrVinc,
  };

  $.ajax({
    type: "POST",
    url: urlListarCarteirinhas,
    data: requestData,
    filter: true,
    success: function (result) {
      if (
        JSON.parse(result).find(
          (x) =>
            x.DataValidadeCarteirinha != null &&
            x.DataValidadeCarteirinha != undefined &&
            x.DataValidadeCarteirinha != "" &&
            x.Cargo != null &&
            x.Cargo != undefined &&
            x.Cargo != ""
        ) != null
      ) {
        $("#divAtualizar").show();
      } else {
        $("#divAtualizar").hide();
      }

      table.destroy();

      table = $("#grdMembros").DataTable({
        search: true,
        lengthMenu: [
          [15, 25, 50, 100, -1],
          [15, 25, 50, 100, "Tudo"],
        ],
        language: {
          url: "/lib/plugins/datatables/pt_br.json",
        },
        aaData: JSON.parse(result),
        columns: colunas,
        select: {
          style: "multi",
          info: false,
        },
      });
    },
  });
}

function DownloadArquivo(atualizarDtVal) {
  $("#btnCarteirinha").prop("disabled", true);
  var overlay = gerarOverlay();
  var loadingFrame = gerarLoadingFrame();

  overlay.append(loadingFrame);
  $("body").append(overlay);

  setTimeout(function () {
    overlay.focus();
  }, 0);

  var request = new XMLHttpRequest();
  request.onreadystatechange = function () {
    if (request.readyState == 4) {
      if (request.status == 200) {
        var disposition = request
          .getResponseHeader("content-disposition")
          .replace(/"/g, "");
        var strAux = disposition.substring(
          disposition.indexOf("filename=") + 9
        );
        var strAuxArray = strAux.split(";");
        var filename = strAuxArray[0];
        if (request.response != null && navigator.msSaveBlob)
          return navigator.msSaveBlob(
            new Blob([request.response], { type: request.response.type }),
            filename
          );

        var blob = new Blob([request.response], {
          type: request.response.type,
        });
        var link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        if (atualizarDtVal) {
          $("#ckAtualizar").iCheck("uncheck");
          let idBatismo = $("option:selected", $("#DataBatismo")).val();
          ListarCarteirinhas(
            $("#rdoMembro")[0].checked,
            $("#rdoMembro")[0].checked
              ? $("#CongregacaoMembro").val()
              : $("#CongregacaoBatismo").val(),
            $("#Cargos").val(),
            idBatismo,
            $('#chkObrCongr').is(':visible') && $("#chkObrCongr").prop("checked")
          );
        }
      } else {
        removerOverlay(null);

        var blob2 = new Blob([request.response], {
          type: request.response.type,
        });
        var reader = new FileReader();
        reader.onload = function () {
          $.alertError("Download", reader.result);
        };
        reader.readAsText(blob2);
      }
      removerOverlay(null);
      $("#btnCarteirinha").prop("disabled", false);
    }
  };
  request.open("POST", urlCarteirinhas, true);
  request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
  request.responseType = "blob";

  /*SELECIONANDO TODAS AS LINHAS QUE FORAM INDICADAS.*/
  var dataTableRows = table.rows({ selected: true }).data().toArray();
  const mem = dataTableRows.reduce((acc, curr) => {
    acc.push(curr.Id);
    return acc;
  }, []);

  let input = {};
  input.Membros = mem;
  input.AtualizaDtValidade = atualizarDtVal;

  request.send(JSON.stringify(input));
}
