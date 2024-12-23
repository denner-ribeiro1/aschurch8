let isReadOnly = false;

$(() => {
    "use strict";

    isReadOnly = $("#IsReadOnly").val().toLowerCase() == "true";

    $("#divFiltro").hide();

    $("#btnAddLista").click((e) => {
        e.preventDefault;

        let form = $("#frmInscricao");
        if (form.valid()) {
            let requestData = {
                idPresenca: $("#Presenca").val(),
                nome: $("#Nome").val(),
                cpf: $("#Cpf").val(),
                igreja: $("#Igreja").val(),
                cargo: $("#Cargo").val(),
                pago: $("#Pago").prop('checked')
            }
            $.ajaxPost(urlIncluir, requestData, function (ret) {
                if (ret.status == "OK") {
                    let membros = ret.data;
                    CarregarTable(membros)
                    LimparCampos();
                }
                else {
                    $.bsMessageError("Atenção", ret.Message);
                }
            });
        }
        else {
            var msg = ValidarFormulario();
            if (msg != null && msg != undefined && msg != "")
                $.alertError("Inscrições", msg);
        }

    })

    $("#btnCancLista").click((e) => {
        e.preventDefault();
        LimparCampos();
        $("#Nome").focus();
    });

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

    $('#btnPesquisar').click(function (e) {
        e.preventDefault();
        if ($("#Presenca").val() != null &&
            $("#Presenca").val() != "") {
            var content = $('#Conteudo').val();
            if ($('#Filtro').val() === "CongregacaoId")
                content = $('#CongregacaoSelecionado').val();
            else if ($('#Filtro').val() === "Cpf")
                content = $('#Cpf').val();

            let requestData = {
                idPresenca: $("#Presenca").val(),
                filtro: $('#Filtro').val(),
                naoMembro: $('#NaoMembro').val(),
                conteudo: content
            }

            $.ajaxPost(urlBuscarLista, requestData, function (ret) {
                if (ret.status == "OK") {
                    let membros = ret.data;
                    CarregarTable(membros)
                }
                else {
                    $.bsMessageError("Atenção", ret.Message);
                }
            });
        }
    });

    $('#btnPesquisar').click();
});

function CarregarMembro() {
    LimparCampos();

    let requestData = {
        idPresenca: $("#Presenca").val(),
        naoMembro: true
    }

    $.ajaxPost(urlBuscarLista, requestData, function (ret) {
        if (ret.status == "OK") {
            let membros = ret.data;
            CarregarTable(membros)
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });
}

function CarregarTable(membros) {
    let table = document.getElementById("tbMembro").getElementsByTagName("tbody")[0];
    $(table).empty();

    if (membros.length > 0) {
        for (var i = 0; i < membros.length; i++) {
            IncluirLinhaTabela(membros[i], i);
        }
    }
    else {
        let row = table.insertRow(0);
        row.className = "jtable-data-row";
        $.incluirCelulasTabela(row, 0, "100%", "Não há inscrições cadastradas para o Curso/Eventos", 6);
    }
}

function IncluirLinhaTabela(membro, i) {
    let valor = $("#Valor").val();
    let table = document.getElementById("tbMembro").getElementsByTagName("tbody")[0];
    let row = table.insertRow(i);
    row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
    $.incluirCelulasTabela(row, 0, null, membro.Id);
    $.incluirCelulasTabela(row, 1, null, membro.Nome);
    $.incluirCelulasTabela(row, 2, null, membro.CPF);
    $.incluirCelulasTabela(row, 3, null, membro.Igreja);
    $.incluirCelulasTabela(row, 4, null, membro.Cargo);
    if (!isReadOnly) {
        let botoes = '';
        if (valor > 0)
            botoes += `<input type="button" value="${membro.Pago ? "Pago" : "Não Pago"}" class="btn ${membro.Pago ? "btn-primary" : "btn-secondary"} btn-sm" style=";width:90px;" onclick="AtualizarPago(${membro.Id}, ${membro.Pago}, this);">&nbsp;&nbsp;`;
        botoes += `<input type="button" value="Excluir" class="btn btn-danger btn-sm" style="width:90px" onclick="ExcluirRow(${membro.Id}, this);">`;
        $.incluirCelulasTabela(row, 5, null, botoes);
    }

}

function LimparCampos() {
    $("#Nome").val("");
    $("#Cpf").val("");
    $("#Igreja").val("");
    $("#Cargo").val("");
    $("#Pago").prop("checked", false);
}

function ExcluirRow(value, botao) {
    let msg = "Deseja excluir o Membro da Lista?";
    if ($("#PermiteInscricoes").val() === "False") {
        msg = "Inscrições já encerradas, após a exclusão não será possivel reinscrever o Membro no Curso/Evento.<br />Deseja realmente excluir o Membro da Lista?";
    }
    $.showYesNoAlert(msg, "Excluir Inscrição", function () {
        let requestData = {
            id: value
        }
        $.ajaxPost(urlExcluir, requestData, function (ret) {
            if (ret.status == "OK") {
                var i = botao.parentNode.parentNode.sectionRowIndex;
                var table = document.getElementById("tbMembro").getElementsByTagName("tbody")[0];
                table.deleteRow(i);

                let qtdLinhas = $("#tbMembro tr").length - 1;
                if (qtdLinhas <= 0) {
                    let row = table.insertRow(0);
                    row.className = "jtable-data-row";
                    $.incluirCelulasTabela(row, 0, "100%", "Não há inscrições cadastrados para o Curso/Eventos", 6);
                }
            }
            else {
                $.bsMessageError("Atenção", ret.Message);
            }
        });
    });
}

function AtualizarPago(id, pago, botao) {
    let requestData = {
        id: id,
        pago: !pago
    }
    $.ajaxPost(urlAtualizarPago, requestData, function (ret) {
        if (ret.status == "OK") {
            if (pago === true) {
                $(botao).val("Não Pago")
                $(botao).removeClass('btn-primary');
                $(botao).addClass('btn-secondary');
            }
            else {
                $(botao).val("Pago")
                $(botao).removeClass('btn-secondary');
                $(botao).addClass('btn-primary');
            }
            $(botao).attr('onclick', `AtualizarPago(${id}, ${!pago}, this)`);
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });

}

function HabilitarCampos(filtro) {
    if (filtro === "Cpf") {
        $("#CpfFiltro").show();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#CpfFiltro").hide();
    }
}

function HabilitarCampos(filtro) {
    if (filtro === "CongregacaoId") {
        $("#CongregacaoSelecionado").show();
        $("#Conteudo").hide();
        $("#CpfFiltro").hide();
    }
    else if (filtro === "Cpf") {
        $("#CpfFiltro").show();
        $("#CongregacaoSelecionado").hide();
        $("#StatusMembroSelecionado").hide();
        $("#Conteudo").hide();
    }
    else {
        $("#Conteudo").show();
        $("#StatusMembroSelecionado").hide();
        $("#CongregacaoSelecionado").hide();
        $("#CpfFiltro").hide();
    }
}

function ValidarFormulario() {
    var erro = "";
    $('span[id*=-error]').each(function (x, item) {
        erro += $(item).text() + "<br>";
    });
    return erro;
}