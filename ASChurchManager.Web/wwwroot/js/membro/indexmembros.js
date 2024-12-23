$(function () {

    //$('.box').boxWidget('toggle');

    var Status = $.getQuerystringValue("Status");
    if (Status !== "") {
        $('#Filtro').val("Status");
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
            noDataAvailable: 'Não existem Membros a serem exibidos no momento!'
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
                width: '18%',
                key: false,
                columnResizable: false
            },
            Cpf: {
                title: 'CPF',
                width: '9%',
                key: false,
                columnResizable: false
            },
            NomeMae: {
                title: 'Nome da Mãe',
                width: '18%',
                key: false,
                columnResizable: false
            },
            Congregacao: {
                title: 'Congregação',
                width: '18%',
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
                width: '30%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id, data.record.Status, data.record.Sede, data.record.PermiteAprovarMembro, data.record.PermiteImprimirCarteirinha);
                }
            }
        }
    });

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "Status")
            content = $('#StatusMembroSelecionado').val();
        else if ($('#Filtro').val() === "CongregacaoId")
            content = $('#CongregacaoSelecionado').val();
        else if ($('#Filtro').val() === "Cpf")
            content = $('#Cpf').val();

        $('#grdMembro').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: content
        });
    });

    //Load all records when page is first shown
    $('#btnPesquisar').click();
});

function botao(id, status, sede, aprovar, imprimir) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button> <span>&nbsp</span>';

    if (status === "Pendente Aprovação" && sede && aprovar) {
        botoes += '<button type="button" class="btn btn-primary btn-sm" id="btnAprovar" ' +
            'onclick = "javascript:window.location.href=\'' + urlAprovar + "/" + id + '\'"; return false;> ' +
            '    <span class="glyphicon glyphicon-wrench"></span> Aprovar/Reprovar' +
            '</button> <span>&nbsp</span>';
    }
    else if (status === "Ativo" && imprimir) {
        botoes += '<button type="button" data-print="' + id + '" class="btn btn-warning btn-sm" id="btnImprimir" ' +
            'onclick = "GerarCarteirinha(' + id + '); return false;"> ' +
            '    <span class="glyphicon glyphicon-user"></span> Carteirinha' +
            '</button>';
    }
    return botoes;
}

function GerarCarteirinha(id) {
    var requestData = {
        Id: id
    };
    $.ajaxPost("/Secretaria/Membro/VerificaVencimentoCarteirinha", requestData, function (data) {
        if (data.DataValidade != "" && data.DataValidade != null) {
            $.showYesNoAlert("A Carteirinha do Membro vence em " + data.DataValidade + ". Deseja gerar um novo vencimento para ela?", "Carteirinha Membro", function () {
                var url = urlPrint + "?id=" + id.toString() + "&atualizaValidade=true";
                $.downloadArquivo(url, null);
            }, null, function () {
                var url = urlPrint + "?id=" + id.toString() + "&atualizaValidade=false";
                $.downloadArquivo(url, null);
            }, null);
        }
        else {
            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);
            var url = urlPrint + "?id=" + id.toString() + "&atualizaValidade=false";
            $.downloadArquivo(url, null);
        }
    });
}

function HabilitarCampos(filtro) {
    if (filtro === "Status") {
        $("#StatusMembroSelecionado").show();
        $("#CongregacaoSelecionado").hide();
        $("#Conteudo").hide();
        $("#Cpf").hide();
    }
    else if (filtro === "CongregacaoId") {
        $("#CongregacaoSelecionado").show();
        $("#StatusMembroSelecionado").hide();
        $("#Conteudo").hide();
        $("#Cpf").hide();
    }
    else if (filtro === "Cpf") {
        $("#Cpf").show();
        $("#CongregacaoSelecionado").hide();
        $("#StatusMembroSelecionado").hide();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#StatusMembroSelecionado").hide();
        $("#CongregacaoSelecionado").hide();
        $("#Cpf").hide();
    }
}