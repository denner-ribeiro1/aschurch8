$(function () {
    $('#Filtro').change((e) => {
        e.preventDefault();

        $("#Conteudo").val("");
    });

    $('#grdCongr').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Id ASC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Congregações a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '8%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Nome',
                width: '30%',
                key: false,
                columnResizable: false
            },
            CongregacaoResponsavelNome: {
                title: 'Congregação Responsável',
                width: '30%',
                key: false,
                columnResizable: false
            },
            PastorResponsavelNome: {
                title: 'Pastor Responsável',
                width: '30%',
                key: false,
                columnResizable: false
            },
            Botoes: {
                title: '',
                width: '15%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                }
            }
        }
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


    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        $('#grdCongr').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: $('#Conteudo').val()
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