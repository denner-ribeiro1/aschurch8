$(function () {
    //$('.box').boxWidget('toggle');

    var Status = $.getQuerystringValue("Status");
    if (Status !== "") {
        $('#Filtro').val("TipoCarta_Status");
        $('#StatusCartaSelecionado').val(Status);
    }


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

    $('#grdCarta').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Id DESC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Cartas a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '6%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Nome',
                width: '20%',
                key: false,
                columnResizable: false
            },
            CongregacaoOrigem: {
                title: 'Congregação Origem',
                width: '15%',
                key: false,
                columnResizable: false
            },
            CongregacaoDestino: {
                title: 'Congregação Destino',
                width: '15%',
                key: false,
                columnResizable: false
            },
            TipoCarta: {
                title: 'Tipo',
                width: '6%',
                key: false,
                columnResizable: false
            },
            StatusCarta: {
                title: 'Status',
                width: '6%',
                key: false,
                columnResizable: false
            },
            DataValidade: {
                title: 'Validade',
                width: '6%',
                key: false,
                columnResizable: false,
                listClass: "Centeralign",
                display: function (data) {
                    var d = data.record.DataValidade.substring(0, 10).split("-");
                    return d[2] + "/" + d[1] + "/" + d[0];
                },
            },
            Botoes: {
                title: '',
                width: '25%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id, data.record.TemplateId, data.record.StatusCarta, data.record.AprovarCarta);
                }
            }
        }
    });

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var status = "";
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "CongregacaoOrigem")
            content = $('#CongregacaoSelecionado').val();
        else if ($('#Filtro').val() === "TipoCarta_Status") {
            content = $('#TipoCartaSelecionado').val();
            status = $('#StatusCartaSelecionado').val();
        }

        $('#grdCarta').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: content,
            status: status
        });
    });

    //Load all records when page is first shown
    $('#btnPesquisar').click();
});

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoOrigem") {
        $("#congregacao").show();
        $("#padrao").hide();
        $("#tipoCarta").hide();
    }
    else if (filtro === "TipoCarta_Status") {
        $("#tipoCarta").show();
        $("#congregacao").hide();
        $("#padrao").hide();
    }
    else {
        $("#padrao").show();
        $("#congregacao").hide();
        $("#tipoCarta").hide();
    }
}

function botao(id, templateId, status, aprovarCarta) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
    if (status !== "Cancelado") {
        botoes += '<span>&nbsp</span>' +
            '<a href="javascript:void(0)" id="lnkPrint" onclick="Imprimir(this);return false;" class="btn btn-warning btn-sm" data-templateid="' + templateId + '"' +
            '    data-targetmodel="Carta" data-id="' + id + '">' +
            '    <span class="glyphicon glyphicon-print" aria-hidden="true"></span> Imprimir' +
            '</a>';
    }
    if (aprovarCarta) {
        botoes += '<span>&nbsp</span>' +
            '<button type="button" class="btn btn-primary btn-sm" id="btnAprovar" ' +
            'onclick = "javascript:window.location.href=\'' + urlAprovar + "/" + id + '\'"; return false;> ' +
            '    <span class="glyphicon glyphicon-ok"></span> Aprovar' +
            '</button>';
    }
    return botoes;
}

function Imprimir(item) {
    var templateid = $(item).data("templateid");
    var targetmodel = $(item).data("targetmodel");
    var id = $(item).data("id");

    $.showYesNoAlert('Deseja imprimir esta Carta ?', "Imprimir Carta",
        function () {
            var url = urlPrint + "?templateId=" + templateid + "&targetModel=" + targetmodel + "&modelId=" + id;
            $.downloadArquivo(url, null);
        });
}
