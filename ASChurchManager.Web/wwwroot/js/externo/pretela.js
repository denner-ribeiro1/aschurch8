let fase = 0;

$(function () {
    try {
        const ipAPI = '//api.ipify.org?format=json';
        fetch(ipAPI)
            .then(response => response.json())
            .then(data => {
                $("#spanip").text(`Seu IP: ${data.ip}`)

            }).catch(() => { $("#spanip").text("") });
    } catch (e) {
        console.log(e);
    }

    Carregar(0);

    $("#lnkAvançar").click((e) => {
        e.preventDefault();
        let form = $("#frmExterno");
        if (form.valid()) {

            var token = $('input[name="__RequestVerificationToken"]', form).val();

            let params = {
                __RequestVerificationToken: token,
                fase: fase,
                cpf: $("#Cpf").val(),
                valor: ""
            }
            if (fase == 0) {
                $.ajaxPost(urlPesquisarMembro, params, function (data) {
                    if (data.Status === "OK") {
                        fase++;
                        Carregar(fase);
                    }
                    else {
                        $.bsMessageError("Atenção", data.Msg);
                    }
                });
            }
            else if (fase === 1) {
                params.valor = $("#NomeMae").val();
                $.ajaxPost(urlPesquisarMembro, params, function (data) {
                    if (data.Status === "OK") {
                        fase++;
                        Carregar(fase);
                    }
                    else {
                        $.bsMessageError("Atenção", data.Msg);
                    }
                });
            }
            else if (fase === 2) {
                params.valor = $("#DataNascimento").val();

                $.ajaxPost(urlPesquisarMembro, params, function (data) {
                    if (data.Status === "OK") {
                        window.location = data.Url;
                    }
                    else {
                        $.bsMessageError("Atenção", data.Msg);
                    }
                });
            }
        }
        else {
            var msg = ValidarFormulario();
            if (msg != null && msg != undefined && msg != "")
                $.alertError("Membro", msg);
        }
    })
});

function ValidarFormulario() {
    var erro = "";
    $(".validar").find('span[id*=-error]').each(function (x, item) {
        erro += $(item).text() + "<br>";
    });
    return erro;
}
const Carregar = (fase) => {
    switch (fase) {
        case 0:
            $("#divCpf").show();
            $("#divNomeMae").hide();
            $("#divDataNasc").hide();
            break;
        case 1:
            $("#divCpf").hide();
            $("#divNomeMae").show();
            $("#divDataNasc").hide();
            break;
        case 2:
            $("#divCpf").hide();
            $("#divNomeMae").hide();
            $("#divDataNasc").show();
            break;
    }
}