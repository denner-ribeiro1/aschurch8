$(function () {
    if ($("#Tipo").val() === "Evento") {
        var dataIni = $("#DataInicio").val();
        if (dataIni === null || dataIni === undefined || dataIni === "") {
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!

            var yyyy = today.getFullYear();
            if (dd < 10) {
                dd = '0' + dd;
            }
            if (mm < 10) {
                mm = '0' + mm;
            }
            var dataAtual = dd + '/' + mm + '/' + yyyy;

            $("#DataInicio").val(dataAtual);
            $("#DataInicio").removeAttr('readonly');
            //$('#DataInicio').datepicker({
            //    format: "dd/mm/yyyy",
            //    todayHighlight: true
            //}).on('changeDate', function (ev) {
            //    $(this).datepicker('hide');
            //});
        }
    }

    if ($("#TipoFrequenciaSelecionado").val() !== "1") {
        $("#quantidade").show();
    }
    else {
        $("#quantidade").hide();
    }

    $("#TipoFrequenciaSelecionado").change(function () {
        if ($(this).val() !== "1") {
            if ($(this).val() === "4") {
                var DataEvento = $("#DataInicio").val();
                var dia = DataEvento.split("/")[0];
                if (dia > 28) {
                    $.alert("Aviso", "Para os meses que não possuem o dia escolhido(" + dia + "), o Evento será agendado sempre no último dia de cada mês!");
                }
            }
            $("#quantidade").show();
        }
        else {
            $("#quantidade").hide();
        }
    });

    $("#lnkBuscarCongregacao").removeAttr("class");
    var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
    if (usuarioCongregacaoSede === false) {
        $("#CongregacaoId").attr("readonly", "readonly");
        $("#lnkBuscarCongregacao").attr("class", "readonly");
    } else {
        $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
    }

    $("#CongregacaoId").blur(function () {
        PesquisarCongregacao($("#CongregacaoId").val(), "CongregacaoNome");
        if ($("#CodigoCongregacaoSede").val() !== $("#CongregacaoId").val()) {
            $("#divCheck").hide();
        }
        else {
            $("#divCheck").show();
        }
    });


    $("#HoraFinal").blur(function () {
        var Congregracao = $("#CongregacaoId").val();
        var DataInicio = $("#DataInicio").val();
        var DataFinal = $("#DataInicio").val();
        var HoraInicio = $("#HoraInicio").val();
        var HoraFim = $(this).val();

        if (Congregracao === undefined || Congregracao === 0) {
            $.bsMessageError("Atenção", "Congregação é de preenchimento obrigatório.");
            return;
        }
        else if (HoraInicio === undefined || HoraInicio === "") {
            $.bsMessageError("Atenção", "Hora do Início do Evento é de preenchimento obrigatório.");
            return;
        }
        else if (HoraFim === undefined || HoraFim === "") {
            $.bsMessageError("Atenção", "Hora do Término do Evento é de preenchimento obrigatório.");
            return;
        }
        else {
            var dataArray = DataInicio.split("/");
            var newDataInicio = dataArray[1] + "/" + dataArray[0] + "/" + dataArray[2];

            var dataArrayF = DataFinal.split("/");
            var newDataFinal = dataArrayF[1] + "/" + dataArrayF[0] + "/" + dataArrayF[2];

            var stt = new Date(newDataInicio + " " + HoraInicio);
            stt = stt.getTime();

            var endt = new Date(newDataFinal + " " + HoraFim);
            endt = endt.getTime();

            if (stt > endt) {
                $.bsMessageError("Atenção", "Hora do Início é maior que a Hora Final do Evento.");
                $("#HoraInicio").val("");
                $("#HoraFinal").val("");
            }
        }
    });

    $("#lnkFechar").click(function () {
        parent.$.fancybox.close();
    });

    $("#lnkSalvar").click(function (e) {
        var msg = "Deseja realmente criar um novo Evento?";
        if ($("#Acao").val() === "Update") {
            msg = "Deseja realmente atualizar o Evento?";
        }

        var requestData = {
            dataInicial: $("#DataInicio").val() + " " + $("#HoraInicio").val(),
            dataFinal: $("#DataInicio").val() + " " + $("#HoraFinal").val(),
            congregacao: $("#CongregacaoId").val(),
            frequencia: $("#TipoFrequenciaSelecionado").val(),
            quantidade: $("#Quantidade").val()
        };

        $.ajaxPost(urlValDatas, requestData, function (data) {
            e.preventDefault;
            if (data.status === "OK") {
                if (data.msg !== "" && data.msg !== undefined) {
                    $.showYesNoAlert(data.msg, "Eventos", function () {
                        SalvarEvento(msg);
                    }, null, function () {
                        $("#HoraInicio").val("");
                        $("#HoraFinal").val("");
                    });
                }
                else {
                    SalvarEvento(msg);
                }
            }
            else {
                $.bsMessageError("Atenção", data.msg);
            }
        });
    });

    $("#lnkExcluir").click(function (e) {
        e.preventDefault;
        var tipoFreq = parseInt($("#TipoFrequenciaSelecionado").val());
        var qtd = parseInt($("#Quantidade").val());
        var id = parseInt($("#Id").val());

        if (tipoFreq > 1 && qtd > 0) {
            $.showYesNoAlert('<h4 style="color:red">Existe(m) ' + qtd + ' evento(s) vinculado(s) ao Evento a ser excluído.<br></br>Deseja remover todos os Eventos vinculados?</h4>', "Eventos",
                function () {
                    ExcluirEvento(id, true);
                }, null,
                function () {
                    ExcluirEvento(id, false);
                });
        }
        else {
            ExcluirEvento(id, false);
        }
    });
});

function SalvarEvento(msg) {
    $.showYesNoAlert(msg, "Eventos", function () {
        var form = $("#frmEventos");
        //form.data("validator").settings.ignore = "";
        if (form.valid()) {
            $('<div id="div-loading"><div></div></div>').appendTo('body');
            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: form.serialize(), // serializes the form's elements.
                success: function (result) {
                    if (result.status == "OK") {
                        parent.$.fancybox.close();
                    }
                    else {
                        $.alertError("Membro", result.msg);
                    }
                },
                complete: function (param) {
                    $('#div-loading').remove();
                }
            });

        }
    });
}

function PesquisarCongregacao(valor, campo) {
    if (valor > 0) {
        var requestData = {
            CongregacaoId: valor
        };
        campo = "#" + campo;
        $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (cong) {
            $(campo).val("");
            if (cong.Id == 0) {
                $.bsMessageError("Atenção", "Código inválido!");
                $(this).val('');
            } else {
                $(campo).val(cong.Nome);
            }
        });
    }
}

function ExcluirEvento(id, excluirVinc) {
    request = {
        id: id,
        excluirVinc: excluirVinc
    };

    $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Evento?<br></br>Após a confirmação NÃO será mais possível recuperar o Evento.</h3>', "Eventos",
        function () {
            $.ajaxPost(urlExcluir, request, function (response) {
                if (response.status == "OK") {
                    parent.$.fancybox.close();
                }
                else {
                    $.alertError("Erro", response.msg);
                }
            });
        });
}