var listaMembro = [];
var files;


$(() => {
    "use strict";

    $('#Arquivo').change((e) => {
        e.preventDefault;
        var extPermitidas = ['xls', 'xlsx'];
        var extArquivo = $('#Arquivo').val().split('.').pop();

        if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
            $.alertError('Upload de Arquivo', 'Somente arquivos do Tipo Excel são permitidos para Upload.');
            $('#Arquivo').val('');
        }
        else {
            files = e.target.files;
        }
    });

    $("#btnCapturar").click((e) => {
        let arquivo = $('#Arquivo').val();
        if (arquivo == null || arquivo == "") {
            $.bsMessageError("Atenção", "Favor selecionar o arquivo para Capturar!");
            $("#Arquivo").focus();
            return;
        }
        var extPermitidas = ['xls', 'xlsx'];
        var extArquivo = arquivo.split('.').pop();

        if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
            $.alertError('Upload de Arquivo', 'Somente arquivos do Tipo Excel são permitidos para Upload.');
            $('#Arquivo').val('');
        }
        else {
            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);

            var formData = new FormData();

            formData.append("files", files[0]);
            formData.append("id", $("#Presenca").val());

            $.ajax({
                type: "POST",
                url: urlCarregarArquivo,
                contentType: false,
                processData: false,
                data: formData,
                success: function (result) {
                    if (result.status == "OK") {
                        listaMembro = result.data;
                        let url = `${urlPopUp}?id=${$("#Presenca").val()}&arquivo=${arquivo}`;
                        $.popUpUrl(url, 600, 800);
                    }
                    else {
                        $.alertError('Captura de Arquivo', result.mensagem);
                    }
                },
                complete: function (param) {
                    removerOverlay(null);
                }
            });
        }
    })

    $("#btnCancelar").click((e) => {
        $('#Arquivo').val("");
        files = null;
    });
});
