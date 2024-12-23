var enviarBatismo = false;

$(function () {
    //$('.box').boxWidget('toggle');

    enviarBatismo = $("#EnviarBatismo").val() === "True";

    HabilitarCampos($('#Filtro').val());

    $('#Filtro').change(function (e) {
        $("#Conteudo").val("");
        HabilitarCampos($('#Filtro').val());
        e.preventDefault();
    });

    $("#Conteudo").keypress(function (e) {
        if ($("#Filtro").val() === "Id") {
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

    $('#grdMembro').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Id ASC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Congregados a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '10%',
                key: true,
                columnResizable: false
            },
            Nome: {
                title: 'Nome',
                width: '30%',
                key: false,
                columnResizable: false
            },
            Congregacao: {
                title: 'Congregação',
                width: '25%',
                key: false,
                columnResizable: false
            },
            Status: {
                title: 'Status',
                width: '10%',
                key: false,
                columnResizable: true
            },
            Botoes: {
                title: '',
                width: '40%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id, data.record.Status);
                }
            }
        }
    });

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "CongregacaoId")
            content = $('#CongregacaoSelecionado').val();
        var status = "";
        if ($('#Filtro').val() === "Status")
            status = $('#StatusSelecionado').val();
        
        $('#grdMembro').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: content,
            status: status
        });
    });

    //Load all records when page is first shown
    $('#btnPesquisar').click();
});

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoId") {
        $("#StatusSelecionado").hide();
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
    }
    else if (filtro === "Status") {
        $("#CongregacaoSelecionado").hide();
        $("#Conteudo").hide();
        $("#StatusSelecionado").show();
    }
    else {
        $("#Conteudo").show();
        $("#CongregacaoSelecionado").hide();
        $("#StatusSelecionado").hide();
    }
}

function botao(id, status) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button> <span>&nbsp</span>';

    if (enviarBatismo && status !== "Inativo") {
        botoes += `<button type="button" class="btn btn-success btn-sm" id="btnAprovar" ` +
            `onclick = "javascript:window.location.href='${urlEnviarBatismo}/${id}?enviar=true'"; return false;> ` +
            `    <span class="glyphicon glyphicon-user"></span> Enviar p/ Batismo` +
            `</button> <span>&nbsp</span>`;
    }
    return botoes;
}