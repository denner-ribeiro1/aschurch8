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
            noDataAvailable: 'Não existem Candidatos ao Batismo para serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '7%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Nome',
                width: '20%',
                key: false,
                columnResizable: false
            },
            Cpf: {
                title: 'CPF',
                width: '10%',
                key: false,
                columnResizable: false
            },
            NomeMae: {
                title: 'Nome da Mãe',
                width: '20%',
                key: false,
                columnResizable: false
            },
            CongregacaoNome: {
                title: 'Congregação',
                width: '25%',
                key: false,
                columnResizable: false
            },
            DataBatismo: {
                title: 'Data do Batismo',
                width: '25%',
                key: false,
                columnResizable: false
            },
            Botoes: {
                title: '',
                width: '40%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                }
            }
        }
    });

    $("#btnNovo").click((e) => {
        e.preventDefault();
        this.location.href = urlNovo;
    })

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "CongregacaoId")
            content = $('#CongregacaoSelecionado').val();
        else if ($('#Filtro').val() === "DataBatismo")
            content = $('#DataBatismoSelecionado').val();
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

function botao(id) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
    return botoes;
}

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoId") {
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
        $("#Cpf").hide();
        $("#DataBatismoSelecionado").hide();
    }
    else if (filtro === "Cpf") {
        $("#Cpf").show();
        $("#CongregacaoSelecionado").hide();
        $("#Conteudo").hide();
        $("#DataBatismoSelecionado").hide();
    }
    else if (filtro === "DataBatismo") {
        $("#DataBatismoSelecionado").show();
        $("#Conteudo").hide();
        $("#CongregacaoSelecionado").hide();
        $("#Cpf").hide();
    }
    else {
        $("#Conteudo").show();
        $("#CongregacaoSelecionado").hide();
        $("#Cpf").hide();
        $("#DataBatismoSelecionado").hide();
    }
}