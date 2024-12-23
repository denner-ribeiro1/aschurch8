$(function () {
    let opc = $("#Pessoa_EstadoCivil").val() == "2"; //Casado

    if (actionName != undefined && actionName != null) {
        if (actionName === "Details" || actionName === "AprovarReprovar") {
            HabilitaDesabilitaCampos(false, "lnkPaiId", "Pessoa_PaiId", "Pessoa_NomePai");
            HabilitaDesabilitaCampos(false, "lnkMaeId", "Pessoa_MaeId", "Pessoa_NomeMae");
            HabilitaDesabilitaCampos(false, "lnkIdConjuge", "Pessoa_IdConjuge", "Pessoa_NomeConjuge");
        }
        else {
            HabilitaDesabilitaCampos($("#Pessoa_PaiMembro").prop('checked'), "lnkPaiId", "Pessoa_PaiId", "Pessoa_NomePai");
            HabilitaDesabilitaCampos($("#Pessoa_MaeMembro").prop('checked'), "lnkMaeId", "Pessoa_MaeId", "Pessoa_NomeMae");
            HabilitaDesabilitaCampos(opc && $("#Pessoa_ConjugeMembro").prop('checked'), "lnkIdConjuge", "Pessoa_IdConjuge", "Pessoa_NomeConjuge");
        }
    }
    if (opc == true)
        $("#divConjuge").css("display", "block");
    else
        $("#divConjuge").css("display", "none");

    $("#lnkPaiId").popUp("Pesquisar - Pai", "Membro", "Pessoa_PaiId", { FiltrarUsuario: "N" });

    $("#Pessoa_PaiId").change(function () {
        var value = $(this).val();
        buscarMembro(value, "Pessoa_NomePai", this);
    });

    $("#Pessoa_PaiMembro").change(function () {
        HabilitaDesabilitaCampos($("#Pessoa_PaiMembro").prop('checked'), "lnkPaiId", "Pessoa_PaiId", "Pessoa_NomePai");
        if ($("#Pessoa_PaiMembro").prop('checked')) {
            $("#Pessoa_NomePai").val("");
        }
        else {
            $("#Pessoa_PaiId").val("");
        }
    });

    $("#lnkMaeId").popUp("Pesquisar - Mãe", "Membro", "Pessoa_MaeId", { FiltrarUsuario: "N" });

    $("#Pessoa_MaeId").change(function () {
        var value = $(this).val();
        buscarMembro(value, "Pessoa_NomeMae", this);
    });

    $("#Pessoa_MaeMembro").change(function () {
        HabilitaDesabilitaCampos($("#Pessoa_MaeMembro").prop('checked'), "lnkMaeId", "Pessoa_MaeId", "Pessoa_NomeMae");
        if ($("#Pessoa_MaeMembro").prop('checked')) {
            $("#Pessoa_NomeMae").val("");
        }
        else {
            $("#Pessoa_MaeId").val("");
        }
    });

    $("#Pessoa_ConjugeMembro").change(function () {
        HabilitaDesabilitaCampos($("#Pessoa_ConjugeMembro").prop('checked'), "lnkIdConjuge", "Pessoa_IdConjuge", "Pessoa_NomeConjuge");
        if ($("#Pessoa_ConjugeMembro").prop('checked')) {
            $("#Pessoa_NomeConjuge").val("");
        }
        else {
            $("#Pessoa_IdConjuge").val("");
        }
    });

    $("#Pessoa_Cpf").change(function () {
        var value = $(this).val();
        if (value != null && value != undefined && value != "") {
            var requestData = {
                Cpf: value,
                Id: $("#Id").val()
            };
            $.ajaxPost("/Secretaria/Membro/VerificaCpfDuplicado", requestData, function (result) {
                if (result.status == 'ERRO') {
                    var errorCpf = $(document).find("[data-valmsg-for='Pessoa.Cpf']");
                    errorCpf.html('<span id="Nome-error" class="">' + result.mensagem + '</span>');
                    return false;
                }
            });
        }
    });

    /************************************************************************************************************
    * CONJUGE
    ************************************************************************************************************/
    $("#lnkIdConjuge").popUp("Pesquisar Cônjuge", "Membro", "Pessoa_IdConjuge", { FiltrarUsuario: "N" });

    $("#Pessoa_IdConjuge").change(function () {
        var value = $(this).val();
        if (value > 0) {
            var requestData = {
                MembroId: value
            };

            $.ajaxPost(urlBuscarMembro, requestData, function (membro) {
                $("#Pessoa_NomeConjuge").val("");
                if (membro.Id == 0) {
                    $.bsMessageError("Atenção", "Membro não cadastrado!");
                    $("#Pessoa_IdConjuge").val("");
                    $("#Pessoa_NomeConjuge").val("");
                } else if (membro.Id > 0) {
                    $("#Pessoa_NomeConjuge").val(membro.Nome);
                    $("#Endereco_Logradouro").val(membro.Endereco.Logradouro);
                    $("#Endereco_Numero").val(membro.Endereco.Numero);
                    $("#Endereco_Complemento").val(membro.Endereco.Complemento);
                    $("#Endereco_Pais").val(membro.Endereco.Pais);
                    HabilitaCamposEndEstrang(membro.Endereco.Pais != "Brasil");

                    if (membro.Endereco.Pais == "Brasil") {
                        $("#Endereco_Cep").val(membro.Endereco.Cep);
                        $("#Endereco_Bairro").val(membro.Endereco.Bairro);
                        $("#Endereco_Cidade").val(membro.Endereco.Cidade);
                        $("#Endereco_Estado").val(membro.Endereco.Estado);
                    }
                    else {
                        $("#Endereco_CodigoPostal").val(membro.Endereco.Cep);
                        $("#Endereco_BairroEstrangeiro").val(membro.Endereco.Bairro);
                        $("#Endereco_CidadeEstrangeiro").val(membro.Endereco.Cidade);
                        $("#Endereco_Provincia").val(membro.Endereco.Estado);
                    }

                    switch (membro.Sexo) {
                        case 1: $("#Pessoa_Sexo").val(2);
                        case 2: $("#Pessoa_Sexo").val(1);
                        default:
                    }

                };
            });
        };
    });

    $("#Pessoa_EstadoCivil").change(function () {
        let opc = $('option:selected', $(this)).text() == "Casado";
        HabilitaDesabilitaCampos(opc && $("#Pessoa_ConjugeMembro").prop('checked'), "lnkIdConjuge", "Pessoa_IdConjuge", "Pessoa_NomeConjuge");
        HabilitarLinkConjuge(opc);

        if (opc == true)
            $("#divConjuge").css("display", "block");
        else
            $("#divConjuge").css("display", "none");
    });

    let paisStr = $("#Endereco_Pais").val();
    HabilitaCamposEndEstrang(paisStr != "Brasil");

    $("#Pessoa_Nome").focus();
});

function HabilitarLinkConjuge(enable) {
    $('#IdConjuge').hide();
    $("#lnkIdConjuge").hide();

    if (enable) {
        $("#lnkIdConjuge").show();
        $("#Pessoa_IdConjuge").removeAttr("readonly");
        $("#Pessoa_IdConjuge").attr("style", "margin-top: 5px;");
        $("#Pessoa_ConjugeMembro").removeAttr("disabled").change();
    }
    else {
        $('#IdConjuge').show();
        $("#Pessoa_ConjugeMembro").prop('checked', false).change();
        $("#Pessoa_ConjugeMembro").attr("disabled", "disabled");
        $("#Pessoa_IdConjuge").attr("readonly", "readonly");
        $("#Pessoa_IdConjuge").val("");
        $("#Pessoa_IdConjuge").attr("style", "margin-top: 0px;");
        $("#Pessoa_NomeConjuge").attr("readonly", "readonly");
        $("#Pessoa_NomeConjuge").val("");
    };
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

function HabilitaCamposEndEstrang(habilita) {
    if (habilita) {
        $("#divCaixaPostal").css("display", "block");
        $("#divCep").css("display", "none");
        $("#divEndBrasil").css("display", "none");
        $("#divEndEstrang").css("display", "block");

        $("#divTelResBr").css("display", "none");
        $("#divTelCelBr").css("display", "none");
        $("#divTelComBr").css("display", "none");

        $("#divTelResEst").css("display", "block");
        $("#divTelCelEst").css("display", "block");
        $("#divTelComEst").css("display", "block");
    }
    else {
        $("#divCaixaPostal").css("display", "none");
        $("#divCep").css("display", "block");
        $("#divEndBrasil").css("display", "block");
        $("#divEndEstrang").css("display", "none");

        $("#divTelResBr").css("display", "block");
        $("#divTelCelBr").css("display", "block");
        $("#divTelComBr").css("display", "block");

        $("#divTelResEst").css("display", "none");
        $("#divTelCelEst").css("display", "none");
        $("#divTelComEst").css("display", "none");
    }
}

function buscarMembro(requestId, elementTargetId, elem) {
    if (requestId > 0) {
        var requestData = {
            MembroId: requestId
        };
        elementTargetId = "#" + elementTargetId;

        $.ajaxPost(urlBuscarMembro, requestData, function (membro) {
            $(elementTargetId).val("");
            if (membro.Id == 0) {
                $.bsMessageError("Atenção", "Membro não cadastrado!");
                $(elem).val("");
            } else if (membro.Id > 0) {
                $(elementTargetId).val(membro.Nome);
            }
        });
    }
}