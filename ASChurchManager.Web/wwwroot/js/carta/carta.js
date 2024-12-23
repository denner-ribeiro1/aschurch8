$(function () {
    "use strict";

    Date.prototype.addDias = function (dias) {
        this.setDate(this.getDate() + dias);
    };

    var acao = $("#acao").val();
    if (acao == "Update" || acao == "Read" || acao == "Delete") {
        var E = document.getElementById("divCongregacaoDest");
        var tipo = $('option:selected', $("#TipoCarta")).text();

        E.style.display = "block";
        var divCong = document.getElementById("divCodCongrDest");
        if (tipo == "Transferência") {
            divCong.style.display = "block";
        }
        else if (tipo == "Mudança" || tipo == "Recomendação") {
            divCong.style.display = "none";
        }
    }
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

    $("#DataValidade").change(function () {
        var data = $(this).val();

        var erroDataValidade = $(document).find("[data-valmsg-for='DataValidade']");
        erroDataValidade.html('');

        if (data != null && data != undefined && data != "") {
            var objDate = new Date();
            objDate.setYear(data.split("/")[2]);
            objDate.setMonth(data.split("/")[1] - 1);//- 1 pq em js é de 0 a 11 os meses
            objDate.setDate(data.split("/")[0]);

            if (objDate.getTime() < new Date().getTime()) {
                erroDataValidade.html('<span id="DataValidade-error" class="">A Data de Validade da Carta deve ser maior que a Data Atual</span>');
                return false;
            };
        };
    });

    // ----------------------------------------- Tipo Carta
    $("#TipoCarta").change(function () {
        var data = $('option:selected', $(this)).text();

        var realDate = new Date();
        realDate.addDias(data === "Recomendação" ? 30 : 60);
        var dtVal = (realDate.getDate() < 9 ? "0" : "") + realDate.getDate() + "/" + (realDate.getMonth() + 1 < 9 ? "0" : "") + (realDate.getMonth() + 1) + "/" + realDate.getFullYear();
        $('#DataValidade').val(dtVal);

        document.getElementById("divCongregacaoDest").style.display = "block";
        var divCong = document.getElementById("divCodCongrDest");
        if (data === "Transferência") {
            divCong.style.display = "block";
            $("#CongregacaoDestId").val("");
            $("#CongregacaoDestId").focus();
            $("#CongregacaoDest").val("");
            $("#CongregacaoDest").attr("readonly", "readonly");
            $("#CongregacaoDest").val("");
        }
        else if (data === "Mudança" || data === "Recomendação") {
            divCong.style.display = "none";
            $("#CongregacaoDest").removeAttr("readonly");
            $("#CongregacaoDest").val("");
            $("#CongregacaoDest").focus();
        }
    });

    // ----------------------------------------- Congregação
    $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoDestId");

    $("#CongregacaoDestId").change(function () {
        var errorCongr = $(document).find("[data-valmsg-for='CongregacaoDestId']");
        errorCongr.html('');

        var congrDest = $(this).val();
        var congrOrig = $("#CongregacaoOrigemId").val();

        if (congrDest == congrOrig) {
            errorCongr.html('<span id="CongregacaoDestId-error" class="">Congregação Destino deve ser diferente da Congregação Origem</span>');
            return false;
        }

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

    // ----------------------------------------- Congregação
    $("#Observacao").keydown(function (event) {
        textCounter();
    });

    $("#Observacao").keyup(function () {
        textCounter();
    });

    $("#TipoCartaSelecionado").change(function () {
        FiltrarGrid(
            $('option:selected', $(this)).text(),
            $('option:selected', $("#StatusCartaSelecionado")).text());
    });

    $("#StatusCartaSelecionado").change(function () {
        FiltrarGrid(
            $('option:selected', $("#TipoCartaSelecionado")).text(),
            $('option:selected', $(this)).text());
    });

    $("#CodigoEmissao").change(function () {
        if ($("#CodigoEmissao").val() !== null && $("#CodigoEmissao").val() !== '') {
            var errorCodigo = $(document).find("[data-valmsg-for='CodigoEmissao']");
            errorCodigo.html('');

            var requestData = {
                id: $("#Id").val(),
                codigoEmissao: $("#CodigoEmissao").val()
            };

            $.ajaxPost("/Carta/ValidarCodigoRecebCarta", requestData, function (erro) {
                if (erro.Erro !== null && erro.Erro !== undefined && erro.Erro !== "") {
                    errorCodigo.html('<span id="CodigoEmissao-error" class="">' + erro.Erro + '</span>');
                    $.bsMessageError("Atenção", erro.Erro);
                    $(this).val("");
                }
            });
        }
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("cartaid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja Cancelar esta Carta?</br>Após a confirmação NÃO será mais possível reverter o Cancelamento.</h3>', "Carta de Transferência",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Carta de Transferência", response.mensagem);
                    }
                });
            });
    });

    $(document).on('click', '#lnkPrint', {}, function (e) {
        e.preventDefault();
        var templateid = $(this).data("templateid");
        var targetmodel = $(this).data("targetmodel");
        var id = $(this).data("id");

        $.showYesNoAlert('Deseja imprimir esta Carta ?', "Imprimir Carta",
            function () {
                var url = urlPrint + "?templateId=" + templateid + "&targetModel=" + targetmodel + "&modelId=" + id;
                $.downloadArquivo(url, null);
            });
    });


    $("#btnAprovar").click(function (event) {
        event.preventDefault;
        $.showYesNoAlert("Após a Aprovação não será possível atualizar o Conteúdo da Carta. Deseja realmente APROVAR a Carta?", "Carta de Transferência",
            function () {
                var request = {
                    id: $("#Id").val(),
                    codigoRecebimento: $("#CodigoEmissao").val()
                };

                $.ajaxPost(urlAprovar, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Carta de Transferência", response.mensagem);
                    }
                });


            });
    });

    $("#btnSalvar").click(function (event) {
        event.preventDefault;

        if (isNaN(parseInt($("#MembroId").val())) || parseInt($("#MembroId").val()) == 0) {
            $.alertError("Carta de Transferência", "Registro do Membro deve ser maior que zero");
            $("#MembroId").focus();
        }
        else if ($("#Nome").val() == "" || $("#Nome").val() == null || $("#Nome").val() == undefined) {
            $.alertError("Carta de Transferência", "Nome do Membro é de preenchimento obrigatório");
            $("#Nome").focus();
        }
        else if ($('option:selected', $("#TipoCarta")).text() == "") {
            $.alertError("Carta de Transferência", "Tipo de Carta é de preenchimento obrigatório");
            $("#TipoCarta").focus();
        }
        else if ($('option:selected', $("#TipoCarta")).text() == "Transferência" &&
            $("#CongregacaoDest").val() == "" || $("#CongregacaoDest").val() == null || $("#CongregacaoDest").val() == undefined) {
            $.alertError("Carta de Transferência", "Congregação de Destino é de preenchimento obrigatório");
            $("#CongregacaoDest").focus();
        }
        else if ($('option:selected', $("#TemplateCartaSelecionado")).text() == "") {
            $.alertError("Carta de Transferência", "Template de Carta é obrigatório");
            $("#TemplateCartaSelecionado").focus();
        }
        else if (!isNaN(parseFloat($("#CongregacaoDestId").val()))
            && parseFloat($("#CongregacaoDestId").val()) > 0
            && $("#CongregacaoDestId").val() == $("#CongregacaoOrigemId").val()) {
            $.alertError("Carta de Transferência", "Congregação Destino deve ser diferente da Congregação Origem");
            $("#CongregacaoDestId").focus();
        }
        else {
            var msg = "Deseja realmente criar uma nova Carta de Transferência?";
            if ($("#acao").val() == "Update") {
                msg = "Deseja realmente atualizar a Carta de Transferência?";
            }
            $.showYesNoAlert(msg, "Carta de Transferência",
                function () {
                    $.ajaxPostForm(urlSalvar, $("#frmCarta"), function (response) {
                        if (response.status == "OK") {
                            var redirect = response.url;
                            var id = response.id;
                            var templateid = response.templateid;
                            $.showYesNoAlert(response.mensagem, "Carta de Transferência", function () {
                                var url = urlPrint + "?templateId=" + templateid + "&targetModel=Carta&modelId=" + id;
                                $.downloadArquivo(url, null, redirect);
                            }, null, function () {
                                window.location = redirect;
                            }, null);
                        }
                        else {
                            $.alertError("Carta de Transferência", response.mensagem);
                        }
                    });
                });
        }
    });

});

function textCounter() {
    var maxlimit = 1000;
    var tam = $("textarea[id$='Observacao']").val().length;
    if (tam > maxlimit)
        $("#Observacao").val(field.substring(0, maxlimit));
    else
        $("#qtdCarac").val(maxlimit - tam);
}

function FiltrarGrid(tipo, status) {
    var requestData = {
        TipoCarta: tipo,
        Status: status
    };

    $.ajaxPost("/Secretaria/Carta/GridCarta", requestData, function (partialViewResult) {
        $("#divGridCarta").html(partialViewResult);
    }, "html");
}