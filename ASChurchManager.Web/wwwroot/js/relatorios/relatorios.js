$(function () {
    "use strict";

    if ($("#ViewModelTitle").val() == "PresencaLista") {
        $('select[name="duallistbox_cargos[]"]').bootstrapDualListbox({
            nonSelectedListLabel: 'Não Selecionados',
            selectedListLabel: 'Selecionados',
            preserveSelectionOnMove: 'moved',
            moveOnSelect: false
        });
        $('select[name="duallistbox_eventos[]"]').bootstrapDualListbox({
            nonSelectedListLabel: 'Não Selecionados',
            selectedListLabel: 'Selecionados',
            preserveSelectionOnMove: 'moved',
            moveOnSelect: false
        });
    }


    $("#DataFinal").blur(function () {
        var datainicial = $("#DataInicio").val();
        var datafinal = $("#DataFinal").val();

        var dtIni = Date.parse(datainicial);
        var dtFin = Date.parse(datafinal);

        var errorDataInicio = $(document).find("[data-valmsg-for='DataInicio']");
        errorDataInicio.html('');

        if (dtIni > dtFin) {
            errorDataInicio.html('<span id="DataInicio-error" class="">O campo Data Inicial é maior do que a Data Final</span>');
        }
    });

    //==================== CANDIDATOS PARA O BATISMO ===========================
    $("#btnRelCandidatoBatismo").click(function (event) {
        event.preventDefault();
        let batismoId = $('option:selected', $("#DataBatismo")).val();
        let dtBatismo = $('option:selected', $("#DataBatismo")).text();
        if (dtBatismo === "Todas") {
            batismoId = "0";
            dtBatismo = "0"
        }
        let congr = $('option:selected', $("#Congregacao")).val();
        let sit = $('option:selected', $("#Situacao")).text();
        let formato = $("#TipoSaida").val();

        let url = `${urlRelCadBat}?congregacao=${congr}&batismoId=${batismoId}&dataBatismo=${dtBatismo.replace("/", "-").replace("/", "-")}&situacao=${sit}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelCandidatoBatismo"));

    });

    //==================== ANIVERSARIANTES ===========================
    $("#btnRelAniversariantes").click(function (event) {
        event.preventDefault();

        let congr = $('option:selected', $("#Congregacao")).val();
        let dataIni = $("#DataInicio").val();
        let dataFin = $("#DataFinal").val();
        let tpmembro = $('option:selected', $("#TipoMembro")).val();
        let formato = $("#TipoSaida").val();

        let url = `${urlRelAniv}?congregacao=${congr}&dataInicio=${dataIni.replace("/", "-").replace("/", "-")}&dataFinal=${dataFin.replace("/", "-").replace("/", "-")}&tipoMembro=${tpmembro}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelAniversariantes"));
    });


    //==================== NASCIMENTOS ===========================
    $("#btnRelNascimentos").click(function (event) {
        event.preventDefault();

        let congr = $('option:selected', $("#Congregacao")).val();
        let dataIni = $("#DataInicio").val();
        let dataFin = $("#DataFinal").val();
        let formato = $("#TipoSaida").val();

        let url = `${urlRelNasc}?congregacao=${congr}&dataInicio=${dataIni.replace("/", "-").replace("/", "-")}&dataFinal=${dataFin.replace("/", "-").replace("/", "-")}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelNascimentos"));
    });

    //==================== CASAMENTO ===========================
    $("#btnRelCasamento").click(function (event) {
        event.preventDefault();

        let congr = $('option:selected', $("#Congregacao")).val();
        let dataIni = $("#DataInicio").val();
        let dataFin = $("#DataFinal").val();
        let formato = $("#TipoSaida").val();

        let url = `${urlCasam}?congregacao=${congr}&dataInicio=${dataIni.replace("/", "-").replace("/", "-")}&dataFinal=${dataFin.replace("/", "-").replace("/", "-")}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelCasamento"));
    });

    //==================== TRANSFERENCIA ===========================
    $("#btnRelTransferencia").click(function (event) {
        event.preventDefault();

        let congr = $("#Congregacao").val();
        let dataIni = $("#DataInicio").val();
        let dataFin = $("#DataFinal").val();
        let formato = $("#TipoSaida").val();

        let url = `${urlTrans}?congregacao=${congr}&dataInicio=${dataIni.replace("/", "-").replace("/", "-")}&dataFinal=${dataFin.replace("/", "-").replace("/", "-")}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelTransferencia"));
    });

    //==================== CONGREGACOES ===========================
    $("#btnRelCongregacao").click(function (event) {
        event.preventDefault();

        let congr = $("#Congregacao").val();
        let formato = $("#TipoSaida").val();

        let url = `${urlCongr}?congregacao=${congr}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelCongregacao"));

    });

    //==================== OBREIROS ===========================
    $("#btnRelObreiros").click(function (event) {
        event.preventDefault();

        let congr = $("#Congregacao").val();
        let formato = $("#TipoSaida").val();

        let url = `${urlRelObr}?congregacao=${congr}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelObreiros"));

    });

    $("#Congregacao").blur(function () {
        var value = $(this).val();
        if (value != null && value != undefined && value != "") {
            var errorSituacao = $(document).find("[data-valmsg-for='Congregacao']");
            errorSituacao.html('');
        };
    });
    $("#Mes").blur(function () {
        var value = $(this).val();
        if (value != null && value != undefined && value != "0" && value != "") {
            var errorSituacao = $(document).find("[data-valmsg-for='Mes']");
            errorSituacao.html('');
        };
    });
    $("#Ano").blur(function () {
        var value = $(this).val();
        if (value != null && value != undefined && value != "0" && value != "") {
            var errorSituacao = $(document).find("[data-valmsg-for='Ano']");
            errorSituacao.html('');
        };
    });

    //==================== RELATÓRIO MENSAL ===========================
    $("#btnRelatorioMensal").click(function (event) {
        event.preventDefault();

        var congr = $("#Congregacao").val();
        var mes = $('option:selected', $("#Mes")).val();
        var ano = $("#Ano").val();


        if (congr == null || congr == "") {
            var errorCongr = $(document).find("[data-valmsg-for='Congregacao']");
            errorCongr.html('<span id="Congregacao-error" class="">Congregação é de preenchimento obrigatório</span>');
            return false;
        }
        if (mes == null || mes == "0" || mes == "") {
            var errorMes = $(document).find("[data-valmsg-for='Mes']");
            errorMes.html('<span id="Mes-error" class="">Mês é de preenchimento obrigatório</span>');
            return false;
        }
        if (ano < 1950 || ano > 2050) {
            var errorAno = $(document).find("[data-valmsg-for='Ano']");
            errorAno.html('<span id="Ano-error" class="">Ano deve ser maior que 1950 e menor de 2050</span>');
            return false;
        }
        var url = "/Secretaria/Relatorios/RelatorioMensal?congregacao=" + congr + "&mes=" + mes + "&ano=" + ano;
        $.downloadArquivo(url, $("#btnRelatorioMensal"));
    });

    //==================== RELATÓRIO MEMBROS ===========================
    $("#btnRelMembro").click(function (event) {
        event.preventDefault();

        let congr = $("#Congregacao").val();
        let status = $('option:selected', $("#Status")).val();
        let tipomembro = $('option:selected', $("#TipoMembro")).val();
        let estadocivil = $('option:selected', $("#EstadoCivil")).val();
        let abedabe = $("#ABEDABE").is(":checked");
        let completo = $("#Completo").is(":checked");
        let filtrarConf = $("#Confirmados").prop("checked");
        let ativosConf = $("#AtivosConfirmados").is(":checked");
        let formato = $("#TipoSaida").val();

        if (congr === "" && status === "NaoDefinido" && tipomembro === "NaoDefinido" && estadocivil === "NaoDefinido") {
            $.alertError("Relatório de Membros", "Favor selecionar pelo menos uma Opção: Congregação/Status/Tipo de Membro/Estado Civil.");
        }
        else {
            if (filtrarConf == false)
                ativosConf = false;

            let url = `${urlRelMembros}?congregacaoId=${congr}&status=${status}&tipoMembro=${tipomembro}&estadocivil=${estadocivil}&simplificado=${completo}&abedabe=${abedabe}&filtrarConf=${filtrarConf}&ativosConf=${ativosConf}&formato=${formato}`
            $.downloadArquivo(url, $("#btnRelMembro"));
        }

    });

    $("#btnRelCursosMembro").click(function (event) {
        event.preventDefault();

        let congr = $('option:selected', $("#Congregacao")).val();
        let cursos = $('option:selected', $("#Cursos")).val();
        let formato = $("#TipoSaida").val();

        let url = `${urlRelCur}?congregacaoId=${congr}&cursoId=${cursos}&formato=${formato}`;
        $.downloadArquivo(url, $("#btnRelCursosMembro"));
    });

    //==================== EVENTO ===========================
    $("#btnEventos").click(function (event) {
        event.preventDefault();

        var congr = $("#Congregacao").val();
        var mes = $('option:selected', $("#Mes")).val();
        var ano = $("#Ano").val();
        var tipoevento = $("#TipoEvento").val();
        var completo = $("#Completo").is(":checked");

        if (ano < 1950 || ano > 2050) {
            var errorAno = $(document).find("[data-valmsg-for='Ano']");
            errorAno.html('<span id="Ano-error" class="">Ano deve ser maior que 1950 e menor de 2050</span>');
            return false;
        }
        var url = `/Secretaria/Relatorios/RelatorioEventos?congregacao=${(congr != "" ? congr : "0")}&mes=${(mes != "" ? mes : "0")}&ano=${ano}&tipoEvento=${(tipoevento != "" ? tipoevento : "0")}&simplificado=${completo}`;
        $.downloadArquivo(url, $("#btnEventos"));
    });


    //==================== PRESENCA ===========================
    $("#PresencaId").change((e) => {
        e.preventDefault;

        if ($("#DataId").length) {
            let idPresenca = $('option:selected', $("#PresencaId")).val();

            let requestData = {
                idPresenca
            }

            $.ajaxPost(urlBuscarDatas, requestData, function (ret) {
                if (ret.status == "OK") {
                    let itens = ret.data;

                    $("#DataId").empty();
                    let option = $("<option></option>").appendTo($('#DataId'));
                    option.attr("value", "");
                    option.html("Todas");

                    for (var i = 0; i < itens.length; i++) {
                        option = $("<option></option>").appendTo($('#DataId'));
                        option.attr("value", itens[i].Id);
                        let descr = `Data: ${FormataData(itens[i].DataHoraInicio)} - Hora Início: ${FormataHora(itens[i].DataHoraInicio)} - Hora Fim: ${FormataHora(itens[i].DataHoraFim)}`;
                        option.html(descr);
                    }
                }
                else {
                    $.bsMessageError("Atenção", ret.Message);
                }
            });
        }
    });

    $("#btnRelPresInscritos").click((e) => {
        e.preventDefault();

        var idPresenca = $('option:selected', $("#PresencaId")).val();
        var congregacao = $("#CongregacaoId").val();
        if (congregacao == null || congregacao == "")
            congregacao = $('option:selected', $("#CongregacaoId")).val();
        var tipo = $('option:selected', $("#TipoInscricao")).val();
        var carimbo = $("#Carimbo").is(":checked");

        var url = `${urlRelInscr}?idPresenca=${idPresenca}&congregacao=${congregacao}&tipo=${tipo}&carimbo=${carimbo}`;

        $.downloadArquivo(url, $("#btnRelPresInscritos"));
    });

    $("#btnRelPresFreq").click((e) => {
        e.preventDefault();

        let idPresenca = $('option:selected', $("#PresencaId")).val();
        if (idPresenca == 0) {
            $.alertError("Relatório de Frequência", "Favor selecionar um Curso/Evento!");
            return;
        }
        let idData = $('option:selected', $("#DataId")).val();
        let congregacao = $('option:selected', $("#CongregacaoId")).val();
        let situacao = $('option:selected', $("#SituacaoSelecionado")).val();
        let formato = $("#TipoSaida").val();
        var url = `${urlRelFreq}?idPresenca=${idPresenca}&idData=${idData}&congregacao=${congregacao}&situacao=${situacao}&formato=${formato}`;

        $.downloadArquivo(url, $("#btnRelPresFreq"));
    });

    $("#btnRelPresLista").click((e) => {
        e.preventDefault();
        let msg = [];
        let erroDtIni = $(document).find("[data-valmsg-for='DataInicio']");
        let erroDtFin = $(document).find("[data-valmsg-for='DataFinal']");
        let erroData = $(document).find("[data-valmsg-for='DataFinal']");

        erroDtIni.html("");
        erroDtFin.html("");
        erroData.html("");

        if ($("#DataInicio").val() == "" || $("#DataFinal").val() == "") {
            if ($("#DataInicio").val() == "") {
                erroDtIni.html("Data Inicio é de preenchimento obrigatório.");
                msg.push("Data Inicio é de preenchimento obrigatório.")
            }
            if ($("#DataFinal").val() == "") {
                erroDtFin.html("Data Final é de preenchimento obrigatório.");
                msg.push("Data Final é de preenchimento obrigatório.")
            }
        }
        else {
            let dtI = $("#DataInicio").val().split("/");
            let dtIni = new Date(dtI[2], dtI[1] - 1, dtI[0]);

            let dtF = $("#DataFinal").val().split("/");
            let dtFin = new Date(dtF[2], dtF[1] - 1, dtF[0]);
            if (dtIni >= dtFin) {
                msg.push("Data Final deve ser maior que a Data Inicial.")
                erroData.html("Data Final deve ser maior que a Data Inicial");
            }
        }

        if ($('[name="duallistbox_cargos[]"]').val().length <= 0) {
            msg.push("Deve ser selecionado pelo menos um Cargo.")
        }

        if ($('[name="duallistbox_eventos[]"]').val().length <= 0) {
            msg.push("Deve ser selecionado pelo menos um Tipo de Evento.")
        }

        if (msg.length > 0) {
            let text = msg.join("<br />");
            $.alertError("Relatório", text);
        }
        else {
            let congregacao = $('option:selected', $("#CongregacaoId")).val();
            let dataIni = $("#DataInicio").val();
            let dataFin = $("#DataFinal").val();
            let cargos = $('[name="duallistbox_cargos[]"]').val().join("_");
            let eventos = $('[name="duallistbox_eventos[]"]').val().join("_");

            let url = `${urlRelFreqLista}?congregacao=${congregacao}&datainicio=${dataIni}&datafinal=${dataFin}&cargos=${cargos}&eventos=${eventos}`;
            $.downloadArquivo(url, $("#btnRelPresLista"));
        }

    });

    $('#Confirmados').on('ifChanged', function (e) {
        e.preventDefault;

        if ($("#Confirmados").prop("checked") == true) {
            $("#TipoMembro").val("Membro");
            $("#TipoMembro").attr("disabled", true);
            $("#Status").val("Ativo");
            $("#Status").attr("disabled", true);
            $("#divConf").show();
        }
        else {
            $("#TipoMembro").val("");
            $("#TipoMembro").removeAttr("disabled");
            $("#Status").val("");
            $("#Status").removeAttr("disabled");
            $("#divConf").hide();
        }
    });
});

function FormataData(datahora) {
    datahora = datahora.split('T');
    let data = datahora[0];
    let newData = data.split('-');
    return `${newData[2]}/${newData[1]}/${newData[0]}`;
}

function FormataHora(datahora) {
    datahora = datahora.split('T');
    return `${datahora[1]}`;
}
