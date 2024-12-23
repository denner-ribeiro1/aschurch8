//$.validator.defaults.ignore = "";

let indexData = -1;
let isreadonly = false;
var files;

$(() => {

    "use strict";

    $("#Valor").maskMoney({ prefix: '', allowNegative: false, thousands: '.', decimal: ',', affixesStay: false });

    isreadonly = $("#IsReadOnly").val().toLowerCase() == "true";

    if (datas == null) {
        datas = [];
    }

    if (isreadonly) {
        $("#divDatas").hide();
        $("#divDatas").attr('disabled', 'disabled');
    }
    else {
        $("#divDatas").show();
        $("#divDatas").removeAttr('disabled');
    }

    CarregarArquivos(arquivos, $("#Acao").val() == "Update");

    var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");

    // ----------------------------------------- Congregação
    $("#lnkBuscarCongregacao").removeAttr("class");

    if (usuarioCongregacaoSede === false) {
        $("#CongregacaoId").attr("readonly", "readonly");
        $("#lnkBuscarCongregacao").attr("class", "readonly");
    }
    else {
        $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
    }

    HabilitaInscricaoAutom($("#CongregacaoId").val() == "1");

    MontarGridDatas(datas);
    HabilitaBotoesAtualAdic(false);

    $("#CongregacaoId").change((e) => {
        e.preventDefault;
        var value = $("#CongregacaoId").val();
        if (value > 0) {
            var requestData = {
                CongregacaoId: value
            };

            $.ajaxPost(urlBuscarCongregacao, requestData, function (congregacao) {
                $("#CongregacaoNome").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código inválido!");
                    $(this).val('');
                }
                else if (congregacao.Id > 0) {
                    $("#CongregacaoNome").val(congregacao.Nome);
                    HabilitaInscricaoAutom($("#CongregacaoId").val() == "1");
                }
            });
        }
    });

    $("#HoraFinal").blur((e) => {
        e.preventDefault;
        ValidarHoras();
    });

    $("#btnAddData").click((e) => {
        e.preventDefault;
        SalvarAtualizar();
    })

    $("#btnUpdData").click((e) => {
        e.preventDefault;
        SalvarAtualizar()
    })

    $("#btnCancData").click((e) => {
        e.preventDefault();
        $("#Data").val(RetornaDataAtual());
        $("#HoraInicio").val("");
        $("#HoraFinal").val("");
        indexData = -1;
        HabilitaBotoesAtualAdic(false);
        if (!isreadonly)
            HabilitarBotoesTabela($(document.getElementById("tbDatas").getElementsByTagName("tbody")), false);
    });

    $("#btnSalvar").click((e) => {
        e.preventDefault();

        $.showYesNoAlert("Deseja realmente salvar um Evento/Curso?", "Controle de Presença em Eventos/Cursos", function () {
            let form = $("#frmPresenca");
            let url = form.attr('action');

            form.validate();
            if (form.valid()) {
                $("#DatasJSON").val(JSON.stringify(datas));
                $.ajaxPostForm(url, $("#frmPresenca"), function (result) {
                    if (result.status === "OK") {
                        window.location = result.url;
                    }
                    else {
                        $.alertError("Controle de Presença em Eventos/Cursos", result.msg);
                    }
                });
            }
            else {
                var msg = ValidarFormulario();
                if (msg != null && msg != undefined && msg != "")
                    $.alertError("Controle de Presença em Eventos/Cursos", msg);
            }
        });
    });

    $('#lnkExcluir').click((e) => {
        e.preventDefault();
        var request = { id: $("#Id").val() };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Evento/Curso?</br>Após a confirmação todos os dados referentes ao Evento/Cursos serão perdidos.</h3>', "Controle de Presença em Eventos/Cursos",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Controle de Presença em Eventos/Cursos", response.mensagem);
                    }
                });
            });
    });

    $('#Arquivo').change((e) => {
        if ($('#Arquivo').val() != "") {
            e.preventDefault;
            var extPermitidas = ['xls', 'xlsx', 'doc', 'docx', 'pdf', 'zip'];
            var extArquivo = $('#Arquivo').val().split('.').pop();

            if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
                $.alertError('Upload de Arquivo', 'Somente arquivos dos Tipos Excel, Word, PDF e ZIP são permitidos para Upload.');
                $('#Arquivo').val('');
            }
            else {
                files = e.target.files;
            }
        }
    });

    $("#btnIncluir").click((e) => {
        let arquivo = $('#Arquivo').val();
        let tamanhoMax = 7340032;

        if (arquivo == null || arquivo == "") {
            $.bsMessageError("Atenção", "Favor selecionar o arquivo para Capturar!");
            $("#Arquivo").focus();
            return;
        }
        if (files != null && files != undefined && files.length > 0 && files[0].size > tamanhoMax) {
            $.bsMessageError("Atenção", `Arquivo maior que 7 MB. Tamanho do arquivo selecionado: ${bytesToSize(files[0].size)}`);
            $("#Arquivo").focus();
            return;
        }

        var extPermitidas = ['xls', 'xlsx', 'doc', 'docx', 'pdf', 'zip'];
        var extArquivo = arquivo.split('.').pop();


        if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
            $.alertError('Upload de Arquivo', 'Somente arquivos dos Tipos Excel, Word, PDF e ZIP são permitidos para Upload.');
            $('#Arquivo').val('');
        }
        else {
            let requestData = {
                idPresenca: $("#Id").val(),
                nomeArquivo: files[0].name
            }

            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);

            $.ajax({
                cache: false,
                type: "POST",
                url: urlArqJaExiste,
                contentType: 'application/x-www-form-urlencoded',
                dataType: "json",
                data: requestData,
                error: function (xhr, status, error) {
                    console.log(error);
                    console.log(xhr.responseText);
                },
                success: function (ret) {
                    if (ret.status == "OK") {
                        let msg = ret.data;
                        if (msg == true) {
                            removerOverlay(null);
                            $.showYesNoAlert(`O Arquivo ${requestData.nomeArquivo} já existe. Deseja sobrepor o arquivo?`, "Arquivo já existe", function () {
                                var formData = new FormData();

                                formData.append("files", files[0]);
                                formData.append("idPresenca", $("#Id").val());

                                var overlay = gerarOverlay();
                                var loadingFrame = gerarLoadingFrame();

                                overlay.append(loadingFrame);
                                $('body').append(overlay);

                                setTimeout(function () {
                                    overlay.focus();
                                }, 0);


                                $.ajax({
                                    type: "POST",
                                    url: urlUplArq,
                                    contentType: false,
                                    processData: false,
                                    data: formData,
                                    error: function (xhr, status, error) {
                                        console.log(error);
                                        console.log(xhr.responseText);
                                    },
                                    success: function (result) {
                                        if (result.status == "OK") {
                                            arquivos = result.data;
                                            CarregarArquivos(arquivos, $("#Acao").val() == "Update");
                                            $('#Arquivo').val('');
                                        }
                                        else {
                                            $.alertError('Upload de Arquivo', result.mensagem);
                                        }
                                    },
                                    complete: function (param) {
                                        removerOverlay(null);
                                    }
                                });
                            });
                        }
                        else {
                            removerOverlay(null);
                            var formData = new FormData();

                            formData.append("files", files[0]);
                            formData.append("idPresenca", $("#Id").val());

                            var overlay = gerarOverlay();
                            var loadingFrame = gerarLoadingFrame();

                            overlay.append(loadingFrame);
                            $('body').append(overlay);

                            setTimeout(function () {
                                overlay.focus();
                            }, 0);


                            $.ajax({
                                type: "POST",
                                url: urlUplArq,
                                contentType: false,
                                processData: false,
                                data: formData,
                                error: function (xhr, status, error) {
                                    console.log(error);
                                    console.log(xhr.responseText);
                                },
                                success: function (result) {
                                    if (result.status == "OK") {
                                        arquivos = result.data;
                                        CarregarArquivos(arquivos, $("#Acao").val() == "Update");
                                        $('#Arquivo').val('');
                                    }
                                    else {
                                        $.alertError('Upload de Arquivo', result.mensagem);
                                    }
                                },
                                complete: function (param) {
                                    removerOverlay(null);
                                }
                            });
                        }
                    }
                    else {
                        removerOverlay(null);
                        $.bsMessageError("Atenção", ret.Message);
                    }
                }
            });
        }
    })

    $("#btnCancelar").click((e) => {
        $('#Arquivo').val("");
        files = null;
    });
})

function SalvarAtualizar() {
    if (ValidarHoras()) {
        if (indexData > -1) {
            let id = datas[indexData].Id;

            editedData = {
                Acao: id > 0 ? 2 : 0,
                Id: id,
                Data: $("#Data").val(),
                HoraInicio: $("#HoraInicio").val(),
                HoraFinal: $("#HoraFinal").val(),
            }
            datas = datas.map(d => d.Id !== id ? d : editedData);
        }
        else {
            datas.push({
                Acao: 0,
                Id: 0,
                Data: $("#Data").val(),
                HoraInicio: $("#HoraInicio").val(),
                HoraFinal: $("#HoraFinal").val(),
            });
        }

        datas.sort(function (a, b) {
            let data1Array = a.Data.split("/");
            let data2Array = b.Data.split("/");

            let newData1 = data1Array[1] + "/" + data1Array[0] + "/" + data1Array[2];
            let newData2 = data2Array[1] + "/" + data2Array[0] + "/" + data2Array[2];
            return new Date(newData1 + " " + a.HoraInicio) - new Date(newData2 + " " + b.HoraInicio);
        });
        MontarGridDatas();
        indexData = -1;
        HabilitaBotoesAtualAdic(false);
        $("#Data").val(RetornaDataAtual());
        $("#HoraInicio").val("");
        $("#HoraFinal").val("");
    }
}

function ExcluirRowData(value) {
    let i = value;
    let requestData = {
        idData: datas[i].Id
    }

    $.ajaxPost(urlExisteInscricaoData, requestData, function (ret) {
        if (ret.status == "OK") {
            let msg = "Deseja excluir a Data?";
            if (ret.data === true) {
                msg = `<h3 style="color: red">Existem registros de Presença para a data (${datas[i].Data}). Você tem certeza que deseja excluir este data?<br/ ><br />Após a confirmação NÃO será possível recuperar os registros de Presença para a data.</h3>`;
            }
            $.showYesNoAlert(msg, "Excluir Data", function () {
                datas[i].Acao = 3;
                MontarGridDatas();
            });
        }
        else {
            $.bsMessageError("Atenção", ret.Message);
        }
    });
}

function AtualizarRowData(value) {
    indexData = value;
    $("#Data").val(datas[indexData].Data);
    $("#HoraInicio").val(datas[indexData].HoraInicio);
    $("#HoraFinal").val(datas[indexData].HoraFinal);
    HabilitaBotoesAtualAdic(true);
    //$("#Data").focus();
    HabilitarBotoesTabela($(document.getElementById("tbDatas").getElementsByTagName("tbody")), true);
}

function HabilitaInscricaoAutom(habilita) {
    if (habilita)
        $("#divInscAutom").show();
    else
        $("#divInscAutom").hide();
}

function HabilitaBotoesAtualAdic(atualiza) {
    if (atualiza) {
        $("#btnAddData").hide()
        $("#btnUpdData").show()
    }
    else {
        $("#btnAddData").show()
        $("#btnUpdData").hide()
    }
}

function HabilitarBotoesTabela(table, enable) {
    $(table).find('tr').each(function (i, row) {
        var $row = $(row);
        $($row).find('.btn').each(function (i, btn) {
            $(btn).prop('disabled', enable);
        })
    })
}

function MontarGridDatas() {
    let table = document.getElementById("tbDatas").getElementsByTagName("tbody")[0];
    $(table).empty();

    let linhas = 0;
    if (datas != null && datas.length > 0) {
        for (var i = 0; i < datas.length; i++) {
            if (datas[i].Acao !== 3) {
                let row = table.insertRow(linhas);
                row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
                $.incluirCelulasTabela(row, 0, null, datas[i].Data);
                $.incluirCelulasTabela(row, 1, null, datas[i].HoraInicio);
                $.incluirCelulasTabela(row, 2, null, datas[i].HoraFinal);
                if (!isreadonly) {
                    var botoes = ` <button type="button" class="btn btn-info btn-sm" onclick="AtualizarRowData(${i});"><span class="glyphicon glyphicon-refresh"></span> Atualizar</button>
                         <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowData(${i});"><span class="glyphicon glyphicon-trash"></span> Excluir</button>`;
                    $.incluirCelulasTabela(row, 3, null, botoes);
                }
                linhas++;
            }
        }
    }
    else {
        let row = table.insertRow(i);
        row.className = "jtable-data-row";
        $.incluirCelulasTabela(row, 0, "100%", "Não existem Datas cadastradas para o Curso/Evento.", 4);
    }
}

function ValidarHoras() {
    var Data = $("#Data").val();
    var HoraInicio = $("#HoraInicio").val();
    var HoraFim = $("#HoraFinal").val();

    if (Data === undefined || Data === "") {
        $.bsMessageError("Atenção", "Data do Curso/Evento é de preenchimento obrigatório.");
        return false;
    }

    if (HoraInicio === undefined || HoraInicio === "") {
        $.bsMessageError("Atenção", "Hora do Início do Curso/Evento é de preenchimento obrigatório.");
        return false;
    }
    else if (HoraFim === undefined || HoraFim === "") {
        $.bsMessageError("Atenção", "Hora do Término do Curso/Evento é de preenchimento obrigatório.");
        return false;
    }
    else {
        var dataArray = Data.split("/");
        var newDataInicio = dataArray[1] + "/" + dataArray[0] + "/" + dataArray[2];

        var stt = new Date(newDataInicio + " " + HoraInicio);
        stt = stt.getTime();

        var endt = new Date(newDataInicio + " " + HoraFim);
        endt = endt.getTime();

        if (stt > endt) {
            $.bsMessageError("Atenção", "Hora do Início é maior que a Hora Final do Curso/Evento.");
            $("#HoraInicio").val("");
            $("#HoraFinal").val("");
            return false;
        }
    }
    return true;
}

function RetornaDataAtual() {
    let dataHoje = new Date();
    let dia = padLeft(dataHoje.getDate(), '0', 2);
    let mes = padLeft(dataHoje.getMonth() + 1, '0', 2);
    let ano = dataHoje.getFullYear();
    return `${dia}/${mes}/${ano}`;
}

function ValidarFormulario() {
    var erro = "";
    $('span[id*=-error]').each(function (x, item) {
        erro += $(item).text() + "<br>";
    });
    return erro;
}

function CarregarArquivos(arq, exibirExcluir) {
    if (arq != null && arq.length > 0) {
        let conteudo = '';
        $("#divCard").html('');
        let linha = 1;
        for (var i = 0; i < arq.length; i++) {
            conteudo += Arquivo(arq[i].Nome, arq[i].Tamanho, arq[i].Tipo.toLowerCase(), $("#Id").val(), exibirExcluir);
            if (linha % 6 == 0) {
                $("#divCard").append(`<div class="row" id="div${i}">`);
                $(`#div${i}`).append(conteudo);
                conteudo = "";
            }
            linha++;
        }
        if (conteudo != "") {
            $("#divCard").append(`<div class="row" id="div${arq.length}">`);
            $(`#div${arq.length}`).append(conteudo);

        }
    }
    else {
        $("#divCard").html('<label>Não há arquivos armazenados para o Curso/Evento.</label>');
    }

}

function Arquivo(nomeArq, tamArq, tipo, idPresenca, exibirExcluir) {
    let file =
        `<div class="col-sm-5 col-lg-2 col-xl-1">
            <div class="file-man-box">
                ${exibirExcluir == true ? `<a href="javascript:void(0)" onclick="DeleteArquivo('${nomeArq}', ${idPresenca});return false;" class="file-close" title="Excluir Arquivo"><i class="glyphicon glyphicon-floppy-remove"></i></a>` : ''}
                <div class="file-img-box"><img src="/images/${tipo}.svg" alt="${nomeArq}" title="${nomeArq}"></div>
                    <div class="file-man-title">
                        <h5 class="mb-0 text-overflow">${nomeArq}</h5>
                        <p class="mb-0"><small>${bytesToSize(tamArq)}</small></p>
                    </div>
                    <a href="javascript:void(0)" onclick="DownloadArquivo('${nomeArq}', ${idPresenca});return false;" class="file-download" title="Baixar Arquivo"><i class="glyphicon glyphicon-download  file-img-dowload" style="margin: 10px"></i></a>
                </div>
            </div>`;
    return file;
}

function DownloadArquivo(nomeArq, id) {
    let url = `${urlDownload}?idPresenca=${id}&nomeArquivo=${nomeArq}`;
    $.downloadArquivo(url, null);
}

function DeleteArquivo(nomeArq, id) {
    $.showYesNoAlert(`Deseja excluir o arquivo ${nomeArq}?`, "Excluir Arquivo", function () {
        let requestData = {
            idPresenca: id,
            nomeArquivo: nomeArq
        }

        $.ajaxPost(urlExcArq, requestData, function (ret) {
            if (ret.status == "OK") {
                arquivos = ret.data;
                CarregarArquivos(arquivos, $("#Acao").val() == "Update");
            }
            else {
                $.bsMessageError("Atenção", ret.Message);
            }
        });
    });
}

function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
}