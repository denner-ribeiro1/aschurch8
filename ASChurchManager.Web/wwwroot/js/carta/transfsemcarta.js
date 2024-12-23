var membros = [];
$(function () {
    "use strict";

    // ----------------------------------------- Membro
    var paramsMembro = {
        TipoMembro: 3,
        Status: 1
    };
    $("#linkIdMembro").popUp("", "Membro", "MembroId", paramsMembro);

    $("#MembroId").blur(function () {
        var value = parseInt($("#MembroId").val());
        if (value > 0) {
            var requestData = {
                MembroId: value
            };

            $.ajaxPost(urlBuscarMembro, requestData, function (dados) {
                if (dados.status === "ERRO") {
                    $.bsMessageError("Atenção", dados.msg);
                    $("#MembroId").val("");
                    $("#Nome").val("");
                    $("#CongregacaoOrigemId").val("");
                    $("#CongregacaoOrigem").val("");
                }
                else {
                    $("#Nome").val(dados.membro.Nome);
                    $("#CongregacaoOrigemId").val(dados.membro.Congregacao.Id);
                    $("#CongregacaoOrigem").val(dados.membro.Congregacao.Nome);
                }
            });
        }
    });

    // ----------------------------------------- Congregação Destino
    $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoDestId");

    $("#CongregacaoDestId").blur(function () {
        var errorCongr = $(document).find("[data-valmsg-for='CongregacaoDestId']");
        errorCongr.html('');

        var congrDest = $(this).val();
        if (congrDest > 0) {
            var requestData = {
                CongregacaoId: congrDest
            };

            $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacaoSemFiltroUsuario", requestData, function (congregacao) {
                $("#CongregacaoDest").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código de Congregação inválido!");
                    $(this).val("");
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoDest").val(congregacao.Nome);
                }
            });
        }
    });


    $(document).on('click', '#btnAdicionar', {}, function (e) {
        e.preventDefault();

        var requestData = {
            MembroId: $("#MembroId").val(),
            Nome: $("#Nome").val(),
            CongregacaoOrigem: $("#CongregacaoOrigem").val(),
        };

        var erroMembro = $(document).find("[data-valmsg-for='MembroId']");
        erroMembro.html('');

        var error = false;
        if (requestData.MembroId == null || requestData.MembroId == undefined || requestData.MembroId == 0) {
            error = true;
            erroMembro.html('<span id="MembroId-error" class="">O campo Id do Membro é obrigatório</span>');
        }

        if (membros.find(i => i.MembroId === requestData.MembroId) != null) {
            $.alertError("Transfência sem Carta", "Membro já adicionado!");
            $("#MembroId").val("");
            $("#Nome").val("");
            $("#CongregacaoOrigemId").val("");
            $("#CongregacaoOrigem").val("");
            return false;
        }

        if (error)
            return false;


        if ($("#Nome").val() != null && $("#Nome").val() != "" && $("#Nome").val() != undefined) {
            AddNewLine(requestData.MembroId, requestData.Nome, requestData.CongregacaoOrigem, membros.length);
            membros.push(requestData);
            $("#MembroId").val("");
            $("#Nome").val("");
            $("#CongregacaoOrigemId").val("");
            $("#CongregacaoOrigem").val("");
        }
    });

    $("#btnTransferir").click(function (event) {
        event.preventDefault();
        var erroCongDest = $(document).find("[data-valmsg-for='CongregacaoDestId']");
        erroCongDest.html('');

        var requestData = {
            congregacaoDestinoId: $("#CongregacaoDestId").val()
        };
        var error = false;
        if (requestData.congregacaoDestinoId == null || requestData.congregacaoDestinoId == undefined || requestData.congregacaoDestinoId == 0) {
            error = true;
            erroCongDest.html('<span id="CongregacaoDestId-error" class="">O campo Congregação Destino é obrigatório</span>');
        }

        if (membros.length === 0) {
            $.alertError("Transfência sem Carta", "Não há membros adicionados para Transferência.");
            return false;
        }
        if (error) {
            return false;
        }

        $.showYesNoAlert("Deseja realmente transferir os membros acima?", "Transferência sem carta",
            function () {
                $("#MembrosJSON").val(JSON.stringify(membros));

                var form = $("#frmTransfCarta");
                var url = form.attr('action');

                if (form.valid()) {
                    $.ajaxPostForm(url, $("#frmTransfCarta"), function (result) {
                        if (result.status === "OK") {
                            window.location = result.url;
                        }
                        else {
                            $.alertError("Configuração de Batismo", result.msg);
                        }
                    });
                }
            });
    });
});

function AddNewLine(id, nome, congregacao, indice) {
    var table = $("#tbMembro tbody")[0];
    if (table != null) {
        if (membros.length == 0) {
            $("#tbMembro tbody tr").remove();
        }

        var row = table.insertRow(-1);
        row.className = "jtable-data-row" + (indice % 2 == 0 ? " jtable-row-even" : "");

        var cel1 = row.insertCell(0);
        cel1.width = "10%";
        cel1.innerHTML = id;

        var cel2 = row.insertCell(1);
        cel2.width = "35%";
        cel2.innerHTML = nome;

        var cel3 = row.insertCell(2);
        cel3.width = "30%";
        cel3.innerHTML = congregacao;

        var botao = '<button type="button" class="btn btn-danger btn-sm" id="btnExcluir" ' +
            'onclick = "ExcluirMembro(' + id + ')"; return false;> ' +
            '    <span class="glyphicon glyphicon-list-alt"></span> Excluir' +
            '</button>';
        var cel4 = row.insertCell(3);
        cel4.width = "10%";
        cel4.innerHTML = botao;
    }
}

function ExcluirMembro(id) {
    $.showYesNoAlert("Deseja excluir o Membro?", "Transfência sem carta", function () {
        for (var i = 0; i < membros.length; i++) {
            if (membros[i].MembroId == id) {
                membros.splice(i, 1);
            }
        }
        $("#tbMembro tbody tr").remove();

        if (membros.length === 0) {
            var table = $("#tbMembro tbody")[0];
            var row = table.insertRow(-1);
            var cel1 = row.insertCell(0);
            cel1.colSpan = 4;
            cel1.width = "100%";
            cel1.innerHTML = "Não existem Membros selecionados para a Transferência";
        }
        else {
            for (var i = 0; i < membros.length; i++) {
                AddNewLine(membros[i].MembroId, membros[i].Nome, membros[i].CongregacaoOrigem, i);
            }
        }
    });
}