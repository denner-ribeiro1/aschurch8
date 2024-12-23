$(function () {
    "use strict";

    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Perfil?", "Perfil", function () {
            $("#frmPerfil").submit();
        });
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $("#Id").val() };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Perfil?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Perfil",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Perfil", response.mensagem);
                    }
                });
            });
    });

    $("#TipoPerfil").change(function () {
        var value = $('#TipoPerfil').val();

        var errorPerfil = $(document).find("[data-valmsg-for='TipoPerfil']");
        errorPerfil.html('');

        if (value == undefined || value == "") {
            errorPerfil.html('<span id="TipoPerfil-error" class="">Campo obrigatório</span>');
            $("#divRotinas").hide();
            return false;
        }

        if (value == 1) { // ADMINISTRADOR
            $("#divRotinas").css('display', 'none');
        } else {
            $("#divRotinas").css('display', 'block');
        }
    });

});