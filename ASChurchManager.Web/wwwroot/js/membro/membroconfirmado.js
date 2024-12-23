$(() => {
    "use strict";

    $("#btnRestaurar").click((e) => {
        Restaurar(false);
    });

    $("#btnRestaurarTodos").click((e) => {
        Restaurar(true);
    });

});

function Restaurar(todos) {
    let campos = [];
    let descrCampos = [];

    let msg = "Deseja restaurar "
    if (todos) {
        msg += " todos os campos do Membro?";
    }
    else {
        $(".checks").each((i, elem) => {
            if ($(elem).prop('checked') == true) {
                campos.push($(elem).val());
                descrCampos.push($(elem).data("descr"));
            }
        });
        
        if (campos.length == 1) {
            msg += `o campo ${descrCampos[0]}?`
        }
        else if (campos.length > 1) {
            msg += `os campos abaixo? <br /> ${descrCampos.join('<br />')}`
        }
        else {
            $.alertError('Restaurar Informações', 'Favor selecionar os campos a serem restaurados!');
            return;
        }
    }
    
    $.showYesNoAlert(msg, "Membro", function () {
        var request = {
            membroId: $("#Id").val(),
            campos: campos.join(',')
        };

        $.ajaxPost(urlRestaurarMembro, request, function (response) {
            if (response.status === "OK") {
                closeAndRefresh()
            }
            else {
                $.alertError("Membro", response.mensagem);
            }
        });
    });
}

function closeAndRefresh() {
    parent.location.reload();
    FecharModal()
}

function FecharModal() {
    parent.$.fancybox.close();
};