$(function () {
    "use strict";
    if (actionName === "Details") {
        HabilitaDesabilitaCampos(false, "lnkPastorId", "PastorId", "Pastor");
        HabilitaDesabilitaCampos(false, "lnkIdMae", "IdMembroMae", "NomeMae");
        HabilitaDesabilitaCampos(false, "lnkIdPai", "IdMembroPai", "NomePai");
    }
    else {
        // ----------------------------------------- Congregação
        var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
        $("#lnkBuscarCongregacao").removeAttr("class");

        if (usuarioCongregacaoSede === false) {
            $("#CongregacaoId").attr("readonly", "readonly");
            $("#lnkBuscarCongregacao").attr("class", "readonly");
        }
        else {
            $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
        }

        $("#PastorId").removeAttr("readonly");
        $("#Pastor").removeAttr("readonly");
        $("label[for=PastorId]").hide();
        $("#PastorId").addClass("alinhar");
        $("#lnkPastorId").show();

        if ($("#PastorMembro").prop('checked')) {
            $("#PastorId").attr("readonly", "readonly");
            $("#Pastor").attr("readonly", "readonly");
            $("label[for=PastorId]").show();
            $("#PastorId").removeClass("alinhar");
            $("#lnkPastorId").hide();
        }

        HabilitaDesabilitaCampos($("#MaeMembro").prop('checked'), "lnkIdMae", "IdMembroMae", "NomeMae");
        HabilitaDesabilitaCampos($("#PaiMembro").prop('checked'), "lnkIdPai", "IdMembroPai", "NomePai");

        $("#CongregacaoId").change(function () {
            var value = $(this).val();
            if (value > 0) {

                var requestData = {
                    CongregacaoId: value
                };

                $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (congregacao) {
                    $("#CongregacaoNome").val("");
                    if (congregacao.Id == 0) {
                        $.bsMessageError("Atenção", "Código inválido!");
                        $(this).val('');
                    }
                    else if (congregacao.Id > 0) {
                        $("#CongregacaoNome").val(congregacao.Nome);
                        $("#PastorId").val(congregacao.PastorResponsavelId);
                        $("#Pastor").val(congregacao.PastorResponsavelNome);
                        $("#PastorId").attr("readonly", "readonly");
                        $("#Pastor").attr("readonly", "readonly");
                    }
                });
            }
        });

        // ---------------------------------------- Pastor
        var paramsMembro = {
            TipoMembro: 3,
            Status: 1
        };
        $("#lnkPastorId").popUp("Pesquisar Pastor", "Membro", "PastorId", paramsMembro);

        $("#PastorId").change(function () {
            if (!$('#PastorId').is('[readonly]')) {
                var value = $(this).val();
                if (value !== "" || value === null || value === undefined) {
                    buscarMembro(value, "Pastor");
                    if ($("#Pastor").val() !== '' || $("#Pastor").val() !== null || $("#Pastor").val() !== undefined) {
                        $("#PastorId").attr("readonly", "readonly");
                    }
                }
            }
        });

        $("#Pastor").change(function () {
            if ($(this).val() === "" || $(this).val() === null || $(this).val() === undefined) {
                $("#PastorId").removeAttr("readonly");
            }
        });

        $("#PastorMembro").change(function () {
            if ($("#PastorMembro").prop('checked') === false) {
                $("#PastorId").removeAttr("readonly");
                $("#Pastor").removeAttr("readonly");
                $("label[for=PastorId]").hide();
                $("#lnkPastorId").show();
                $("#PastorId").addClass("alinhar");
                $("#PastorId").val("");
                $("#Pastor").val("");
            }
            else {
                $("#PastorId").attr("readonly", "readonly");
                $("#Pastor").attr("readonly", "readonly");
                $("label[for=PastorId]").show();
                $("#PastorId").removeClass("alinhar");
                $("#lnkPastorId").hide();

                var value = $("#CongregacaoId").val();
                if (value > 0) {

                    var requestData = {
                        CongregacaoId: value
                    };

                    $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (congregacao) {
                        $("#CongregacaoNome").val("");
                        if (congregacao.Id === 0) {
                            $.bsMessageError("Atenção", "Código inválido!");
                            $(this).val('');
                        }
                        else if (congregacao.Id > 0) {
                            $("#CongregacaoNome").val(congregacao.Nome);
                            $("#PastorId").val(congregacao.PastorResponsavelId);
                            $("#Pastor").val(congregacao.PastorResponsavelNome);
                            $("#PastorId").attr("readonly", "readonly");
                            $("#Pastor").attr("readonly", "readonly");
                        }
                    });
                }
            }

        });

        // ----------------------------------------- Pai
        $("#lnkIdPai").popUp("Pesquisar - Pai da Criança", "Membro", "IdMembroPai", paramsMembro);

        $("#IdMembroPai").change(function () {
            var value = $(this).val();
            buscarMembro(value, "NomePai");
        });

        $("#PaiMembro").change(function () {
            HabilitaDesabilitaCampos($("#PaiMembro").prop('checked'), "lnkIdPai", "IdMembroPai", "NomePai");
            if ($("#PaiMembro").prop('checked')) {
                $("#NomePai").val("");
            }
            else {
                $("#IdMembroPai").val("");
            }
        });


        // ----------------------------------------- Mãe
        $("#lnkIdMae").popUp("Pesquisar - Mãe da Criança", "Membro", "IdMembroMae", paramsMembro);

        $("#IdMembroMae").change(function () {
            var value = $(this).val();
            buscarMembro(value, "NomeMae");
        });

        $("#MaeMembro").change(function () {
            HabilitaDesabilitaCampos($("#MaeMembro").prop('checked'), "lnkIdMae", "IdMembroMae", "NomeMae");
            if ($("#MaeMembro").prop('checked')) {
                $("#NomeMae").val("");
            }
            else {
                $("#IdMembroMae").val("");
            }
        });

        $("#btnSalvar").click(function (e) {
            e.preventDefault;
            var msg = "Deseja realmente criar um novo Nascimento?";
            if (actionName === "Edit") {
                msg = "Deseja realmente atualizar o Nascimento?";
            }
            $.showYesNoAlert(msg, "Nascimento", function () {
                var form = $("#frmNascimento");
                var url = form.attr('action');

                if (form.valid()) {
                    $.ajaxPostForm(url, $("#frmNascimento"), function (result) {
                        if (result.status === "OK") {
                            window.location = result.url;
                        }
                        else {
                            $.alertError("Nascimento", result.msg);
                        }
                    });
                }
            });
        });
    }

});

function excluirNascimento() {
    var request = { id: $("#lnkExcluir").data("nascimentoid") };
    $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Nascimento?</br>Após a confirmação NÃO será mais possível recuperar o Nascimento.</h3>', "Nascimento",
        function () {
            $.ajaxPost(urlExcluir, request, function (response) {
                if (response.status === "OK") {
                    window.location = response.url;
                }
                else {
                    $.alertError("Nascimento", response.mensagem);
                }
            });
        });
};

function buscarMembro(requestId, elementTargetId) {
    if (requestId > 0) {

        var requestData = {
            MembroId: requestId
        };
        elementTargetId = "#" + elementTargetId;

        $.ajaxPost("/Secretaria/Membro/BuscarMembro", requestData, function (membro) {
            $(elementTargetId).val("");
            if (membro.Id === 0) {
                $.bsMessageError("Atenção", "Membro não cadastrado!");
                $(this).val("");
            } else if (membro.Id > 0) {
                $(elementTargetId).val(membro.Nome);
            }
        });
    }
}

function HabilitaDesabilitaCampos(opcao, linkModal, campoId, campoNome) {
    if (opcao) {
        $("#" + linkModal).show();
        $("label[for=" + campoId + "]").hide();

        $("#" + campoId).removeAttr("readonly");
        $("#" + campoId).addClass("alinhar");
        $("#" + campoId).focus();

        $("#" + campoNome).attr("readonly", "readonly");
    }
    else {
        $("#" + linkModal).hide();
        $("label[for=" + campoId + "]").show();

        $("#" + campoId).attr("readonly", "readonly");
        $("#" + campoId).removeClass("alinhar");

        $("#" + campoNome).removeAttr("readonly");
        $("#" + campoNome).focus();
    }
}