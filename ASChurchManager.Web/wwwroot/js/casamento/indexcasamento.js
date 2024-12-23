$(function () {
    //$('.box').boxWidget('toggle');

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

    $('#grdCasamento').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Id ASC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Casamentos a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '5%',
                key: true,
                columnResizable: false,
            },
            CongregacaoNome: {
                title: 'Congregação',
                width: '20%',
                key: false,
                columnResizable: false
            },
            DataCasamento: {
                title: 'Data',
                width: '6%',
                key: false,
                columnResizable: false,
                sorting: true,
                display: function (data) {
                    var d = data.record.DataCasamento.substring(0, 10).split("-");
                    return d[2] + "/" + d[1] + "/" + d[0];
                },
                listClass: "Centeralign"
            },
            HoraInicio: {
                title: 'Hora Início',
                width: '7%',
                key: false,
                sorting: false,
                columnResizable: false,
                listClass: "Centeralign"
            },
            NoivoNome: {
                title: 'Nome do Noivo',
                width: '20%',
                key: false,
                columnResizable: false
            },
            NoivaNome: {
                title: 'Nome do Noiva',
                width: '20%',
                key: false,
                columnResizable: false
            },
            Botoes: {
                title: '',
                width: '5%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                }
            }
        }
    });

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "CongregacaoNome")
            content = $('#CongregacaoSelecionado').val();
        
        $('#grdCasamento').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: content
        });
    });

    //Load all records when page is first shown
    $('#btnPesquisar').click();
});

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoNome") {
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#CongregacaoSelecionado").hide();
    }
}

function botao(id) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
    return botoes;
}