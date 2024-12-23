$(() => {

    try {
        const ipAPI = '//api.ipify.org?format=json';
        fetch(ipAPI)
            .then(response => response.json())
            .then(data => {
                let ip = data.ip;
                $("#spanip").text(`Seu IP: ${ip}`);
                $("#IP").val(ip);
            }).catch(() => { $("#spanip").text("") });
    } catch (e) {
        console.log(e);
    }

    $("#lnkTermos").click((e) => {
        let cpf = $("#Cpf").val();
        let nome = $("#Nome").val();
        let rg = $("#Rg").val();
        let ip = $("#IP").val();
        let url = `${urlPopUp}?cpf=${cpf}&nome=${nome}&rg=${rg}&ip=${ip}`;
        $.popUpUrl(url, 550, 655);
    });

    $("#btnBuscarCEP").click((e) => {
        e.preventDefault();
        PesquisarCEP($("#Cep"), $("#Logradouro"), $("#Bairro"), $("#Cidade"), $("#Estado"));
    });

    $("#btnSalvar").click((e) => {
        e.preventDefault();
        let form = $("#frmExterno");
        let aceita = $("#AceitaTermos").prop('checked');
        if (aceita === false) {
            $.alertError("Membro", "Favor ler e aceitar o TERMO DE AUTORIZAÇÃO PARA TRATAMENTO DE DADOS – LGPD");
            $("#AceitaTermos").focus();
        }
        else {
            var response = grecaptcha.getResponse();
            if (response.length == 0) {
                $.alertError("Membro", "Favor validar o Captcha!");
                document.getElementById('g-recaptcha-error').innerHTML = '<span style="color:red;">Favor validar o Captcha!</span>';
                return false;
            }
            if (form.valid()) {
                $.showYesNoAlert("Deseja finalizar a atualização do cadastro?", "Membro", function () {
                    $.ajaxPostForm(urlSalvar, $("#frmExterno"), function (result) {
                        if (result.status === "OK") {
                            Swal.fire(
                                "Confirmação do Membro",
                                `Membro atualizado com Sucesso!<br/>${result.msg}`,
                                'info'
                            ).then(() => {
                                window.location = result.url;
                            });
                        }
                        else {
                            $.alertError("Membro", result.msg);
                        }
                    });
                });
            }
            else {
                var msg = ValidarFormulario();
                if (msg != null && msg != undefined && msg != "")
                    $.alertError("Membro", msg);
            }
        }
    });
});

function ValidarFormulario() {
    var erro = "";
    $("#divMembro").find('span[id*=-error]').each(function (x, item) {
        erro += $(item).text() + "<br>";
    });
    return erro;
}

function verifyCaptcha() {
    document.getElementById('g-recaptcha-error').innerHTML = '';
}