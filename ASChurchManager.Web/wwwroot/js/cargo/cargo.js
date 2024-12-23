$(function () {
    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Cargo?", "Cargo", function () {
            $("#frmCargo").submit();
        });
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $("#Id").val() };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Cargo?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Cargo",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Cargo", response.mensagem);
                    }
                });
            });
    });
})