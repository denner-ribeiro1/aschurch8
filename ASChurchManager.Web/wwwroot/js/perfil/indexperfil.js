$(function () {
    $('#grdPerfil').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        pageSizeChangeArea: false,
        defaultSorting: 'Id DESC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Perfil a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '10%',
                key: true,
                columnResizable: false,
            },
            Nome: {
                title: 'Descrição Perfil',
                width: '70%',
                key: false,
                columnResizable: false
            },
            Botoes: {
                title: '',
                width: '10%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id);
                }
            }
        }
    });
    $('#grdPerfil').jtable('load');
})

function botao(id) {
    return '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
}