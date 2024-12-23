$(function () {
    "use strict";

    if (document.getElementById("DataBatismo").options.length === 1) {
        PesquisaDadosBatismo($("#DataBatismo").val());
    }

    $("#DataBatismo").change((e) => {
        e.preventDefault();
        PesquisaDadosBatismo($("#DataBatismo").val());
    });

    $("#btnSalvarLista").click(function (event) {
        event.preventDefault();
        if ($("#DataBatismo").val() !== null && $("#DataBatismo").val() !== "") {
            $.showYesNoAlert(`Deseja Salvar a Lista de Presença do Batismo para a data ${$("#DataBatismo").find("option:selected").text()}?`, "Lista de Presença", function () {
                var request = {
                    idBatismo: $("#DataBatismo").val(),
                    candidatos: PreparaLista()
                };
                $.ajaxPost(urlSalvar, request, function (result) {
                    if (result.status === "OK") {
                        window.location = result.url;
                    }
                    else {
                        $.alertError("Lista de Presença", result.msg);
                    }
                });
            });
        }
        else {
            $.alertError("Lista de Presença", "Favor selecionar uma data para o Batismo para Salvar a Lista!");
        }
    });

    $("#lnkRelListaBatismo").click(function (event) {
        event.preventDefault();
        if ($("#DataBatismo").val() !== null && $("#DataBatismo").val() !== "") {
            $.showYesNoAlert("Deseja imprimir a Lista de Presença?", "Lista de Presença", function () {
                var batismoId = $("#DataBatismo").val();
                var dtBatismo = $("#DataBatismo").find("option:selected").text();
                var url = "/Secretaria/Relatorios/RelatorioCandidatosBatismo?congregacao=Todas&tipoSaida=PDF&batismoId=" + batismoId + "&dataBatismo=" + dtBatismo.replace("/", "-").replace("/", "-") + "&situacao=0";
                $.downloadArquivo(url, $("#btnRelatorioListaBatismo"));
            });
        }
        else {
            $.alertError("Lista de Presença", "Favor selecionar uma data para o Batismo para imprimir a Lista de Presença!");
        }
    });
});

function PreparaLista() {
    var membro = [];
    $(".ckbMembro").each(function (i, x) {
        membro.push({
            MembroId: $(x).data('membroid'),
            Situacao: $(x).prop('checked')
        });
    });
    return membro;
}

function PesquisaDadosBatismo(idBatismo) {
    if (idBatismo != undefined &&
        idBatismo != null &&
        idBatismo != "") {
        let urlpastor = `${urlPastores}?idBatismo=${idBatismo}`;
        $.ajaxGet(urlpastor, (result) => {
            $('#divPastores').html(result);
        });

        let urlCand = `${urlCandidatos}?idBatismo=${idBatismo}`;
        $.ajaxGet(urlCand, (result) => {
            $('#divCandidatos').html(result);
            $("[data-toggle='toggle']").bootstrapToggle('destroy')
            $("[data-toggle='toggle']").bootstrapToggle();
        });
    }
    else {
        $('#divPastores').html("");
        $('#divCandidatos').html("");
    }
}