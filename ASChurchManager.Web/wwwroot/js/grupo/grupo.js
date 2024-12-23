$(function () {
    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Grupo?", "Grupo", function () {
            $("#frmGrupo").submit();
        });
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $("#Id").val() };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Grupo?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Grupo",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Grupo", response.mensagem);
                    }
                });
            });
    });
})