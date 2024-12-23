var indexObs = -1;
var permiteexcluirobservacaomembro = false;
var isreadonly = false;
var files;
var arquivos = [];
var urlFechar = "";

$(function () {

    var acao = $("#Acao").val();
    permiteexcluirobservacaomembro = ($("#PermiteExcluirObservacaoMembro").val().toLowerCase() == "true");
    isreadonly = $("#Pessoa_somenteleitura").val().toLowerCase() == "true";

    if (obs == null || obs == undefined)
        obs = [];

    if ((acao == "Create") || (acao == "Read") ||
        (acao == "Update") || (acao == "Delete")) {

        if (obs && obs.length > 0) {
            MontarGridObs(obs);
        }
    }

    $("#lnkBuscarCongregacao").removeAttr("class");

    var usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
    if (usuarioCongregacaoSede === false) {
        // Usuário de outras congregações não podem fazer a busca
        $("#CongregacaoId").attr("readonly", "readonly");
        $("#lnkBuscarCongregacao").attr("class", "disabled");
    } else {
        // Para usuario sede habilita busca de congregação
        $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoId");
    }

    $("#CongregacaoId").blur(function () {
        var value = $(this).val();
        if (value > 0) {

            var requestData = {
                CongregacaoId: value
            };

            $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (congregacao) {
                $("#CongregacaoNome").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código inválido!");
                    $(this).val("");
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoNome").val(congregacao.Nome);
                }
            });
        }
    });

    /************************************************************************************************************
    * BATISMO
    ************************************************************************************************************/
    $("#BatismoId").change(function () {
        var data = $('option:selected', $("#BatismoId")).text();

        var erroDataPrevistaBatismo = $(document).find("[data-valmsg-for='DataPrevistaBatismo']");
        erroDataPrevistaBatismo.html('');

        if (data != null && data != undefined && data != "") {
            var objDate = new Date();
            objDate.setYear(data.split("/")[2]);
            objDate.setMonth(data.split("/")[1] - 1);//- 1 pq em js é de 0 a 11 os meses
            objDate.setDate(data.split("/")[0]);

            if (objDate.getTime() < new Date().getTime()) {
                erroDataPrevistaBatismo.html('<span id="DataPrevistaBatismo-error" class="">A Data prevista para o Batismo deve ser maior que a Data Atual</span>');
                return false;
            };
        };
    });

    $("#Pessoa_DataNascimento").change(() => {
        if ($("#Pessoa_DataNascimento").val() != null && $("#Pessoa_DataNascimento").val() != "") {
            let dt = $("#Pessoa_DataNascimento").val().split("/");
            let dtNasc = new Date(dt[2], dt[1] - 1, dt[0]);
            let erroDataNasc = $(document).find("[data-valmsg-for='Pessoa.DataNascimento']");

            if (dtNasc >= new Date()) {
                $.alertError("Batismo", "Data de Nascimento maior que a Data Atual");
                erroDataNasc.html("Data de Nascimento maior que a Data Atual");
                return;
            }

            let idBatismo = $('option:selected', $("#BatismoId")).val();
            let dtBatismo = $('#DataPrevistaBatismo').val();
            if (((idBatismo != null && idBatismo != "") || (dtBatismo != null && dtBatismo != ""))) {
                let requestData = {
                    id: idBatismo
                };

                $.ajaxPost(urlBuscarBatismo, requestData, function (ret) {
                    if (ret.batismo) {
                        let dtBatismo = new Date(ret.batismo.DataBatismo);
                        let idade = CalcularIdade(dtNasc, dtBatismo);
                        let msgIdade = `<h3 style="color:red">A Idade do Candidato (${idade} anos) é inferior a idade mínima permitida (${ret.batismo.IdadeMinima} anos) para o Batismo.</h3><br />`;

                        erroDataNasc.html("");

                        let usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
                        if (usuarioCongregacaoSede === false && idade < ret.batismo.IdadeMinima) {
                            $.alertError("Batismo", msgIdade);
                            erroDataNasc.html(msgIdade.replace("<h3 style=", "").replace('"', '').replace("color:red", "").replace('">', '').replace('</h3><br />', ''));
                        }
                    }
                });
            }
        }
    });

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("membroid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Candidato ao Batismo?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Membro",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status === "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Membro", response.mensagem);
                    }
                });
            });
    });

    /************************************************************************************************************
    * OBSERVACAO
    ************************************************************************************************************/
    $(document).on('click', '#btnCancObs', {}, function (e) {
        e.preventDefault();

        $("#Observacao").val("");
        $("#DataCadastroObs").val("");
        indexObs = -1;
        $("#btnAddObs").val("Adicionar");
        HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), false);
    })

    $(document).on('click', '#btnAddObs', {}, function (e) {
        e.preventDefault();
        var dadosObs = CarregarDadosObservacao($("#Observacao").val(), $("#DataCadastroObs").val(), $("#nomeUsuarioObs").val(), $("#idUsuarioObs").val(), true);

        var erroObs = $(document).find("[data-valmsg-for='Observacao']");
        erroObs.html('');

        var errordata = $(document).find("[data-valmsg-for='DataCadastroObs']");
        errordata.html('');

        var error = false;
        if (dadosObs.Observacao == null || dadosObs.Observacao == undefined || dadosObs.Observacao == 0) {
            error = true;
            erroObs.html('<span id="Observacao-error" class="">O campo Observação é obrigatório</span>');
        }

        if (dadosObs.DataCadastroObs == null || dadosObs.DataCadastroObs == undefined || dadosObs.DataCadastroObs == 0) {
            error = true;
            errordata.html('<span id="DataCadastroObs-error" class="">O campo Data de Cadastro é obrigatório</span>');
        }

        if (dadosObs.DataCadastroObs != null && dadosObs.DataCadastroObs != undefined && dadosObs.DataCadastroObs != "") {
            var objDate = new Date();
            objDate.setYear(dadosObs.DataCadastroObs.split("/")[2]);
            objDate.setMonth(dadosObs.DataCadastroObs.split("/")[1] - 1);//- 1 pq em js é de 0 a 11 os meses
            objDate.setDate(dadosObs.DataCadastroObs.split("/")[0]);

            if (objDate.getTime() > new Date().getTime()) {
                errordata.html('<span id="DataCadastroObs-error" class="">A Data de Cadastro da Observação deve ser menor que a Data Atual</span>');
                error = true;
            }
        }

        if (error) {
            return false;
        }
        if (indexObs > -1) {
            obs.splice(indexObs, 1);
            indexObs = -1;
        }
        obs.push(dadosObs);
        obs.sort(function (a, b) {
            var dtS = a.DataCadastroObs.split("/");
            var dt1 = new Date(dtS[2], dtS[1] - 1, dtS[0]);
            dtS = b.DataCadastroObs.split("/");
            var dt2 = new Date(dtS[2], dtS[1] - 1, dtS[0]);

            return ((dt1 < dt2) ? 1 : ((dt1 > dt2) ? -1 : 0));
        });
        MontarGridObs(obs);

        $("#btnAddObs").val("Adicionar");
        $("#Observacao").val("");
        $("#DataCadastroObs").val("");
        if (permiteexcluirobservacaomembro && !isreadonly)
            HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), false);
    })

    /************************************************************************************************************
    * ARQUIVOS
    ************************************************************************************************************/

    if (arquivosMembros && arquivosMembros.length > 0) {
        for (var i = 0; i < arquivosMembros.length; i++) {
            arquivos.push(CarregarDadosArquivo(arquivosMembros[i].IsCurso, arquivosMembros[i].Cadastrado, arquivosMembros[i].Id,
                arquivosMembros[i].MembroId,
                arquivosMembros[i].IsCurso ? arquivosMembros[i].CursoId : 0,
                arquivosMembros[i].IsCurso ? arquivosMembros[i].NomeCurso : "",
                arquivosMembros[i].IsCurso ? arquivosMembros[i].Local : "",
                arquivosMembros[i].IsCurso ? formatJsonToDate(arquivosMembros[i].DataInicioCurso) : "",
                arquivosMembros[i].IsCurso ? formatJsonToDate(arquivosMembros[i].DataEncerramentoCurso) : "",
                arquivosMembros[i].IsCurso ? arquivosMembros[i].CargaHoraria : "",
                null, null,
                arquivosMembros[i].IsCurso ? "" : arquivosMembros[i].DescricaoArquivo,
                arquivosMembros[i].NomeOriginal, arquivosMembros[i].Tamanho, false, arquivosMembros[i].IsSave,
                arquivosMembros[i].Index));
        }
        MontarGridArquivo();
    }

    $('#Arquivo').on('change', function (e) {
        var extPermitidas = ['pdf'];
        var extArquivo = this.value.split('.').pop();

        if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
            $.alertError('Upload de Arquivo', 'Somente arquivos do Tipo PDF são permitidos para Upload.');
            this.value = '';
        }
        else {
            files = e.target.files;
        }
    });

    $(document).on('click', '#btnAddArq', {}, function (e) {
        var membroId = $('#Id').val();
        var descricao = $("#DescricaoArquivo").val();
        var tamanhoMax = 2097152; // 2MB EM BYTES
        var erro = false;
        var erroArq = $(document).find("[data-valmsg-for='Arquivo']");
        var erroDescArq = $(document).find("[data-valmsg-for='DescricaoArquivo']");
        erroArq.html('');
        erroDescArq.html('');

        if (descricao == null || descricao == "" || descricao == undefined) {
            erroDescArq.html('<span id="DescricaoArquivo-error" class="">Descrição do Arquivo é obrigatório</span>');
            erro = true;
        }

        if (files == null || files == undefined || files.length == 0) {
            erroArq.html('<span id="Arquivo-error" class="">Você deve selecionar um Arquivo</span>');
            erro = true;
        }
        else if ((files[0].size) > tamanhoMax) {
            erroArq.html('<span id="Arquivo-error" class="">Arquivo maior que 2 MB. Tamanho do arquivo selecionado: {0}.</span>'.format(bytesToSize(files[0].size)));
            erro = true;
        }

        if (!erro) {
            var file = null;
            if (files != null && files != undefined && files.length > 0)
                file = files[0];

            arquivos.push(CarregarDadosArquivo(false, null, 0, membroId, null, null, null, null, null, null, file, null, descricao, file.name, file.size, false, false, gera_id()));
            MontarGridArquivo();
            $("#Arquivo").val("");
            $("#DescricaoArquivo").val("");
        }
    });

    $(document).on('click', '#btnCancArq', {}, function (e) {
        $("#Arquivo").val("");
        $("#DescricaoArquivo").val("");
    });


    $("#btnSalvar").click(function (e) {
        e.preventDefault;
        var form = $("#frmMembro");
        var url = form.attr('action');

        if (form.valid()) {
            let dt = $("#Pessoa_DataNascimento").val().split("/");
            let dtNasc = new Date(dt[2], dt[1] - 1, dt[0]);
            var erroDataNasc = $(document).find("[data-valmsg-for='Pessoa.DataNascimento']");

            if (dtNasc >= new Date()) {
                $.alertError("Batismo", "Data de Nascimento maior que a Data Atual");
                erroDataNasc.html("Data de Nascimento maior que a Data Atual");
                return;
            }

            let requestData = {
                id: $('option:selected', $("#BatismoId")).val()
            };

            $.ajaxPost(urlBuscarBatismo, requestData, function (ret) {
                if (ret.batismo) {
                    let dtBatismo = new Date(ret.batismo.DataBatismo);

                    let idade = CalcularIdade(dtNasc, dtBatismo);
                    erroDataNasc.html("");

                    let msgIdade = `<h3 style="color:red">A Idade do Candidato (${idade} anos) é inferior a idade mínima permitida (${ret.batismo.IdadeMinima} anos) para o Batismo.</h3><br />`;

                    let usuarioCongregacaoSede = ($("#hdnUsuarioSede").val().toLowerCase() === "true");
                    if (usuarioCongregacaoSede === false && idade < ret.batismo.IdadeMinima) {
                        $.alertError("Batismo", msgIdade);
                        erroDataNasc.html(msgIdade.replace("<h3 style=", "").replace('"', '').replace("color:red", "").replace('">', '').replace('</h3><br />', ''));
                    }
                    else {
                        let msg = (idade < ret.batismo.IdadeMinima ? msgIdade : "");
                        if ($("#EnviarBatismo") === true) {
                            msg += "Deseja realmente enviar o Membro para o Batismo?";
                        } else {
                            if ($("#Acao").val() === "Update") {
                                msg += "Deseja realmente atualizar o Candidato ao Batismo?";
                            } else {
                                msg += "Deseja realmente criar um novo Candidato ao Batismo?"
                            }
                        }

                        $.showYesNoAlert(msg, "Batismo", function () {
                            $("#Obs").val(JSON.stringify(obs));

                            if (!ExistsFileToSave()) {
                                $.ajaxPostForm(url, $("#frmMembro"), function (result) {
                                    if (result.status === "OK") {
                                        window.location = result.url;
                                    }
                                    else {
                                        $.alertError("Batismo", result.msg);
                                    }
                                });
                            }
                            else {
                                var table = $('#modalSalvar table')[0].getElementsByTagName("tbody")[0];
                                $(table).empty();

                                $("#modalSalvar").modal({ backdrop: "static" });
                                AddRowGridModal(0, "Salvando os dados do Candidato ao Batismo")

                                $.ajaxPostForm(url, $("#frmMembro"), function (result) {
                                    if (result.status == "OK") {
                                        urlFechar = result.url;
                                        $('#modalSalvar tbody tr[name="row0"').find('span[id="lbl0"]').html(result.msg);
                                        $('#modalSalvar tbody tr[name="row0"').find('div[id="div0"]').html('<span class="label label-success">Finalizado</span>');

                                        for (var x = 0; x < arquivos.length; x++) {
                                            arquivos[x].MembroId = result.membroid;
                                        }

                                        for (var x = 0; x < arquivos.length; x++) {
                                            var urlUpl = "/Secretaria/Membro/SalvarExcluirArquivos";
                                            if ((arquivos[x].IsDelete && arquivos[x].IsSave) || (!arquivos[x].IsDelete && !arquivos[x].IsSave)) {
                                                if (window.FormData !== undefined) {
                                                    var formData = new FormData();
                                                    if ((arquivos[x].Arquivo != null && arquivos[x].Arquivo != undefined)) {
                                                        formData.append("files", arquivos[x].Arquivo);
                                                    }

                                                    var cadastrado = null;
                                                    var arq = CarregarDadosArquivo(arquivos[x].IsCurso, cadastrado, arquivos[x].Id, arquivos[x].MembroId, arquivos[x].CursoId, arquivos[x].NomeCurso, arquivos[x].Local, arquivos[x].DataInicioCurso,
                                                        arquivos[x].DataEncerramentoCurso, arquivos[x].CargaHoraria, null, null, arquivos[x].DescricaoArquivo, arquivos[x].NomeOriginal, arquivos[x].Tamanho, arquivos[x].IsDelete, arquivos[x].IsSave,
                                                        arquivos[x].Index);
                                                    formData.append("model", JSON.stringify(arq));

                                                    $.ajax({
                                                        type: "POST",
                                                        url: urlUpl,
                                                        contentType: false,
                                                        processData: false,
                                                        data: formData,
                                                        beforeSend: function () {
                                                            if (arquivos[x].IsDelete)
                                                                AddRowGridModal(arquivos[x].Index, 'Excluindo o Documento <b>"{0}"</b>'.format(arquivos[x].DescricaoArquivo));
                                                            else
                                                                AddRowGridModal(arquivos[x].Index, 'Salvando o Documento <b>"{0}"</b>'.format(arquivos[x].DescricaoArquivo));
                                                        },
                                                        success: function (result) {
                                                            var row = '#modalSalvar tbody tr[name="row{0}"]'.format(result.index);
                                                            var lbl = 'span[id="lbl{0}"]'.format(result.index);
                                                            var div = 'div[id="div{0}"]'.format(result.index);
                                                            if (result.status == "OK") {
                                                                $(row).find(lbl).html(result.msg);
                                                                $(row).find(div).html('<span class="label label-success">Finalizado</span>');
                                                            }
                                                            else {
                                                                $(row).find(lbl).html('<b>{0}</b>'.format(result.msg));
                                                                $(row).find(div).html('<span class="label label-danger">Erro</span>');
                                                            }

                                                            var posicao = EncontrarIndice(arquivos, function (f) {
                                                                return f.Index == result.index;
                                                            });
                                                            if (posicao > -1) {
                                                                arquivos[posicao].IsSave = true;
                                                                arquivos[posicao].IsDone = true;
                                                            }

                                                            if (HabilitarBotaoModal(arquivos)) {
                                                                $("#divFooter").css("display", "block");
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                            else {
                                                arquivos[x].IsDone = true;
                                            }
                                        }
                                    }
                                    else {
                                        var row = $('#modalSalvar tbody tr[name="row0"');
                                        row.find('span[id="lbl0"]').html(result.msg);
                                        row.find('div[id="div0"]').html('<span class="label label-danger">Erro</span>');
                                        $("#divFooter").css("display", "block");
                                        urlFechar = "";
                                        return false;
                                    }
                                });
                            }
                        });
                    }
                }
                else {
                    $.alertError("Batismo", "Batismo não encontrado!");
                }

            });
        }
        else {
            var msg = ValidarFormulario();
            if (msg != null && msg != undefined && msg != "")
                $.alertError("Candidato ao Batismo", msg);
        }
    });

    $(document).on('click', '#btnFecharModal', {}, function (e) {
        if (urlFechar != "" && urlFechar != null && urlFechar != undefined) {
            e.preventDefault;
            window.location = urlFechar;
        }
    });
});


function CalcularIdade(dtNascimento, dtAtual) {
    var diferencaAnos = dtAtual.getFullYear() - dtNascimento.getFullYear();
    if (new Date(dtAtual.getFullYear(), dtAtual.getMonth(), dtAtual.getDate()) <
        new Date(dtAtual.getFullYear(), dtNascimento.getMonth(), dtNascimento.getDate()))
        diferencaAnos--;
    return parseInt(diferencaAnos);
}

function ValidarFormulario() {
    var erro = "";
    $('span[id*=-error]').each(function (x, item) {
        erro += $(item).text() + "<br>";
    });
    return erro;
}

function IncluirCelulasTabela(row, col, width, text, colSpan) {
    var cell1 = row.insertCell(col);
    if (width != null && width != undefined && width != "")
        cell1.width = width;
    if (colSpan != null && colSpan != undefined && colSpan != "")
        cell1.colSpan = colSpan;
    cell1.innerHTML = text;
}

function HabilitarBotoesTabela(table, enable) {
    $(table).find('tr').each(function (i, row) {
        var $row = $(row);
        $($row).find('.btn').each(function (i, btn) {
            $(btn).prop('disabled', enable);
        })
    })
}

function ExcluirRowObs(value) {
    $.showYesNoAlert("Deseja excluir a Observação?", "Excluir Observação", function () {
        var i = value.parentNode.parentNode.sectionRowIndex;
        var table = document.getElementById("tbObs").getElementsByTagName("tbody")[0];
        table.deleteRow(i);
        obs.splice(i, 1);
        if (obs.length == 0) {
            var row = table.insertRow(0);
            row.className = "jtable-data-row";
            IncluirCelulasTabela(row, 0, "100%", "Não há observações cadastradas para o Membro", 4);
        }
    });
}

function AtualizarRowObs(value) {
    var i = value.parentNode.parentNode.sectionRowIndex;
    indexObs = i;
    $("#Observacao").val(obs[i].Observacao);
    $("#DataCadastroObs").val(obs[i].DataCadastroObs);
    $("#btnAddObs").val("Atualizar");
    $("#Observacao").focus();
    HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), true);
}

function CarregarDadosObservacao(observacao, datacadastroobs, responsavel, idresponsavel, mostrarBotao) {
    return {
        Observacao: observacao,
        DataCadastroObs: datacadastroobs,
        ResponsavelObs: responsavel,
        IdResponsavelObs: idresponsavel,
        MostrarBotao: mostrarBotao
    }
}

function MontarGridObs(obser) {
    var table = document.getElementById("tbObs").getElementsByTagName("tbody")[0];
    $(table).empty();

    for (var i = 0; i < obser.length; i++) {
        var row = table.insertRow(i);
        row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
        IncluirCelulasTabela(row, 0, null, obser[i].Observacao);
        IncluirCelulasTabela(row, 1, null, obser[i].DataCadastroObs);
        IncluirCelulasTabela(row, 2, null, obser[i].ResponsavelObs);

        if (obser[i].MostrarBotao || (permiteexcluirobservacaomembro && !isreadonly)) {
            var botoes = ' <button type="button" class="btn btn-info btn-sm" onclick="AtualizarRowObs(this);"><span class="glyphicon glyphicon-refresh"></span> Atualizar</button>' +
                ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowObs(this);"><span class="glyphicon glyphicon-trash"></span> Excluir</button>';
            IncluirCelulasTabela(row, 3, null, botoes);
        }
    }
}

function MontarGridArquivo() {
    var arq = EncontrarItem(arquivos, function (c) {
        return c.IsCurso == false && c.IsDelete == false
    });

    var isreadonly = $('#IsReadOnly').val().toLowerCase() == "true";
    if (arq == undefined || arq.length == 0) {
        var table = document.getElementById("tbArq").getElementsByTagName("tbody")[0];
        $(table).empty();
        var row = table.insertRow(0);
        row.className = "jtable-data-row";
        IncluirCelulasTabela(row, 0, "100%", "Não há Documentos cadastrados para o Candidato", 4);
    }
    else {
        var table = document.getElementById("tbArq").getElementsByTagName("tbody")[0];
        $(table).empty();
        for (var i = 0; i < arq.length; i++) {
            var row = table.insertRow(i);
            row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");

            IncluirCelulasTabela(row, 0, null, arq[i].DescricaoArquivo);
            IncluirCelulasTabela(row, 1, null, arq[i].NomeOriginal);
            IncluirCelulasTabela(row, 2, null, bytesToSize(arq[i].Tamanho));
            var botao = "";
            if (!isreadonly)
                if (arq[i].IsSave)
                    botao += ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowArq(this,{0});"><span class="glyphicon glyphicon-trash"></span> Excluir</button>'.format(arq[i].Id);
                else
                    botao += ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowArq(this,{0});"><span class="glyphicon glyphicon-trash"></span> Excluir</button>'.format(arq[i].Index);

            if (arq[i].IsSave && arq[i].NomeOriginal != null && arq[i].NomeOriginal != undefined && arq[i].NomeOriginal != "")
                botao += ' <button type="button" class="btn btn-info btn-sm" onclick="DownloadRowArq(this,{0});"><span class="glyphicon glyphicon-download-alt"></span> Download</button>'.format(arq[i].Id);
            IncluirCelulasTabela(row, 3, null, botao);
        }
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

function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
}

function DownloadRowArq(value, index) {
    Download(index)
}

function ExcluirRowArq(value, index) {
    $.showYesNoAlert("Deseja excluir o Arquivo?", "Excluir Arquivo", function () {
        ExcluirArquivo(index);
        MontarGridArquivo();
    });
}

function CarregarDadosArquivo(iscurso, cadastrado, id, membroid, cursoid, nomecurso, local, datainiciocurso,
    dataencerramentocurso, cargahoraria, arquivo, arquivocurso, descricao, nomeoriginal, tamanho, isdelete, issave,
    index, isdone) {
    return {
        IsCurso: iscurso,
        Cadastrado: cadastrado,
        Id: id,
        MembroId: membroid,
        CursoId: cursoid,
        NomeCurso: nomecurso,
        Local: local,
        DataInicioCurso: datainiciocurso,
        DataEncerramentoCurso: dataencerramentocurso,
        CargaHoraria: cargahoraria,
        Arquivo: arquivo,
        ArquivoCurso: arquivocurso,
        DescricaoArquivo: descricao,
        NomeOriginal: nomeoriginal,
        Tamanho: tamanho,
        IsDelete: isdelete,
        IsSave: issave,
        Index: index,
        IsDone: isdone
    }
}

function gera_id() {
    var size = 7///tamanho do ID e armazena na variável
    var randomized = Math.ceil(Math.random() * Math.pow(10, size));//Cria um número aleatório do tamanho definido em size.
    var digito = Math.ceil(Math.log(randomized));//Cria o dígito verificador inicial
    while (digito > 10) {//Pega o digito inicial e vai refinando até ele ficar menor que dez
        digito = Math.ceil(Math.log(digito));
    }
    var id = "{0}{1}".format(randomized, digito);//Cria a ID
    return parseInt(id);
}

function ExcluirArquivo(index) {
    var posicao = EncontrarIndice(arquivos, function (f) {
        return f.Index == index;
    });
    if (posicao > -1) {
        arquivos[posicao].IsDelete = true;
    }
}

function ExistsFileToSave() {
    for (var i = 0; i < arquivos.length; i++) {
        if ((arquivos[i].IsDelete && arquivos[i].IsSave) || (!arquivos[i].IsDelete && !arquivos[i].IsSave)) {
            return true;
        }
    }
    return false;
}

function AddRowGridModal(index, message) {
    var table = $('#modalSalvar table')[0].getElementsByTagName("tbody")[0];
    var row = table.insertRow(table.rows.length);
    row.setAttribute("name", "row{0}".format(index));
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    cell1.innerHTML = '<span id="lbl{0}">{1}</span>'.format(index, message);
    cell2.innerHTML = '<div id="div{0}"><div class="as-reload-img"><div></div>'.format(index);
}

function HabilitarBotaoModal(arquivo) {
    count = arquivo.reduce(function (r, a) {
        return r + + (a.IsDone);
    }, 0);
    return arquivo.length == count;
}

function Download(index, btn) {
    var url = "/Secretaria/Membro/DownloadArquivo?id={0}".format(index);
    $.downloadArquivo(url, btn);

}