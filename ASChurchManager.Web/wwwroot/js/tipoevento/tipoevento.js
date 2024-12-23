$(function () {
    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Tipo de Evento?", "Tipo de Evento", function () {
            $("#frmTipoEvento").submit();
        });
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $("#Id").val() };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Tipo de Evento?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Tipo de Evento",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Tipo de Evento", response.mensagem);
                    }
                });
            });
    });
})