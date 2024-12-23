let posicao = "";
let chbSelId = '';
let margem = {}

$(() => {
    "use strict";
    $('.posicao').iCheck('destroy');

    $("#TipoInscricao").change((e) => {
        e.preventDefault;

        let tipo = $('option:selected', $("#TipoInscricao")).val();
        if (tipo == "Membro") {
            $("#divMembro").show();
            $("#divNaoMembro").hide();
        }
        else if (tipo == "NaoMembro") {
            $("#divMembro").hide();
            $("#divNaoMembro").show();
        }
        else {
            $("#divMembro").hide();
            $("#divNaoMembro").hide();
        }
    })

    var paramsMembro = {
        TipoMembro: 3,
        Status: 1
    };
    $("#lnkMembroId").popUp("Pesquisar Membro", "MembroObreiros", "MembroId", paramsMembro);

    $("#Cpf").change(function (e) {
        e.preventDefault;
        let value = $("#Cpf").val();
        if (value != "") {
            PesquisarMembro();
        }
    });

    $("#MembroId").change(function (e) {
        e.preventDefault;
        var value = $("#MembroId").val();
        if (value > 0) {
            PesquisarMembro();
        }
    });


    $('#chbSelEtiq').on('ifChanged', function (e) {
        e.preventDefault;
        $('input[id^="chb_"]').each((i, chb) => {
            $(chb).prop("checked", false);
        })
        $("#chbSelEtiq").prop("checked") == true ? $("#divPosInicial").show() : $("#divPosInicial").hide();
    });

    $('.posicao').on("click", function (e) {
        if (chbSelId != $(e.currentTarget).attr('id')) {
            e.preventDefault;
            let chbSel = $(this).attr("id");
            if ($(e.currentTarget).prop("checked"))
                posicao = $(this).val();
            else
                posicao = '';

            $('input[id^="chb_"]').each((i, chb) => {
                if ($(chb).attr("id") != chbSel) {
                    $(chb).prop("checked", false);
                }
            })
        }
    });

    $("#btnImprimir").click(function (e) {
        e.preventDefault();

        let pres = $('option:selected', $("#PresencaId")).val();
        let congr = $('option:selected', $("#CongregacaoId")).val();
        let tipo = $('option:selected', $("#TipoInscricao")).val();

        let valor = "";
        if (tipo == "Membro") {
            valor = $("#MembroId").val();
        } else if (tipo == "NaoMembro") {
            valor = $("#Cpf").val();
        }

        if (pres == null || pres == undefined || pres == "") {
            $.bsMessageError("Atenção", "Favor selecionar o Curso/Evento!");
            return
        }

        let url = `${urlEtiquetas}?idPresenca=${pres}&congregacao=${congr}&tipo=${tipo}&valor=${valor}`;
        if ($("#chbSelEtiq").prop("checked") && posicao != null && posicao != "") {
            url += `&posicao=${posicao}`;
        }
        if (margem != null && (margem.margemTop > 0 || margem.margemEsq > 0)) {
            const { margemTop, margemEsq } = margem;

            if (margemTop > 0)
                url += `&margemTop=${margemTop}`;

            if (margemEsq > 0)
                url += `&margemEsq=${margemEsq}`;
        }
        $.downloadArquivo(url, $("#btnImprimir"));

    });


    $("#btnConfig").click((e) => {
        (async () => {

            const { value: formValues } = await Swal.fire({
                title: 'Ajustes Margens',
                html:
                    `<label for="margemTop" class="control-label">Acima</label>
                    <input id="margemTop" type="number" class="form-control" ${(margem.margemTop > 0 ? `value="${margem.margemTop}"` : '')} oninput="this.value = this.value.replace(/[^0-9]/g, '');">
                    <br />
                    <label for="margemEsq" type="number" class="control-label">Esquerda</label>
                    <input id="margemEsq" type="number" class="form-control" ${(margem.margemEsq > 0 ? `value="${margem.margemEsq}"` : '')} oninput="this.value = this.value.replace(/[^0-9]/g, '');">`,
                focusConfirm: false,
                showCancelButton: true,
                confirmButtonText: '<i class="glyphicon glyphicon-floppy-saved"></i> Salvar',
                cancelButtonText: '<i class="glyphicon glyphicon-home"></i> Fechar',
                preConfirm: () => {
                    let mrg = {
                        margemTop: document.getElementById('margemTop').value,
                        margemEsq: document.getElementById('margemEsq').value
                    }
                    if (mrg.margemTop > 100) {
                        Swal.showValidationMessage(
                            `A Margem Acima deve ser menor que 100`
                        );
                        return false;
                    }

                    if (mrg.margemEsq > 100) {
                        Swal.showValidationMessage(
                            `A Margem Esquerda deve ser menor que 100`
                        );
                        return false;
                    }
                    return mrg;
                }
            })

            if (formValues) {
                margem = formValues;
            }

        })()
    })
});

function PesquisarMembro() {
    let presencaId = $('option:selected', $("#PresencaId")).val();
    let tipo = $('option:selected', $("#TipoInscricao")).val();

    let requestData = {};

    if (tipo == "Membro") {
        requestData = {
            presencaId,
            tipo,
            valor: $("#MembroId").val()
        };
    }
    else if (tipo == "NaoMembro") {
        requestData = {
            presencaId,
            tipo,
            valor: $("#Cpf").val()
        };
    }

    $.ajaxPost(urlBuscarMembro, requestData, function (ret) {
        if (ret.status == "OK") {
            let membro = ret.data;
            if (membro == null || membro == undefined || membro.Id == 0) {
                $.bsMessageError("Atenção", "Membro não cadastrado para o Curso/Evento!");
                $("#MembroId").val("");
                $("#Cpf").val("");
                $("#NomeMembro").val("");
                $("#NomeCpf").val("");
            }
            else {
                if (tipo == "Membro") {
                    $("#NomeMembro").val(membro.Nome);
                }
                else if (tipo == "NaoMembro") {
                    $("#NomeCpf").val(membro.Nome);
                }

            }
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });
}