var lista = window.parent.listaMembro;
var table;

var colunas = [
  {
    width: "13%",
    title: "Tipo",
    mRender: function (data, type, row) {
      return row.MembroId == 0 ? "Não Membro" : "Membro";
    },
  },
  { width: "25%", data: "Nome", title: "Nome" },
  { width: "15%", data: "CPF", title: "CPF" },
  { width: "20%", data: "Igreja", title: "Congregação/Igreja" },
  { width: "12%", data: "Cargo", title: "Cargo" },
  {
    width: "15%",
    title: "Críticas",
    mRender: function (data, type, row) {
      let erros = "";
      for (var j = 0; j < row.ErrosRetorno.length; j++) {
        erros += row.ErrosRetorno[j].Mensagem + "<br />";
      }
      return erros;
    },
  },
];

$(() => {
  "use strict";

  let nomeArquivo = $("#lblNomeArquivo").text().split("\\").pop();
  $("#lblNomeArquivo").text(nomeArquivo);

  let arquivoValido = false;
  arquivoValido =
    lista
      .map((e) => {
        return e.ErrosRetorno.map((a) => {
          return a.Mensagem;
        });
      })
      .filter((a) => a != "").length > 0;

  if (arquivoValido) {
    $("#btnConfirmar").hide();
    $("#checkTodos").hide();
    $("#tbMembro tbody tr")
      .find("input:checkbox")
      .each(function () {
        $(this).attr("disabled", "disabled");
      });
    $("#lblStatus").text(
      "Foram encontradas algumas inconsistências na Captura do arquivo. Ajustar conforme a lista abaixo."
    );
  } else {
    $("#lblStatus").text("Arquivo válido para captura.");
  }

  let membros = lista;
  if (table != undefined && table != null) table.destroy();
  table = $("#grdMembros").DataTable({
    search: true,
    lengthMenu: [
      [10, 25, 50, -1],
      [10, 25, 50, "Tudo"],
    ],
    language: {
      url: "/lib/plugins/datatables/pt_br.json",
    },
    aaData: membros,
    columns: colunas,
  });


  $("#btnConfirmar").click((e) => {
    $.showYesNoAlert(
      "Deseja efetivar a capturar do Arquivo?",
      "Captura de Arquivo",
      function () {
        var overlay = gerarOverlay();
        var loadingFrame = gerarLoadingFrame();

        overlay.append(loadingFrame);
        $("body").append(overlay);

        setTimeout(function () {
          overlay.focus();
        }, 0);

        var formData = new FormData();

        formData.append("files", parent.files[0]);
        formData.append("idPresenca", $("#IdPresenca").val());

        $.ajax({
          type: "POST",
          url: urlIncluir,
          contentType: false,
          processData: false,
          data: formData,
          success: function (result) {
            if (result.status == "OK") {
              Swal.fire("Inscrições", result.msg, "info").then(() => {
                $("#Arquivo", parent.document).val("");
                parent.ListarMembros($("#Presenca", parent.document).val(), $("#chkNaoMembro", parent.document).prop("checked"));
                parent.$.fancybox.close();
              });
            } else {
              $.alertError("Inscrição", result.msg);
            }
          },
          complete: function (param) {
            removerOverlay(null);
          },
        });
      }
    );
  });

  $("#btnCancelar").click((e) => {
    e.preventDefault;
    $("#Arquivo", parent.document).val("");
    parent.$.fancybox.close();
  });
});