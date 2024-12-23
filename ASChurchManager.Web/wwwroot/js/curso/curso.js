$(function () {
    $("#DataInicio").blur(function () {
        var errorDtIni = $(document).find("[data-valmsg-for='DataInicio']");
        errorDtIni.html('');

        var dtFimS = $("#DataEncerramento").val();
        if (dtFimS != null && dtFimS != "" && dtFimS != undefined) {
            var dtIniArr = $("#DataInicio").val().split("/");
            var dtIni = new Date(dtIniArr[1] + "/" + dtIniArr[0] + "/" + dtIniArr[2]);

            var dtFimArr = dtFimS.split("/");
            var dtFim = new Date(dtFimArr[1] + "/" + dtFimArr[0] + "/" + dtFimArr[2]);

            if (dtFim < dtIni) {
                errorDtIni.html('<span id="DataInicio-error" class="">Data de Início deve ser menor ou igual a Data de Encerramento.</span>');
            }
        }
    });

    $("#DataEncerramento").blur(function () {
        var errordtEnc = $(document).find("[data-valmsg-for='DataEncerramento']");
        errordtEnc.html('');

        var dtIniS = $("#DataInicio").val();
        if (dtIniS != null && dtIniS != "" && dtIniS != undefined) {
            var dtIniArr = dtIniS.split("/");
            var dtIni = new Date(dtIniArr[1] + "/" + dtIniArr[0] + "/" + dtIniArr[2]);

            var dtFimArr = $("#DataEncerramento").val().split("/");
            var dtEnc = new Date(dtFimArr[1] + "/" + dtFimArr[0] + "/" + dtFimArr[2]);

            if (dtEnc < dtIni) {
                errordtEnc.html('<span id="DataEncerramento-error" class="">Data de Encerramento deve ser maior ou igual a Data de Início.</span>');
            }
        }
    });

    $("#btnSalvar").click(function () {
        $.showYesNoAlert("Deseja salvar as alterações no Curso?", "Curso", function () {

            var errorDtIni = $(document).find("[data-valmsg-for='DataInicio']");
            errorDtIni.html('');
            var errordtEnc = $(document).find("[data-valmsg-for='DataEncerramento']");
            errordtEnc.html('');

            var dtIniS = $("#DataInicio").val();
            var dtIniArr = dtIniS.split("/");
            var dtIni = new Date(dtIniArr[1] + "/" + dtIniArr[0] + "/" + dtIniArr[2]);

            var dtFimArr = $("#DataEncerramento").val().split("/");
            var dtEnc = new Date(dtFimArr[1] + "/" + dtFimArr[0] + "/" + dtFimArr[2]);

            if (dtEnc < dtIni) {
                errordtEnc.html('<span id="DataEncerramento-error" class="">Data de Encerramento deve ser maior ou igual a Data de Início.</span>');
            }
            $("#frmCurso").submit();
        });
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("cursoid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Curso?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Curso",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Curso", response.mensagem);
                    }
                });
            });
    });
})