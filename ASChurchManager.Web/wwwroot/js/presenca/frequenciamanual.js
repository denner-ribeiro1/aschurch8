$(() => {
    "use strict";

    if ($("#Status").val() == "Finalizado") {
        $("#chkDatasFinalizadas").prop("checked", true);
        ExibirDatasFinalizadas($("#chkDatasFinalizadas"));
        $("#divCheck").hide();
    }
    else {
        ExibirDatasFinalizadas($("#chkDatasFinalizadas"));
    }

    $('#chkDatasFinalizadas').on('ifChanged', function (e) {
        e.preventDefault;
        ExibirDatasFinalizadas($("#chkDatasFinalizadas"));
    });

    $("#DataId").change((e) => {
        e.preventDefault;

        let idPres = $("#PresencaId").val();
        let idData = $('option:selected', $("#DataId")).val();
        let situacaoData = "";
        if ($('option:selected', $("#DataId")).text() != null && $('option:selected', $("#DataId")).text() != "")
            situacaoData = $('option:selected', $("#DataId")).text().split(' - ')[3].replace('Situação: ', '');

        if (situacaoData == "Finalizado") {
            $("#divReabrir").show();
            $("#divFinalizar").hide();
        }
        else if (situacaoData == "Aberto" || situacaoData == "Andamento") {
            $("#divReabrir").hide();
            $("#divFinalizar").show();
        }
        else {
            $("#divReabrir").hide();
            $("#divFinalizar").hide();
        }

        $('#grdInscricoes').jtable('load', {
            idPresenca: idPres,
            idData: idData
        });
    })

    HabilitarCampos($('#Filtro').val());

    $('#Filtro').change(function (e) {
        e.preventDefault();
        $("#Conteudo").val("");
        HabilitarCampos($('#Filtro').val());
    });

    $("#Conteudo").keypress(function (e) {
        if ($("#Filtro").val() === "Id" || $("#Filtro").val() === "MembroId") {
            if (e.keyCode === 16)
                isShift = true;

            //Allow only Numeric Keys.
            if (((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode === 8 || e.keyCode <= 37) && isShift === false)
                return true;
            return false;
        }
        else
            return true;
    });

    $("#btnPesquisar").click((e) => {
        e.preventDefault();
        let idPres = $("#PresencaId").val();
        let idData = $('option:selected', $("#DataId")).val();

        if (idData !== null && idData !== "") {
            let content = $('#Conteudo').val();
            if ($('#Filtro').val() === "Igreja")
                content = $('#CongregacaoSelecionado').val();
            else if ($('#Filtro').val() === "CPF")
                content = $('#CpfFiltro').val();

            $('#grdInscricoes').jtable('load', {
                idPresenca: idPres,
                idData: idData,
                filtro: $('#Filtro').val(),
                conteudo: content
            });
        }
        else {
            $.alertError("Frequência", "Favor selecionar uma data para pesquisar na Lista de Presença!");
        }
    })

    $("#btnFinalizar").click((e) => {
        e.preventDefault;
        let idData = $('option:selected', $("#DataId")).val();
        AtualizarSituacaoData(idData, true);
    })

    $("#btnReabrir").click((e) => {
        e.preventDefault;
        let idData = $('option:selected', $("#DataId")).val();
        AtualizarSituacaoData(idData, false);
    })

    $("#btnImprimir").click(function (event) {
        event.preventDefault();

        let idData = $('option:selected', $("#DataId")).val();
        let idPresenca = $("#PresencaId").val();

        if (idData !== null && idData !== "") {
            $.showYesNoAlert("Deseja imprimir a Lista de Presença?", "Lista de Presença", function () {
                var url = `${urlImpLista}?idPresenca=${idPresenca}&idData=${idData}`;
                $.downloadArquivo(url, $("#btnImprimir"));
            });
        }
        else {
            $.alertError("Lista de Presença", "Favor selecionar uma data para imprimir a Lista de Presença!");
        }
    });

    $('#grdInscricoes').jtable({
        paging: true, //Enable paging
        pageSize: 20, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Nome ASC',
        actions: {
            listAction: urlInscricoes
        },
        messages: {
            noDataAvailable: 'Não existem Inscrições a serem exibidas!'
        },
        fields: {
            Botoes: {
                title: '',
                width: '7%',
                sorting: false,
                display: function (data) {
                    return Botao(data.record.Id, data.record.Situacao, data.record.Tipo, data.record.Justificativa, data.record.StatusData);
                }
            },
            Id: {
                title: 'Inscr.Nº',
                width: '5%',
                key: true,
                columnResizable: false,
            },
            MembroId: {
                title: 'RM',
                width: '5%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Nome',
                width: '25%',
                key: false,
                columnResizable: false
            },
            CPF: {
                title: 'CPF',
                width: '10%',
                key: false,
                columnResizable: false
            },
            Igreja: {
                title: 'Congregação/Igreja',
                width: '25%',
                key: false,
                columnResizable: false
            },
            Tipo: {
                title: 'Registro',
                width: '5%',
                key: false,
                columnResizable: false
            },
        },
        recordsLoaded: function (event, data) {
            $("[data-toggle='toggle']").bootstrapToggle('destroy')
            $("[data-toggle='toggle']").bootstrapToggle();

            $("[data-toggle='toggle'] .ckbInscr").each((i, c) => {
                let cmp = `#lnk_${$(c).data("inscrid")}`;

                if ($(c).prop("checked") == true) {
                    $(cmp).hide();
                }
                else {
                    $(cmp).show();
                }
            })
        }
    });
})

function ExibirDatasFinalizadas(value) {
    let idPres = $("#PresencaId").val();
    let exibir = $(value).prop("checked");

    let requestData = {
        idPresenca: idPres,
        somenteFinalizados: exibir,
        incluirAndamento: true
    }

    $.ajaxPost(urlBuscarDatas, requestData, function (ret) {
        if (ret.status == "OK") {
            let itens = ret.data;

            $("#DataId").empty();
            let option = $("<option></option>").appendTo($('#DataId'));
            option.attr("value", "");
            option.html("");

            for (var i = 0; i < itens.length; i++) {
                option = $("<option></option>").appendTo($('#DataId'));
                option.attr("value", itens[i].Id);
                let descr = `Data: ${FormataData(itens[i].DataHoraInicio)} - Hora Início: ${FormataHora(itens[i].DataHoraInicio)} - Hora Fim: ${FormataHora(itens[i].DataHoraFim)} - Situação: ${FormatarTipo(itens[i].Status)}`;
                option.html(descr);
            }
            $('#grdInscricoes').jtable('load', {
                idPresenca: idPres,
                idData: 0
            });

            $("#divReabrir").hide();
            $("#divFinalizar").hide();
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });
}

function FormataData(datahora) {
    datahora = datahora.split('T');
    let data = datahora[0];
    let newData = data.split('-');
    return `${newData[2]}/${newData[1]}/${newData[0]}`;
}

function FormataHora(datahora) {
    datahora = datahora.split('T');
    return `${datahora[1]}`;
}

function FormatarTipo(valor) {
    switch (valor) {
        case 1: return "Aberto";
        case 2: return "Andamento";
        case 3: return "Finalizado"
        default: return "";
    }
}

function VerificarCheck(tipo, value) {
    if (tipo === true) {
        $.showYesNoAlert(`Membro confirmou a sua presença através do QrCode.<br />Deseja realmente alterar a sua presença no Curso/Evento?`,
            "Frequência",
            () => {
                let request = {
                    id: $(value).data("inscrid"),
                    idData: $('option:selected', $("#DataId")).val(),
                    situacao: $(value).prop('checked'),
                    justificativa: $(value).data("justificativa")
                };
                $.ajaxPost(urlAtualFreqManual, request, function (result) {
                    if (result.status === "OK") {
                        $('#grdInscricoes').jtable('reload');
                    }
                    else {
                        $.alertError("Frequência", result.Message);
                    }
                });

            },
            null,
            () => {
                $('#grdInscricoes').jtable('reload');
            });
    }
    else {
        let request = {
            id: $(value).data("inscrid"),
            idData: $('option:selected', $("#DataId")).val(),
            situacao: $(value).prop('checked'),
            justificativa: $(value).data("justificativa")
        };
        $.ajaxPost(urlAtualFreqManual, request, function (result) {
            if (result.status === "OK") {
                $('#grdInscricoes').jtable('reload');
            }
            else {
                $.alertError("Frequência", result.Message);
            }
        });
    }
}

function Botao(id, situacao, tipo, justificativa, status) {
    let checks = '';
    let verCheck = tipo === "Automática";
    let readOnly = !(status === "Em Aberto" || status === "Andamento");

    if (situacao === true) {
        checks = `<input type="checkbox" class="ckbInscr" onchange="VerificarCheck(${verCheck}, this)"
                    data-inscrid="${id}" checked="checked" data-tipo="${tipo}" id="chb_${id}"
                    ${readOnly ? ' disabled="disabled" ' : ''} data-toggle="toggle" data-on="Presente" data-off="Ausente">`;
        if (!readOnly) {
            checks += `<center>
                  <a href="#" onclick="Justificativa(${id}, this);return false;" id="lnk_${id}"
                   data-inscrid="${id}" data-tipo="${tipo}" data-justificativa="${justificativa}">
                    <span class="glyphicon glyphicon-align-justify" aria-hidden="true"></span> Justificar
                  </a>
                  </center>`;
        }
    }
    else {
        checks = `<input type="checkbox" class="ckbInscr" onchange="VerificarCheck(${verCheck}, this)"
                   data-inscrid="${id}" data-tipo="${tipo}" id="chb_${id}" data-justificativa="${justificativa}"
                   ${readOnly ? ' disabled="disabled" ' : ''} data-toggle="toggle" data-on="Presente" data-off="Ausente">
                  <br />`;

        if (justificativa == null || justificativa == "") {
            if (!readOnly) {
                checks += `<center>
                             <a href="#" onclick="Justificativa(${id}, this);return false;" id="lnk_${id}"
                              data-inscrid="${id}" data-tipo="${tipo}" data-justificativa="${justificativa}">
                                <span class="glyphicon glyphicon-align-justify" aria-hidden="true"></span> Justificar
                             </a>
                           </center>`;
            }
        }
        else {
            checks += `<center>
                <a href="#" onclick="Justificativa(${id}, this);return false;" id="lnk_${id}"
                 data-inscrid="${id}" data-tipo="${tipo}" data-justificativa="${justificativa}" data-somenteleitura="${readOnly}">
                    <span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Justificado
                </a>
            </center>`;
        }
    }
    return checks;
}

function HabilitarCampos(filtro) {
    if (filtro === "Igreja") {
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
        $("#CpfFiltro").hide();
    }
    else if (filtro === "CPF") {
        $("#CpfFiltro").show();
        $("#CongregacaoSelecionado").hide();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#CongregacaoSelecionado").hide();
        $("#CpfFiltro").hide();
    }
}

function AtualizarSituacaoData(id, finalizar) {
    $.showYesNoAlert(`Deseja ${finalizar ? "<b><font color='red'>FINALIZAR</font></b>" : "<b><font color='red'>REABRIR</font></b>"} a Lista de Presença para ${$("#Descricao").val()}?`,
        "Frequência",
        () => {
            let request = {
                idData: id,
                finalizar: finalizar
            };
            $.ajaxPost(urlSalvar, request, function (result) {
                if (result.status === "OK") {
                    document.location.reload(true);
                }
                else {
                    $.alertError("Frequência", result.Message);
                }
            });

        });
}