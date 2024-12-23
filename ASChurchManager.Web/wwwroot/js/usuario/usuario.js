$(function () {
    "use strict";

    $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");

    $("#CongregacaoId").blur(function () {

        var value = $(this).val();
        if (value > 0) {

            var requestData = {
                CongregacaoId: value
            };

            $.ajaxPost(urlBuscarCongregacao, requestData, function (congregacao) {
                $("#CongregacaoNome").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código inválido!");
                    $(this).val('');
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoNome").val(congregacao.Nome);
                }
            });
        }
    });

    $("#Username").blur(function () {
        var value = $(this).val();
        if (value != null && value != undefined && value != "") {
            var requestData = {
                username: value
            };
            $.ajaxPost(urlUsuarioDuplicado, requestData, function (result) {
                if (result == true) {
                    var errorUsername = $(document).find("[data-valmsg-for='Username']");
                    errorUsername.html('<span id="Nome-error" class="">Usuário já existe</span>');
                    return false;
                }
            });
        }
    });

    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Usuário?", "Usuário", function () {
            $("#frmUsuario").submit();
        });
    })
    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("cargoid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Usuário?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Cargo",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Cargo", response.mensagem);
                    }
                });
            });
    });
});