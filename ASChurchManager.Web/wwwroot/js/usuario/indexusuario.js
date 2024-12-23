$(function () {
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

    $('#grdUsuario').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Id ASC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Usuários a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Id',
                width: '5%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Nome',
                width: '20%',
                key: false,
                columnResizable: false
            },
            Username: {
                title: 'Usuário',
                width: '10%',
                key: false,
                columnResizable: false
            },
            Email: {
                title: 'E-mail',
                width: '20%',
                key: false,
                columnResizable: false
            },
            Congregacao: {
                title: 'Congregação',
                width: '14%',
                key: false,
                columnResizable: false
            },
            Botoes: {
                title: '',
                width: '20%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                }
            }
        }
    });
    $('#grdUsuario').jtable('load');

    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "CongregacaoId")
            content = $('#CongregacaoSelecionado').val();
        
        $('#grdUsuario').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: content
        });
    });
})

function botao(id) {
    var botoes = '';

    botoes = '<button type="button" class="btn btn-warning btn-sm" id="btnAlterar" ' +
        'onclick = "javascript:window.location.href=\'' + urlAlterarSenha + "?id=" + id + '&exibirVoltar=true\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-retweet"></span> Alterar Senha' +
        '</button> <span>&nbsp</span>';

    botoes += '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';

    return botoes;
}

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoId") {
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#CongregacaoSelecionado").hide();
    }
}