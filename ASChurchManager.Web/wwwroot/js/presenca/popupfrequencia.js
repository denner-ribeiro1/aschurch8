
$(() => {
    "use strict";

    onScan.attachTo(document);

    $(document).on('scan', (scanCode, quantity) => {
        
        let code = scanCode.originalEvent.detail.scanCode.replaceAll("\\", "");
        code = code.replaceAll("000026", "");

        var form = $('#frmPresenca');
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        let requestData = {
            __RequestVerificationToken: token,
            code
        }
        onScan.detachFrom(document);
        $.ajaxPost(urlConfirmarPresenca, requestData, function (ret) {
            if (ret.status == "OK") {
                Swal.fire({
                    title: 'Presença Confirmada!',
                    icon: 'success',
                    showConfirmButton: false,
                    html: `<br /><br />Evento: <b>${ret.evento}</b> <br />Data: <b>${ret.dtEvento}</b><br />Nome: <b>${ret.nome}</b> <br /><br />`,
                    timer: 2000,
                    timerProgressBar: true,
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    allowEnterKey: false,
                    stopKeydownPropagation: false,
                }).then((result) => {
                    onScan.attachTo(document);
                })
            }
            else {
                Swal.fire({
                    title: 'Atenção!',
                    icon: 'error',
                    showConfirmButton: false,
                    html: `<br /><br /><b>${ret.Message}</b> <br /><br />`,
                    timer: 2000,
                    timerProgressBar: true,
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    allowEnterKey: false,
                    stopKeydownPropagation: false,
                }).then((result) => {
                    onScan.attachTo(document);
                })
            }
        });


    })

})