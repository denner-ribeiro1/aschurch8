$(function () {
    "use strict";
    $("#HoraBatismo").mask("99:99");

    $("#tbPastor tbody tr").remove();
    for (var i = 0; i < pastorCeleb.length; i++) {
        AddNewLine(pastorCeleb[i].MembroId, pastorCeleb[i].Nome, pastorCeleb[i].IsReadOnly, i);
    }

    var paramsMembro = {
        TipoMembro: 3,
        Status: 1
    };
    $("#lnkBuscarPastorCelebrante").popUp("Pesquisar Pastor", "Membro", "MembroId", paramsMembro);

    $("#MembroId").blur(function () {
        var value = $(this).val();
        if (value > 0) {
            if (pastorCeleb.find(i => i.MembroId === value) != null) {
                $.alertError("Configuração de Batismo", "Pastor já adicionado!");
                $("#MembroId").val("");
                $("#Nome").val("");
                return false;
            }
            var requestData = {
                MembroId: value
            };
            $.ajaxPost("/Secretaria/Batismo/BuscarMembro", requestData, function (membro) {
                $("#Nome").val("");
                if (membro.Status === "OK") {
                    $("#Nome").val(membro.Nome);

                } else {
                    $("#MembroId").val("");
                    $.bsMessageError("Atenção", membro.Msg);
                }
            });
        }
    });

    var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
    if (usuarioCongregacaoSede === false) {
        // Usuário de outras congregações não podem fazer o agendamento do Batismo
        $("#DataPrevistaBatismo").attr("readonly", "readonly");
    } else {
        $("#DataPrevistaBatismo").removeAttr("readonly");
    }

    $("#DataBatismo").change(function () {
        var partesData = $("#DataBatismo").val().split("/");
        var dataB = new Date(partesData[2], partesData[1] - 1, partesData[0]);
        partesData = $("#DataMaximaCadastro").val().split("/");
        var dataMax = new Date(partesData[2], partesData[1] - 1, partesData[0])

        var erroDataValidade = $(document).find("[data-valmsg-for='DataBatismo']");
        erroDataValidade.html('');

        if (dataMax > dataB) {
            erroDataValidade.html('<span id="DataBatismo-error" class="">A Data do Batismo deve ser maior que a Data máxima para o cadastro de Batismo</span>');
            $("#DataBatismo").val("");
            return false;
        }
    });
    /************************************************************************************************************
    * PASTOR CELEBERANTE
    ************************************************************************************************************/
    $(document).on('click', '#btnAddPastor', {}, function (e) {
        e.preventDefault();

        var requestData = {
            MembroId: $("#MembroId").val(),
            Nome: $("#Nome").val(),
            IsReadOnly: false
        };

        var errorPastor = $(document).find("[data-valmsg-for='MembroId']");
        errorPastor.html('');

        var error = false;

        if (requestData.MembroId == null || requestData.MembroId == undefined || requestData.MembroId == 0) {
            error = true;
            errorPastor.html('<span id="Id-error" class="">Selecione um Pastor</span>');
        }

        if (error) {
            return false;
        }

        if (pastorCeleb.find(i => i.MembroId === requestData.MembroId) != null) {
            $.alertError("Configuração de Batismo", "Pastor já adicionado!");
            $("#MembroId").val("");
            $("#Nome").val("");
            return false;
        }
        AddNewLine(requestData.MembroId, requestData.Nome, false, pastorCeleb.length);
        pastorCeleb.push(requestData);

        $("#MembroId").val("");
        $("#Nome").val("");
    });

    $(document).on('click', '#btnSalvar', {}, function (e) {
        e.preventDefault;
        var partesData = $("#DataBatismo").val().split("/");
        var dataB = new Date(partesData[2], partesData[1] - 1, partesData[0]);
        partesData = $("#DataMaximaCadastro").val().split("/");
        var dataMax = new Date(partesData[2], partesData[1] - 1, partesData[0])

        var erroDataValidade = $(document).find("[data-valmsg-for='DataBatismo']");
        erroDataValidade.html('');

        if (dataMax > dataB) {
            erroDataValidade.html('<span id="DataBatismo-error" class="">A Data do Batismo deve ser maior que a data máxima para o cadastro de batismo</span>');
            $("#DataBatismo").val("");
            $.alertError("Batizado", "A Data do Batismo deve ser maior que a data máxima para o cadastro de batismo");
            return false;
        }

        var msg = "Deseja realmente criar uma nova Configuração do Batismo?";
        if ($("#Acao").val() === "Update") {
            msg = "Deseja realmente atualizar a Configuração do Batismo?";
        }
        $.showYesNoAlert(msg, "Configuração de Batismo", function () {
            $("#PastoresCelebrantesJSON").val(JSON.stringify(pastorCeleb));

            var form = $("#frmBatismo");
            var url = form.attr('action');

            if (form.valid()) {
                $.ajaxPostForm(url, $("#frmBatismo"), function (result) {
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

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("membroid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir esta Configuração de Batismo?</br>Após a confirmação todos os candidatos ao Batismo que estavam vinculados a esse batismo voltarão a ser Congregados.</h3>', "Configuração de Batismo",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Configuração de Batismo", response.mensagem);
                    }
                });
            });
    });

});

function AddNewLine(id, nome, isreadonly, indice) {
    var table = $("#tbPastor tbody")[0];
    if (table != null) {
        if (pastorCeleb.length == 0) {
            $("#tbPastor tbody tr").remove();
        }

        var row = table.insertRow(-1);
        row.className = "jtable-data-row" + (indice % 2 == 0 ? " jtable-row-even" : "");

        var cel1 = row.insertCell(0);
        cel1.width = "10%";
        cel1.innerHTML = id;

        var cel2 = row.insertCell(1);
        cel2.width = "70%";
        cel2.innerHTML = nome;

        if (isreadonly === false) {
            var botao = '<button type="button" class="btn btn-danger btn-sm" id="btnExcluir" ' +
                'onclick = "ExcluirPastor(' + id + ')"; return false;> ' +
                '    <span class="glyphicon glyphicon-list-alt"></span> Excluir' +
                '</button>';

            var cel3 = row.insertCell(2);
            cel3.width = "10%";
            cel3.innerHTML = botao;
        }
    }
}

function ExcluirPastor(id) {
    $.showYesNoAlert("Deseja excluir o Pastor Celebrante?", "Configuração de Batismo", function () {
        for (var i = 0; i < pastorCeleb.length; i++) {
            if (pastorCeleb[i].MembroId == id) {
                pastorCeleb.splice(i, 1);
            }
        }
        $("#tbPastor tbody tr").remove();

        if (pastorCeleb.length === 0) {
            var table = $("#tbPastor tbody")[0];
            var row = table.insertRow(-1);
            var cel1 = row.insertCell(0);
            cel1.colSpan = 3;
            cel1.width = "100%";
            cel1.innerHTML = "Não existem Pastores cadastrados para o Batismo";
        }
        else {
            for (var i = 0; i < pastorCeleb.length; i++) {
                AddNewLine(pastorCeleb[i].MembroId, pastorCeleb[i].Nome, pastorCeleb[i].IsReadOnly, i);
            }
        }
    });
}