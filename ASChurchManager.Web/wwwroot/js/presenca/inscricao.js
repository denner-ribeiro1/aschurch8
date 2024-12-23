var table;
var listaMembro = [];
var files;

var colunas = [
  {
    width: "5%",
    data: "MembroId",
    title: "RM",
    render: (data, type, row) => (data > 0 ? data : "N/M"),
  },
  { width: "25%", data: "Nome", title: "Nome" },
  { width: "10%", data: "CPF", title: "CPF" },
  { width: "27%", data: "Igreja", title: "Congregação/Igreja" },
  { width: "15%", data: "Cargo", title: "Cargo" },
  {
    width: "17%",
    orderable: false,
    searchable: false,
    mRender: function (data, type, row) {
      let valor = $("#Valor").val();
      let botoes = "";
      if (valor > 0)
        botoes += `<input type="button" value="${
          row.Pago ? "Pago" : "Não Pago"
        }" class="btn ${
          row.Pago ? "btn-primary" : "btn-secondary"
        } btn-sm" style=";width:80px;" onclick="AtualizarPago(${row.Id}, ${
          row.Pago
        }, this);">&nbsp;&nbsp;`;
      botoes += `<input type="button" value="Excluir" class="btn btn-danger btn-sm" style="width:80px" onclick="ExcluirRow(${row.Id}, this);">`;
      return botoes;
    },
  },
];

let isReadOnly = false;

$(() => {
  "use strict";

  isReadOnly = $("#IsReadOnly").val().toLowerCase() == "true";

  var paramsMembro = {
    CongregacaoId: $("#CongregacaoEventoId").val(),
  };

  $("#lnkMembroId").popUp(
    "Pesquisar Membro",
    "MembroObreiros",
    "MembroId",
    paramsMembro
  );

  $("#chkNaoMembro").prop("checked", false);
  $("#chkNaoMembro").iCheck("uncheck");

  $("#divManual").show();
  $("#divArquivo").hide();
  ListarMembros($("#Presenca").val());

  $("#Membro").change(function () {
    if ($("#Membro").prop("checked")) {
      $("#divMembro").show();
      $("#divNaoMembro").hide();

      $("#MembroId").removeAttr("readonly");
      $("#Cpf").attr("readonly", "readonly");
      $("#Nome").attr("readonly", "readonly");
      $("#Congregacao").attr("readonly", "readonly");
      $("#Cargo").attr("readonly", "readonly");
    } else {
      $("#divMembro").hide();
      $("#divNaoMembro").show();

      $("#Cpf").removeAttr("readonly");
      $("#MembroId").removeAttr("readonly");
      $("#Nome").removeAttr("readonly");
      $("#Congregacao").removeAttr("readonly");
      $("#Cargo").removeAttr("readonly");
    }
    LimparCampos();
  });

  $("#MembroId").change(function (e) {
    e.preventDefault;

    var value = $("#MembroId").val();
    LimparCampos();

    if (value > 0) {
      var requestData = {
        MembroId: value,
      };

      $.ajaxPost(urlBuscarMembro, requestData, function (membro) {
        if (membro.Id == 0) {
          $.bsMessageError("Atenção", "Membro não cadastrado!");
        } else {
          if (membro.Status != 1) {
            $.bsMessageError("Atenção", "Membro não Ativo!");
          } else if (membro.TipoMembro != 3) {
            $.bsMessageError("Atenção", "Membro inválido!");
          } else {
            if (
              $("#CongregacaoEventoId").val() > 0 &&
              $("#CongregacaoEventoId").val() != membro.Congregacao.Id
            ) {
              $.bsMessageError(
                "Atenção",
                `Evento exclusivo para a Congregação ${$(
                  "#CongregacaoEvento"
                ).val()}.</br>Membro pertence a Congregação ${
                  membro.Congregacao.Nome
                }!`
              );
              return;
            }

            $("#MembroId").val(membro.Id);
            $("#Nome").val(membro.Nome);
            $("#Congregacao").val(membro.Congregacao.Nome);

            if (membro.Cargos != null && membro.Cargos.length > 0) {
              var cargo = membro.Cargos.reduce(function (a, b) {
                return a.DataCargo > b.DataCargo ? a : b;
              });
              $("#Cargo").val(cargo.Descricao);
            }
          }
        }
      });
    }
  });

  $("#Cpf").change(function (e) {
    e.preventDefault;

    var value = $("#Cpf").val();
    LimparCampos();

    if (value != "" && value != undefined && value != null) {
      var requestData = {
        cpf: value,
      };

      $.ajaxPost(BuscarMembroCPF, requestData, function (retorno) {
        if (
          retorno.msg != "" &&
          retorno.msg != undefined &&
          retorno.msg != null
        ) {
          let msg = `${retorno.msg}<br />Deseja continuar a inscrição sem vincular o membro ao Curso?
                      <br /><br /><font color='red'>Importante: Ao realizar a inscrição como "Não Membro", o sistema não vinculará o membro ao curso, ou seja, não constará no histórico do membro o Curso/Evento.</font>`;
          $.showYesNoAlert(
            msg,
            "Inscrição de Não Membro",
            () => {
              $("#Cpf").val(value);
              $("#Nome").focus();
            },
            null,
            () => {
              $("#Membro").prop("checked", true);
              $("#Membro").change();
              if (retorno.id > 0) {
                $("#MembroId").val(retorno.id);
                $("#MembroId").change();
              }
            }
          );
        } else {
          $("#Cpf").val(value);
          $("#Nome").focus();
        }
      });
    }
  });

  $("#btnAddLista").click((e) => {
    e.preventDefault;

    if ($("#Membro").prop("checked")) {
      if ($("#MembroId").val() == null || $("#MembroId").val() == "") {
        $.bsMessageError("Atenção", "Favor selecionar um Membro");
        $("#MembroId").focus();
        return;
      }

      let requestData = {
        idPresenca: $("#Presenca").val(),
        idMembro: $("#MembroId").val(),
        pago: $("#Pago").prop("checked"),
      };
      $.ajaxPost(urlIncluir, requestData, function (ret) {
        if (ret.status == "OK") {
          ListarMembros(
            $("#Presenca").val(),
            $("#chkNaoMembro").prop("checked")
          );
          LimparCampos();
        } else {
          $.bsMessageError("Atenção", ret.Message);
        }
      });
    } else {
      let form = $("#frmInscricao");
      if (form.valid()) {
        let requestData = {
          idPresenca: $("#Presenca").val(),
          nome: $("#Nome").val(),
          cpf: $("#Cpf").val(),
          igreja: $("#Congregacao").val(),
          cargo: $("#Cargo").val(),
          pago: $("#Pago").prop("checked"),
        };
        $.ajaxPost(urlIncluirNaoMembros, requestData, function (ret) {
          if (ret.status == "OK") {
            ListarMembros(
              $("#Presenca").val(),
              $("#chkNaoMembro").prop("checked")
            );
            LimparCampos();
          } else {
            $.bsMessageError("Atenção", ret.Message);
          }
        });
      } else {
        var msg = ValidarFormulario();
        if (msg != null && msg != undefined && msg != "")
          $.alertError("Inscrições", msg);
      }
    }
  });

  $("#btnCancLista").click((e) => {
    e.preventDefault();
    LimparCampos();
    $("#MembroId").focus();
  });

  $("input[type=radio][name=optTipo]").change(function () {
    if (this.value == "M") {
      $("#divManual").show();
      $("#divArquivo").hide();
    } else if (this.value == "A") {
      $("#divManual").hide();
      $("#divArquivo").show();
    }
    LimparCampos();
  });

  $("#chkNaoMembro").on("ifChanged", function (event) {
    ListarMembros($("#Presenca").val(), $("#chkNaoMembro").prop("checked"));
  });

  $("#Arquivo").change((e) => {
    e.preventDefault;
    var extPermitidas = ["xlsx"];
    var extArquivo = $("#Arquivo").val().split(".").pop();

    if (
      typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) ==
      "undefined"
    ) {
      $.alertError(
        "Upload de Arquivo",
        "Somente arquivos do Tipo Excel são permitidos para Upload."
      );
      $("#Arquivo").val("");
    } else {
      files = e.target.files;
    }
  });

  $("#btnCapturar").click((e) => {
    let arquivo = $("#Arquivo").val();
    if (arquivo == null || arquivo == "") {
      $.bsMessageError("Atenção", "Favor selecionar o arquivo para Capturar!");
      $("#Arquivo").focus();
      return;
    }
    var extPermitidas = ["xlsx"];
    var extArquivo = arquivo.split(".").pop();

    if (
      typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) ==
      "undefined"
    ) {
      $.alertError(
        "Upload de Arquivo",
        "Somente arquivos do Tipo Excel são permitidos para Upload."
      );
      $("#Arquivo").val("");
    } else {
      var overlay = gerarOverlay();
      var loadingFrame = gerarLoadingFrame();

      overlay.append(loadingFrame);
      $("body").append(overlay);

      setTimeout(function () {
        overlay.focus();
      }, 0);

      var formData = new FormData();

      formData.append("files", files[0]);
      formData.append("id", $("#Presenca").val());

      $.ajax({
        type: "POST",
        url: urlCarregarArquivo,
        contentType: false,
        processData: false,
        data: formData,
        success: function (result) {
          if (result.status == "OK") {
            listaMembro = result.data;
            let url = `${urlPopUp}?id=${$(
              "#Presenca"
            ).val()}&arquivo=${arquivo}`;
            $.popUpUrl(url, 550, 800);
          } else {
            $.alertError("Captura de Arquivo", result.mensagem);
          }
        },
        complete: function (param) {
          removerOverlay(null);
        },
      });
    }
  });

  $("#btnCancelarCapt").click((e) => {
    $("#Arquivo").val("");
    files = null;
  });
});

function LimparCampos() {
  $("#MembroId").val("");
  $("#Cpf").val("");
  $("#Nome").val("");
  $("#Congregacao").val("");
  $("#Cargo").val("");
  $("#Pago").bootstrapToggle("off");
  $("#Pago").prop("checked", false);
  $("#chkNaoMembro").iCheck("uncheck");
}

function ExcluirRow(value, botao) {
  var $button = $(botao);

  let msg = `Deseja realmente excluir a inscrição de ${$($button.parents("tr"))
    .find("td:eq(1)")
    .text()} no Curso/Evento?`;
  if ($("#PermiteInscricoes").val() === "False") {
    msg = `Inscrições já encerradas, após a exclusão não será possivel reinscrever o Membro(a) (${$(
      $button.parents("tr")
    )
      .find("td:eq(1)")
      .text()}) no Curso/Evento. <br />Deseja realmente excluir o Membro(a)?`;
  }
  $.showYesNoAlert(msg, "Excluir Membro", function () {
    let requestData = {
      id: value,
    };
    $.ajaxPost(urlExcluir, requestData, function (ret) {
      if (ret.status == "OK") {
        table.row($button.parents("tr")).remove().draw();
        LimparCampos();
      } else {
        $.bsMessageError("Atenção", ret.Message);
      }
    });
  });
}

function AtualizarPago(id, pago, botao) {
  let requestData = {
    id: id,
    pago: !pago,
  };
  $.ajaxPost(urlAtualizarPago, requestData, function (ret) {
    if (ret.status == "OK") {
      if (pago === true) {
        $(botao).val("Não Pago");
        $(botao).removeClass("btn-primary");
        $(botao).addClass("btn-secondary");
      } else {
        $(botao).val("Pago");
        $(botao).removeClass("btn-secondary");
        $(botao).addClass("btn-primary");
      }
      $(botao).attr("onclick", `AtualizarPago(${id}, ${!pago}, this)`);
    } else {
      $.bsMessageError("Atenção", ret.Message);
    }
  });
}

function ListarMembros(idPresenca, somenteMembro) {
  let requestData = {
    idPresenca,
    somenteMembro,
  };

  $.ajaxPost(urlBuscarLista, requestData, function (ret) {
    if (ret.status == "OK") {
      let membros = ret.data;
      if (table != undefined && table != null) table.destroy();
      table = $("#grdMembros").DataTable({
        search: true,
        lengthMenu: [
          [15, 25, 50, 100, -1],
          [15, 25, 50, 100, "Tudo"],
        ],
        language: {
          url: "/lib/plugins/datatables/pt_br.json",
        },
        aaData: membros,
        columns: colunas,
      });
    } else {
      $.bsMessageError("Atenção", ret.Message);
    }
  });
}
