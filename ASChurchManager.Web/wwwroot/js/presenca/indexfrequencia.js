$(() => {
    "use strict";

    CarregarTable(lPresAnd);

    $("#PresencaId").change((e) => {
        e.preventDefault;

        let requestData = {
            idPresenca: $('option:selected', $("#PresencaId")).val()
        }

        $.ajaxPost(urlBuscarDatas, requestData, function (ret) {
            if (ret.status == "OK") {
                let itens = ret.data;

                $("#DataId").empty();
                for (var i = 0; i < itens.length; i++) {
                    var option = $("<option></option>").appendTo($('#DataId'));
                    option.attr("value", itens[i].Id);
                    let descr = `${FormataData(itens[i].DataHoraInicio)} - Início: ${FormataHora(itens[i].DataHoraInicio)} - Fim: ${FormataHora(itens[i].DataHoraFim)}`;
                    option.html(descr);
                }

            }
            else {
                $.bsMessageError("Atenção", ret.Message);
            }
        });
    });

    $("#btnIniciar").click((e) => {
        e.preventDefault;
        if ($('option:selected', $("#PresencaId")).text() == null || $('option:selected', $("#PresencaId")).text() == "") {
            $.bsMessageError("Atenção", "Favor selecionar um Curso/Evento!");
            $("#PresencaId").focus();
            return;
        }

        if ($('option:selected', $("#DataId")).text() == null || $('option:selected', $("#DataId")).text() == "") {
            $.bsMessageError("Atenção", "Favor selecionar uma Data!");
            $("#DataId").focus();
            return;
        }
        let requestData = {
            presencaId: $('option:selected', $("#PresencaId")).val(),
            dataId: $('option:selected', $("#DataId")).val(),
            tipo: "AND"
        }

        $.ajaxPost(urlIniciarLista, requestData, function (ret) {
            if (ret.status == "OK") {
                CarregarTable(ret.data)
                if ($('option:selected', $("#PresencaId")).text() != null || $('option:selected', $("#PresencaId")).text() != "") {
                    $("#PresencaId").change();
                }
            }
            else {
                $.bsMessageError("Atenção", ret.Message);
            }
        });

    })

    $("#btnRegistrar").click((e) => {
        e.preventDefault;
        createPopupWin(urlPopUp, 'Frequencia', 1200, 650);
    })

});

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

function FormataDataHora(datahora) {
    datahora = datahora.split('T');
    let data = datahora[0];
    let newData = data.split('-');
    return `${newData[2]}/${newData[1]}/${newData[0]} - ${datahora[1]}`;
}
function CarregarTable(eventos) {
    let table = document.getElementById("tbEvento").getElementsByTagName("tbody")[0];
    $(table).empty();

    if (eventos.length > 0) {
        for (var i = 0; i < eventos.length; i++) {
            IncluirLinhaTabela(eventos[i], i);
        }
    }
    else {
        let row = table.insertRow(0);
        row.className = "jtable-data-row";
        $.incluirCelulasTabela(row, 0, "100%", "Não há Cursos/Eventos iniciados.", 2);
    }
}

function IncluirLinhaTabela(evento, i) {
    let table = document.getElementById("tbEvento").getElementsByTagName("tbody")[0];

    for (var i = 0; i < evento.Datas.length; i++) {
        let row = table.insertRow(i);
        row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");

        let { DataHoraInicio, DataHoraFim, Id } = evento.Datas[i];
        let botoes = '';
        botoes += `<button type="button" class="btn btn-app" style="margin-left: 5px;margin-top:10px;padding: 9px 5px;height:52px" onclick="PararFrequencia(${Id}, ${evento.Id}, this);">
                       <i class="fa fa-pause"></i> Parar
                   </button>`;
        $.incluirCelulasTabela(row, 0, null, botoes);
        
        let descr = `${evento.Descricao} - Início: ${FormataDataHora(DataHoraInicio)} - Fim: ${FormataDataHora(DataHoraFim)}`;
        $.incluirCelulasTabela(row, 1, null, descr);
    }

}


function PararFrequencia(id, eventoId, valor) {
    let requestData = {
        presencaId: eventoId,
        dataId: id
    }

    $.ajaxPost(urlIniciarLista, requestData, function (ret) {
        if (ret.status == "OK") {
            CarregarTable(ret.data)
            if ($('option:selected', $("#PresencaId")).text() != null || $('option:selected', $("#PresencaId")).text() != "") {
                $("#PresencaId").change();
            }
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });
}

function createPopupWin(pageURL, pageTitle,
    popupWinWidth, popupWinHeight) {
    var left = (screen.width - popupWinWidth) / 2;
    var top = (screen.height - popupWinHeight) / 4;
    var myWindow = window.open(pageURL, pageTitle,
        'resizable=yes, width=' + popupWinWidth
        + ', height=' + popupWinHeight + ', top='
        + top + ', left=' + left);
}