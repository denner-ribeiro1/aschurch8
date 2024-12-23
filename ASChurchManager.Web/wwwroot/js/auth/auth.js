$(() => {
    $("#btnEntrar").click((e) => {
        e.preventDefault();
        let form = $("#frmLogin");
        var response = grecaptcha.getResponse();
        if (response.length == 0) {
            $.alertError("Login", "Favor validar o Captcha!");
            document.getElementById('g-recaptcha-error').innerHTML = '<span style="color:red;">Favor validar o Captcha!</span>';
            return false;
        }
        if (form.valid()) {
            form.submit();
        }
    })
});

function verifyCaptcha() {
    document.getElementById('g-recaptcha-error').innerHTML = '';
}