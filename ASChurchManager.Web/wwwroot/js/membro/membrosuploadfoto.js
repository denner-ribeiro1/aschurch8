var dom;
$(function () {
    "use strict";
    /**      UPLOAD - INICIO     */
    dom = {
        uploader: $("#uploader"),
        uploads: $("#divFoto"),
        fileDescription: $("#arquivoDescricao"),
        fotoBorder: $("#fotoBorder"),
        removeFile: $("#removeFile"),
        hdnFoto: $("#Pessoa_FotoPath")
    };

    removerBordaFoto();

    var uploader = new plupload.Uploader({
        runtimes: "html5",
        url: pathUrl,
        drop_element: "uploader",
        browse_button: "selectFiles",
        container: "uploader",
        max_file_size: '1024mb',
        resize: { width: 180, height: 230, quality: 90 },
        filters: [{ title: "Arquivo de Imagem", extensions: "jpg,png" }]
    });

    uploader.bind("QueueChanged", function (uploader) {
        if (uploader.files.length > 1) {
            uploader.files.splice(0, 1);
        }

        for (var i = 0; i < uploader.files.length; i++) {
            showImagePreview(uploader.files[i]);

            setTimeout(function () {
                var imgValue = $(".arquivoFoto").attr("src");

                if (imgValue != undefined) {
                    removerBordaFoto();
                }

                $(dom.hdnFoto).val(imgValue);
            }, 300);
        }
    });

    uploader.init();

    $(dom.removeFile).on("click", function () {
        if (uploader.files.length > 1) {
            uploader.files.splice(0, 1);
        }
        removerFoto();
        $(this).css("display", "none");
    });
    /**      UPLOAD - FIM     */
});

function showImagePreview(file) {
    $(dom.uploads).html('');
    $(dom.fileDescription).html('');

    var image = $(new Image()).appendTo(dom.uploads);

    // https://github.com/moxiecode/moxie/wiki/Image
    var preloader = new mOxie.Image();

    preloader.onload = function () {
        preloader.downsize(180, 230); // redimensiona a imagem

        image.prop("src", preloader.getAsDataURL());
        image.prop("id", "imgFoto");
        image.prop("class", "arquivoFoto");
        image.prop("alt", "Foto");
    };

    preloader.load(file.getSource());

    // Descarrega o preview da foto
    setTimeout(function () {
        var imgValue = $(".arquivoFoto").attr("src");

        if (imgValue == undefined) {
            removerFoto();
            preloader.destroy(); // descarrega o blob da imagem

            // Recria os atributos do componente image
            image.prop("id", "imgFoto");
            image.prop("class", "arquivoFoto");
            image.prop("alt", "Foto");
            image.prop("src", "");

            $.bsMessageWarning("Atenção", "Formato de arquivo inválido!");
            return;
        }
    }, 300);

    $(dom.fileDescription).append('<div id="' + file.id + '">' + file.name + ' (' + plupload.formatSize(file.size) + ') <b></b>' + '</div>');
}
function removerBordaFoto() {
    if ($("#imgFoto").attr("src") != undefined && $("#imgFoto").attr("src") != "") {
        $(dom.fotoBorder).css("border", "0px");
        $(dom.removeFile).css("display", "block");
    }
}
function removerFoto() {
    $("#imgFoto").attr("src", "");
    $("#imgFoto").attr("alt", "Foto");
    $(dom.hdnFoto).val("");
    $(dom.fotoBorder).css("border", "1px solid black");
}
