$(function () {
    "use strict";
    $("#HoraInicio").mask("99:99");
    $("#HoraFim").mask("99:99");

    if (actionName === "Details") {
        HabilitaDesabilitaCampos(false, "lnkPastorId", "PastorId", "PastorNome");
        HabilitaDesabilitaCampos(false, "lnkNoivoId", "NoivoId", "NoivoNome");
        HabilitaDesabilitaCampos(false, "lnkPaiNoivoId", "PaiNoivoId", "PaiNoivoNome");
        HabilitaDesabilitaCampos(false, "lnkMaeNoivoId", "MaeNoivoId", "MaeNoivoNome");
        HabilitaDesabilitaCampos(false, "lnkNoivaId", "NoivaId", "NoivaNome");
        HabilitaDesabilitaCampos(false, "lnkPaiNoivaId", "PaiNoivaId", "PaiNoivaNome");
        HabilitaDesabilitaCampos(false, "lnkMaeNoivaId", "MaeNoivaId", "MaeNoivaNome");
    }
    else {
        var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
        // ----------------------------------------- Congregação
        $("#lnkBuscarCongregacao").removeAttr("class");

        if (usuarioCongregacaoSede === false) {
            $("#CongregacaoId").attr("readonly", "readonly");
            $("#lnkBuscarCongregacao").attr("class", "readonly");
        }
        else {
            $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
        }

        $("#PastorId").removeAttr("readonly");
        $("#PastorNome").removeAttr("readonly");
        $("label[for=PastorId]").hide();
        $("#PastorId").addClass("alinhar");
        $("#lnkPastorId").show();

        if ($("#PastorMembro").prop('checked')) {
            $("#PastorId").attr("readonly", "readonly");
            $("#PastorNome").attr("readonly", "readonly");
            $("label[for=PastorId]").show();
            $("#PastorId").removeClass("alinhar");
            $("#lnkPastorId").hide();
        }

        HabilitaDesabilitaCampos($("#NoivoMembro").prop('checked'), "lnkNoivoId", "NoivoId", "NoivoNome");
        HabilitaDesabilitaCampos($("#PaiNoivoMembro").prop('checked'), "lnkPaiNoivoId", "PaiNoivoId", "PaiNoivoNome");
        HabilitaDesabilitaCampos($("#MaeNoivoMembro").prop('checked'), "lnkMaeNoivoId", "MaeNoivoId", "MaeNoivoNome");
        HabilitaDesabilitaCampos($("#NoivaMembro").prop('checked'), "lnkNoivaId", "NoivaId", "NoivaNome");
        HabilitaDesabilitaCampos($("#PaiNoivaMembro").prop('checked'), "lnkPaiNoivaId", "PaiNoivaId", "PaiNoivaNome");
        HabilitaDesabilitaCampos($("#MaeNoivaMembro").prop('checked'), "lnkMaeNoivaId", "MaeNoivaId", "MaeNoivaNome");

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
                        $("#PastorNome").val(congregacao.PastorResponsavelNome);
                        $("#PastorId").attr("readonly", "readonly");
                        $("#PastorNome").attr("readonly", "readonly");
                    }
                });
            }
        });

        $("#HoraFim").change(function () {
            var CasamentoId = $("#CasamentoId").val();
            var CongregacaoNome = $("#CongregacaoNome").val();
            var Congregracao = $("#CongregacaoId").val();
            var DataCasamento = $("#DataCasamento").val();
            var HoraInicio = $("#HoraInicio").val();
            var HoraFim = $(this).val();

            if (Congregracao == undefined || Congregracao == 0) {
                $.bsMessageError("Atenção", "Congregação é de preenchimento obrigatório.");
                return;
            }
            else if (DataCasamento == undefined || DataCasamento == "") {
                $.bsMessageError("Atenção", "Data de Casamento é de preenchimento obrigatório.");
                return;
            }
            else if (HoraInicio == undefined || HoraInicio == "") {
                $.bsMessageError("Atenção", "Hora do Início do Casamento é de preenchimento obrigatório.");
                return;
            }
            else if (HoraFim == undefined || HoraFim == "") {
                $.bsMessageError("Atenção", "Hora do Término do Casamento é de preenchimento obrigatório.");
                return;
            }
            else {
                var dataArray = DataCasamento.split("/");
                var newDataCasamento = dataArray[1] + "/" + dataArray[0] + "/" + dataArray[2];

                var stt = new Date(newDataCasamento + " " + HoraInicio);
                stt = stt.getTime();

                var endt = new Date(newDataCasamento + " " + HoraFim);
                endt = endt.getTime();

                if (stt > endt) {
                    $.bsMessageError("Atenção", "Hora do Início é maior que a Hora Final do Casamento.");
                }

                var parametro = {
                    CongregacaoId: Congregracao,
                    CongregacaoNome,
                    DataCasamento: DataCasamento,
                    HoraInicio: HoraInicio,
                    HoraFim: HoraFim,
                    CasamentoId: CasamentoId
                };

                $.ajaxPost("/Secretaria/Casamento/VerificarCasamento", parametro, function (data) {
                    if (data.status == "OK") {
                        if (data.dados.CasamentoId > 0) {
                            $.bsMessageError("Atenção", "Casamento já agendado para esta data!");
                            $("#DataCasamento").val("");
                            $("#HoraInicio").val("");
                            $("#HoraFim").val("");
                        }
                    }
                    else if (data.status == "ERRO") {
                        $.bsMessageError(data.mensagem);
                        $("#DataCasamento").val("");
                        $("#HoraInicio").val("");
                        $("#HoraFim").val("");
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
                    buscarMembro(value, "PastorNome");
                    if ($("#PastorNome").val() !== '' || $("#PastorNome").val() !== null || $("#PastorNome").val() !== undefined) {
                        $("#PastorId").attr("readonly", "readonly");
                    }
                }
            }
        });

        $("#PastorNome").change(function () {
            if ($(this).val() === "" || $(this).val() === null || $(this).val() === undefined) {
                $("#PastorId").removeAttr("readonly");
            }
        });

        $("#PastorMembro").change(function () {
            if ($("#PastorMembro").prop('checked') === false) {
                $("#PastorId").removeAttr("readonly");
                $("#PastorNome").removeAttr("readonly");
                $("label[for=PastorId]").hide();
                $("#lnkPastorId").show();
                $("#PastorId").addClass("alinhar");
                $("#PastorId").val("");
                $("#PastorNome").val("");
            }
            else {
                $("#PastorId").attr("readonly", "readonly");
                $("#PastorNome").attr("readonly", "readonly");
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
                            $("#PastorNome").val(congregacao.PastorResponsavelNome);
                            $("#PastorId").attr("readonly", "readonly");
                            $("#PastorNome").attr("readonly", "readonly");
                        }
                    });
                }
            }

        });

        // ----------------------------------------- Noivo
        $("#lnkNoivoId").popUp("Pesquisar - Noivo", "Membro", "NoivoId", paramsMembro);

        $("#NoivoId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "NoivoNome");
        });

        $("#NoivoMembro").change(function () {
            HabilitaDesabilitaCampos($("#NoivoMembro").prop('checked'), "lnkNoivoId", "NoivoId", "NoivoNome");
            if ($("#NoivoMembro").prop('checked')) {
                $("#NoivoNome").val("");
            }
            else {
                $("#NoivoId").val("");
            }
        });

        // ----------------------------------------- Pai Noivo
        $("#lnkPaiNoivoId").popUp("Pesquisar Pai do Noivo", "Membro", "PaiNoivoId", paramsMembro);

        $("#PaiNoivoId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "PaiNoivoNome");
        });

        $("#PaiNoivoMembro").change(function () {
            HabilitaDesabilitaCampos($("#PaiNoivoMembro").prop('checked'), "lnkPaiNoivoId", "PaiNoivoId", "PaiNoivoNome");
            if ($("#PaiNoivoMembro").prop('checked')) {
                $("#PaiNoivoNome").val("");
            }
            else {
                $("#PaiNoivoId").val("");
            }
        });

        // ----------------------------------------- Mãe Noivo
        $("#lnkMaeNoivoId").popUp("Pesquisar Mãe do Noivo", "Membro", "MaeNoivoId", paramsMembro);

        $("#MaeNoivoId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "MaeNoivoNome");
        });

        $("#MaeNoivoMembro").change(function () {
            HabilitaDesabilitaCampos($("#MaeNoivoMembro").prop('checked'), "lnkMaeNoivoId", "MaeNoivoId", "MaeNoivoNome");
            if ($("#MaeNoivoMembro").prop('checked')) {
                $("#MaeNoivoNome").val("");
            }
            else {
                $("#MaeNoivoId").val("");
            }
        });

        // ----------------------------------------- Noiva
        $("#lnkNoivaId").popUp("Pesquisar Noiva", "Membro", "NoivaId", paramsMembro);

        $("#NoivaId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "NoivaNome");
        });

        $("#NoivaMembro").change(function () {
            HabilitaDesabilitaCampos($("#NoivaMembro").prop('checked'), "lnkNoivaId", "NoivaId", "NoivaNome");
            if ($("#NoivaMembro").prop('checked')) {
                $("#NoivaNome").val("");
            }
            else {
                $("#NoivaId").val("");
            }
        });

        // ----------------------------------------- Pai Noiva
        $("#lnkPaiNoivaId").popUp("Pesquisar Pai da Noiva", "Membro", "PaiNoivaId", paramsMembro);

        $("#PaiNoivaId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "PaiNoivaNome");
        });

        $("#PaiNoivaMembro").change(function () {
            HabilitaDesabilitaCampos($("#PaiNoivaMembro").prop('checked'), "lnkPaiNoivaId", "PaiNoivaId", "PaiNoivaNome");
            if ($("#PaiNoivaMembro").prop('checked')) {
                $("#PaiNoivaNome").val("");
            }
            else {
                $("#PaiNoivaId").val("");
            }
        });

        // ----------------------------------------- Mãe Noiva
        $("#lnkMaeNoivaId").popUp("Pesquisar Mãe da Noiva", "Membro", "MaeNoivaId", paramsMembro);

        $("#MaeNoivaId").change(function () {
            var value = $(this).val();
            buscarMembro(value, "MaeNoivaNome");
        });

        $("#MaeNoivaMembro").change(function () {
            HabilitaDesabilitaCampos($("#MaeNoivaMembro").prop('checked'), "lnkMaeNoivaId", "MaeNoivaId", "MaeNoivaNome");
            if ($("#MaeNoivaMembro").prop('checked')) {
                $("#MaeNoivaNome").val("");
            }
            else {
                $("#MaeNoivaId").val("");
            }
        });

        $("#btnSalvar").click(function (e) {
            e.preventDefault;
            var msg = "Deseja realmente criar um novo Casamento?";
            if (actionName === "Edit") {
                msg = "Deseja realmente atualizar o Casamento?";
            }
            $.showYesNoAlert(msg, "Casamento", function () {
                var form = $("#frmCasamento");
                var url = form.attr('action');

                if (form.valid()) {
                    $.ajaxPostForm(url, $("#frmCasamento"), function (result) {
                        if (result.status === "OK") {
                            window.location = result.url;
                        }
                        else {
                            $.alertError("Batismo", result.msg);
                        }
                    });
                }
            });
        });
    }
});

function excluirCasamento() {
    var request = { id: $("#lnkExcluir").data("casamentoid") };
    $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Casamento?</br>Após a confirmação NÃO será mais possível recuperar o Casamento.</h3>', "Casamento",
        function () {
            $.ajaxPost(urlExcluir, request, function (response) {
                if (response.status === "OK") {
                    window.location = response.url;
                }
                else {
                    $.alertError("Casamento", response.mensagem);
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
            if (membro.Id == 0) {
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