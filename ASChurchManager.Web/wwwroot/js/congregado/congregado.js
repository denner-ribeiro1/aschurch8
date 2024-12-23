$(function () {
    "use strict";

    var batismo = $.getQuerystringValue("Batismo");

    if (batismo == 1) {
        $("#lnkEdit").css("display", "none");
        $("#lnkDelete").css("display", "none");
    }
    else {
        $("#lnkEnvBatismo").css("display", "none");
    }

    $("#lnkBuscarCongregacao").removeAttr("class");

    var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
    if (usuarioCongregacaoSede === false) {
        // Usuário de outras congregações não podem fazer a busca
        $("#CongregacaoId").attr("readonly", "readonly");
        $("#lnkBuscarCongregacao").attr("class", "disabled");
    } else {
        // Para usuario sede habilita busca de congregação
        $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
    }

    $("#CongregacaoId").blur(function () {
        var value = $(this).val();
        if (value > 0) {

            var requestData = {
                CongregacaoId: value
            };

            $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (congregacao) {
                $("#CongregacaoNome").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código inválido!");
                    $(this).val("");
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoNome").val(congregacao.Nome);
                }
            });
        }
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("membroid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Congregado?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Congregado",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Membro", response.mensagem);
                    }
                });
            });
    });


    $("#btnSalvar").click(function () {
        var msg = "Deseja realmente criar um novo Congregado?";
        if ($("#Acao").val() == "Update") {
            msg = "Deseja realmente atualizar o Congregado?";
        }
        $.showYesNoAlert(msg, "Congregado", function () {
            $("#frmCongreg").submit();
        });
    });
});