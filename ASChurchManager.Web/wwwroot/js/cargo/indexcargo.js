$(function () {
    $('#grdCargo').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        pageSizeChangeArea: false,
        defaultSorting: 'Id DESC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Cargos a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Id',
                width: '6%',
                key: true,
                columnResizable: false,
            },
            Descricao: {
                title: 'Descrição Cargo',
                width: '40%',
                key: false,
                columnResizable: false
            },
            Lider: {
                title: 'Líder',
                width: '30%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    return data.record.Lider === true ? "Sim" : "Não";
                }
            },
            Obreiro: {
                title: 'Obreiro',
                width: '30%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    return data.record.Obreiro === true ? "Sim" : "Não";
                }
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
    $('#grdCargo').jtable('load');
})

function botao(id) {
    return '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
}