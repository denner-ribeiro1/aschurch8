$(function () {
    $("#btnNovo").click((e) => {
        e.preventDefault();

        $.ajaxPost(urlBatismosEmAberto, null, function (data) {
            if (data.Status == "OK") {
                window.location = urlCreate
            }
            else {
                $.showYesNoAlert('<h3 style="color:red">' + data.Msg + '</h3>', "Configuração de Batismo",
                    function () {
                        window.location = urlCreate
                    });
            }
        });

        var request = { id: $(this).data("membroid") };

    });

    $('#grdBatismo').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'DataBatismo DESC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Configurações de Batismo a serem exibidas no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '10%',
                key: true,
                columnResizable: false,
            },
            DataBatismo: {
                title: 'Data do Batismo',
                width: '25%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    var d = data.record.DataBatismo.substring(0, 10).split("-");
                    return d[2] + "/" + d[1] + "/" + d[0];
                },
                listClass: "Centeralign"
            },
            DataMaximaCadastro: {
                title: 'Data Máxima para o Cadastro',
                width: '25%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    var d = data.record.DataMaximaCadastro.substring(0, 10).split("-");
                    return d[2] + "/" + d[1] + "/" + d[0];
                },
                listClass: "Centeralign"
            },
            Status: {
                title: 'Status',
                width: '20%',
                key: false,
                columnResizable: false,
                display: function (data) {
                    var descr = "";
                    if (data.record.Status == "1")
                        descr = "Em aberto";
                    else if (data.record.Status == "2")
                        descr = "Finalizado";
                    else if (data.record.Status == "3")
                        descr = "Cancelado";
                    return descr;
                }
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

    $('#grdBatismo').jtable('load');
});

function botao(id) {
    var botoes = '';
    botoes = '<button type="button" class="btn btn-info btn-sm" id="btnDetalhes" ' +
        'onclick = "javascript:window.location.href=\'' + urlDetalhes + "/" + id + '\'"; return false;> ' +
        '    <span class="glyphicon glyphicon-list-alt"></span> Detalhes' +
        '</button>';
    return botoes;
}