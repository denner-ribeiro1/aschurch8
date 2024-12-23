$(() => {
    "use strict";

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

    $('#grdPresenca').jtable({
        paging: true, //Enable paging
        pageSize: 15, //Set page size (default: 10)
        sorting: true, //Enable sorting
        pageSizeChangeArea: false,
        defaultSorting: 'Status ASC',
        actions: {
            listAction: urlList
        },
        messages: {
            noDataAvailable: 'Não existem Eventos a serem exibidos no momento!'
        },
        fields: {
            Id: {
                title: 'Código',
                width: '6%',
                key: true,
                columnResizable: false,
            },
            Descricao: {
                title: 'Descricao',
                width: '25%',
                key: false,
                columnResizable: false
            },
            Congregacao: {
                title: 'Congregacao',
                width: '25%',
                key: false,
                columnResizable: false
            },
            TipoEvento: {
                title: 'Tipo Evento',
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
                width: '10%',
                sorting: false,
                display: function (data) {
                    return botao(data.record.Id, data.record.Status);
                }
            }
        }
    });

    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        var content = $('#Conteudo').val();
        if ($('#Filtro').val() === "Status")
            content = $('#StatusPresencaSelecionado').val();
        else if ($('#Filtro').val() === "CongregacaoId")
            content = $('#CongregacaoSelecionado').val();
        else if ($('#Filtro').val() === "TipoEventoId")
            content = $('#TipoEventoSelecionado').val();

        $('#grdPresenca').jtable('load', {
            campo: $('#Filtro').val(),
            valor: content
        });
    });

    $('#btnPesquisar').click();
});

function botao(id, status) {
    var botoes = '';
    if (status !== "Finalizado") {
        botoes = `<button type="button" class="btn btn-primary btn-sm" 
                        onclick = "javascript:window.location.href=\'${urlDetalhes}?idPresenca=${id}\'"; return false;>
                      <span class="glyphicon glyphicon-list-alt"></span> Registrar
                  </button>`;
    }
    else {
        botoes = `<button type="button" class="btn btn-info btn-sm" 
                        onclick = "javascript:window.location.href=\'${urlDetalhes}?idPresenca=${id}\'"; return false;> 
                      <span class="glyphicon glyphicon-list-alt"></span> Editar
                  </button>`;
    }
    return botoes;
}

function HabilitarCampos(filtro) {
    if (filtro === "Status") {
        $("#StatusPresencaSelecionado").show();
        $("#TipoEventoSelecionado").hide();
        $("#CongregacaoSelecionado").hide();
        $("#Conteudo").hide();
    }
    else if (filtro === "CongregacaoId") {
        $("#CongregacaoSelecionado").show();
        $("#TipoEventoSelecionado").hide();
        $("#StatusPresencaSelecionado").hide();
        $("#Conteudo").hide();
    }
    else if (filtro === "TipoEventoId") {
        $("#TipoEventoSelecionado").show();
        $("#CongregacaoSelecionado").hide();
        $("#StatusPresencaSelecionado").hide();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#TipoEventoSelecionado").hide();
        $("#CongregacaoSelecionado").hide();
        $("#StatusPresencaSelecionado").hide();
    }
}