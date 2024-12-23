$(function () {
    $(document).on('click', '#lnkMotivoReprovacao', {}, function (e) {
        e.preventDefault();

        var requestData = {
            Id: $("#Id").val(),
            MotivoReprovacao: $("#MotivoReprovacao").val()
        };


        var errorMotivoReprovacao = $(document).find("[data-valmsg-for='MotivoReprovacao']");
        errorMotivoReprovacao.html('');

        var error = false;
        if (requestData.MotivoReprovacao == null || requestData.MotivoReprovacao == undefined || requestData.MotivoReprovacao == "") {
            error = true;
            errorMotivoReprovacao.html('<span id="MotivoReprovacao-error" class="">O campo Motivo de Reprovação é obrigatório</span>');
        }
        if (error) {
            return false;
        }

        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja Reprovar este Membro?</h3>', "Membros",
            function () {
                $.ajaxPost("/Secretaria/Membro/ReprovarMembro", requestData, function (json) {
                    if (json.erro == "") {
                        window.top.location.href = json.url;
                    }
                    else {
                        $.bsMessageError("Atenção", "Ocorreu o seguinte erro ao tentar processar sua requisição. Mensagem: " + json.erro);
                    }
                });

            });
    });

    $("#lnkFechar").click(function () {
        parent.$.fancybox.close();
    });

});
