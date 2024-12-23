$(function () {
    "use strict";

    $("#btnFechar").click(function () {
        parent.$.fancybox.close();
    });

    $("#SearchProperty").change(function () {
        $('#SearchValue').val("");
        $('#SearchValue').unmask();

        if ($('#SearchProperty').val() !== '' && $('#SearchProperty').val() !== null && $('#SearchProperty').val() !== undefined) {
            var mask = $('#SearchProperty').val().split(";")[1].split(":")[1];
            if (mask !== undefined && mask !== "")
                $('#SearchValue').mask(mask);
        }
    });

    $('#SearchValue').keypress(function (event) {
        if ($('#SearchProperty').val() !== '' && $('#SearchProperty').val() !== null && $('#SearchProperty').val() !== undefined) {
            var prop = $('#SearchProperty').val().split(";")[2].split(":")[1];
            if (prop === "Inteiro") {
                if (event.which !== 8 && isNaN(String.fromCharCode(event.which))) {
                    event.preventDefault();
                }
                else if ($("#SearchValue").val().length > 8) {
                    event.preventDefault();
                }
            }
        }
    });

    $("#SearchValue").change(function () {
        var errordata = $(document).find("[data-valmsg-for='SearchValue']");
        errordata.html('');

        if ($('#SearchProperty').val() !== '' && $('#SearchProperty').val() !== null && $('#SearchProperty').val() !== undefined) {
            var prop = $('#SearchProperty').val().split(";")[2].split(":")[1];
            if (prop === "CPF") {
                if (!ValidarCPF($('#SearchValue').val())) {
                    errordata.html('<span id="SearchValue-error" class="">CPF inválido.</span>');
                }
            }
        }
    });
        
    $("#lnkBuscar").on("click", function (e) {
        var erro = $(document).find("[data-valmsg-for='SearchValue']").val();
        if (erro === null || erro === undefined || erro === "") {
            if ($('#SearchProperty').val() === "" || $('#SearchProperty').val() === undefined || $('#SearchProperty').val() === null) {
                $.alertError("Pesquisar", "Favor selecionar o tipo de Busca");
                $('#SearchProperty').focus();
            }
            else {
                CarregarTable($('#SearchProperty').val(), $('#SearchValue').val(), $('#SearchModel').val(), $('#Parametros').val());
            }
        }
    });

    CarregarTable($('#SearchProperty').val(), $('#SearchValue').val(), $('#SearchModel').val(), $('#Parametros').val());
});

function CarregarTable(prop, val, model, parametros) {
    $('#divGrid').jtable('load', {
        property: prop,
        value: val,
        model: model,
        parametros
    });
}

function closePopupMvc(value) {
    var elementid = $("#TargetElementId").val();
    parent.$("#" + elementid).val(value);
    parent.$("#" + elementid).blur();
    parent.$("#" + elementid).change();
    parent.$.fancybox.close();
}