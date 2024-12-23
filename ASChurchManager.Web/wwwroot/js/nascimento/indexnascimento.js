$(function () {

    //$('.box').boxWidget('toggle');

    $('#grdNasc').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'DataApresentacao DESC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Nascimentos a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '10%',
                key: true,
                columnResizable: false,
                visibility: 'hidden'
            },

            DataApresentacao: {
                title: 'Data de Apresentação',
                width: '15%',
                columnResizable: false,
                display: function (data) {
                    var d = data.record.DataApresentacao.substring(0, 10).split("-");
                    return d[2] + "/" + d[1] + "/" + d[0];
                },
                listClass: "Centeralign"
            },
            Crianca: {
                title: 'Nome da Criança',
                width: '25%',
                key: false,
                columnResizable: false
            },
            NomePai: {
                title: 'Nome do Pai',
                width: '25%',
                key: false,
                columnResizable: false
            },
            NomeMae: {
                title: 'Nome da Mãe',
                width: '25%',
                key: false,
                columnResizable: false
            },
            Sexo: {
                title: 'Sexo',
                width: '10%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    return data.record.Sexo === 2 ? "Feminino" : "Masculino";
                }
            },
            Botoes: {
                title: '',
                width: '10%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                    //'<button type="button" class="btn btn-primary btn-sm" onclick="AbrirDetalhes(' + data.record.Id + ')">Detalhes</button> ';
                }
            }
        }
    });

    //Re-load records when user click 'load records' button.
    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        $('#grdNasc').jtable('load', {
            filtro: $('#Filtro').val(),
            conteudo: $('#Conteudo').val()
        });
    });

    //Load all records when page is first shown
    $('#btnPesquisar').click();
});

function AbrirDetalhes(id) {
    window.location.href = urlDetalhes + "/" + id;
}

function botao(id) {
    return '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" onclick="AbrirDetalhes(' + id + ')">' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
}