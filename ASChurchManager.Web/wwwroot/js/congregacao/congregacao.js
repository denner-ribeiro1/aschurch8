//$.validator.defaults.ignore = "";

var files;
var indexObs = -1;
let extPermitidas = ['xls', 'xlsx', 'doc', 'docx', 'pdf', 'zip'];
let tamanhoMax = 104857600;

$(function () {
    $(".cnpj").mask("99.999.999/9999-99");


    $("#lnkCongregacaoResponsavelId").popUp("Pesquisar Congregação Responsável", "Congregacao", "CongregacaoResponsavelId");

    var paramsMembro = {
        TipoMembro: 3,
        Status: 1
    };

    $("#lnkPastorResponsavelId").popUp("Pesquisar Pastor", "Membro", "PastorResponsavelId", paramsMembro);

    $("#lnkResponsavelId").popUp("Pesquisar Membro Responsável", "Membro", "ResponsavelId", paramsMembro);

    $("#lnkObreiroId").popUp("Pesquisar Obreiro", "Membro", "ObreiroId", paramsMembro);

    $("#CongregacaoResponsavelId").blur(function () {
        var value = $(this).val();
        if (value > 0) {
            var requestData = {
                CongregacaoId: value
            };
            $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacao", requestData, function (congregacao) {
                $("#CongregacaoResponsavelNome").val("");
                if (congregacao.Id == 0) {
                    $.alertError("Atenção", "Código inválido!");
                    $(this).val("");
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoResponsavelNome").val(congregacao.Nome);
                }
            });
        }
    });

    $("#PastorResponsavelId").change((e) => {
        e.preventDefault();
        var value = $("#PastorResponsavelId").val();
        if (value > 0) {
            if (obreiros.length > 0) {
                var item = $.findElement(obreiros, "ObreiroId", $("#PastorResponsavelId").val());
                if (item != null && item != undefined) {
                    $.alertError("Atenção", "Código já incluído na Lista de Obreiros!");
                    $("#PastorResponsavelId").val("");
                    $("#PastorResponsavelNome").val("");
                    return;
                }
            }

            var requestData = {
                membroId: value,
                congregacaoId: $("#Id").val()
            };

            $.ajaxPost(urlBuscarRespons, requestData, function (data) {
                if (data.result == "OK") {
                    const { membro } = data;
                    $("#PastorResponsavelNome").val("");
                    if (membro.Id == 0) {
                        $.alertError("Atenção", "Código inválido!");
                        $("#PastorResponsavelId").val("");
                    } else if (membro.Status != 1) {
                        $.alertError("Atenção", "Membro não Ativo!");
                        $("#PastorResponsavelId").val("");
                    } else if (membro.Cargos.length == 0) {
                        $.alertError("Atenção", "Membro não possui Cargos cadastrado!");
                        $("#PastorResponsavelId").val("");
                    }
                    else {
                        $("#PastorResponsavelNome").val(membro.Nome);
                    }
                }
                else {
                    $("#PastorResponsavelId").val("");
                    $("#PastorResponsavelNome").val("");
                    $.alertError("Atenção", data.mensagem);
                }

            });
        }
    });

    $("#ObreiroId").change((e) => {
        e.preventDefault();

        var value = $("#ObreiroId").val();
        if (value > 0) {

            $("#ObreiroNome").val("");
            $("#ObreiroCargo").val("");

            if (obreiros.length > 0) {
                var item = $.findElement(obreiros, "ObreiroId", $("#ObreiroId").val());
                if (item != null && item != undefined) {
                    $.alertError("Atenção", "Código já incluído na Lista de Obreiros!");
                    $("#ObreiroId").val("");
                    $("#ObreiroNome").val("");
                    $("#ObreiroCargo").val("");
                    return;
                }
            }

            if ($("#PastorResponsavelId").val() == value) {
                $.alertError("Atenção", "Obreiro já cadastrado como Pastor Responsável da Congregação!");
                $("#ObreiroId").val("");
                $("#ObreiroNome").val("");
                $("#ObreiroCargo").val("");
                return;
            }

            var requestData = {
                membroId: value,
                congregacaoId: $("#Id").val(),
                obreiro: true
            };

            $.ajaxPost(urlBuscarRespons, requestData, function (data) {
                if (data.result == "OK") {
                    const { membro } = data;
                    $("#ObreiroNome").val("");
                    $("#ObreiroCargo").val("");
                    if (membro.Id == 0) {
                        $.alertError("Atenção", "Código inválido!");
                        $("#ObreiroId").val("");
                    } else if (membro.Status != 1) {
                        $.alertError("Atenção", "Membro não Ativo!");
                        $("#ObreiroId").val("");
                    } else if (membro.Cargos.length == 0) {
                        $.alertError("Atenção", "Membro não possui Cargos cadastrado!");
                        $("#ObreiroId").val("");
                    }
                    else {
                        $("#ObreiroNome").val(membro.Nome);
                        $("#ObreiroCargo").val(membro.Cargos[0].Descricao);
                    }
                }
                else {
                    $("#ObreiroId").val("");
                    $("#ObreiroNome").val("");
                    $("#ObreiroCargo").val("");
                    $.alertError("Atenção", data.mensagem);
                }
            });
        }
    });

    $("#ResponsavelId").blur(function () {
        var value = $(this).val();
        if (value > 0) {
            var requestData = {
                MembroId: value
            };
            $.ajaxPost("/Secretaria/Membro/BuscarMembro", requestData, function (membro) {
                $("#ResponsavelNome").val("");
                if (membro.Id == 0) {
                    $.alertError("Atenção", "Código inválido!");
                    $(this).val("");
                } else if (membro.Status != 1) {
                    $.alertError("Atenção", "Membro não Ativo!");
                    $(this).val("");
                }
                else if (membro.Id > 0) {
                    $("#ResponsavelNome").val(membro.Nome);
                }
            });
        }
    });

    var acao = $("#Acao").val();
    if ((acao == "Create") || (acao == "Read") ||
        (acao == "Update") || (acao == "Delete")) {

        var isreadonlygrupo = ($("#isreadonlygrupo").val().toLowerCase() == "true");
        var isreadonlyobreiro = ($("#isreadonlyobreiro").val().toLowerCase() == "true");
        let isreadonlyobs = ($("#isreadonlyobs").val().toLowerCase() == "true");


        if (obreiros.length > 0) {
            let table = document.getElementById("tbObreiro").getElementsByTagName("tbody")[0];
            $(table).empty();

            for (var i = 0; i < obreiros.length; i++) {
                let row = table.insertRow(i);
                row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
                $.incluirCelulasTabela(row, 0, null, obreiros[i].ObreiroId);
                $.incluirCelulasTabela(row, 1, null, obreiros[i].ObreiroNome);
                $.incluirCelulasTabela(row, 2, null, obreiros[i].ObreiroCargo);
                if (!isreadonlyobreiro)
                    $.incluirCelulasTabela(row, 3, null, '<input type="button" value="Excluir" class="btn btn-danger btn-sm" onclick="ExcluirRowObr(this);">');
            }
        }

        if (grupos.length > 0) {
            let table = document.getElementById("tbGrupo").getElementsByTagName("tbody")[0];
            $(table).empty();

            for (var i = 0; i < grupos.length; i++) {
                let row = table.insertRow(i);
                row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
                $.incluirCelulasTabela(row, 0, null, grupos[i].Grupo);
                $.incluirCelulasTabela(row, 1, null, grupos[i].NomeGrupo);
                $.incluirCelulasTabela(row, 2, null, grupos[i].ResponsavelId);
                $.incluirCelulasTabela(row, 3, null, grupos[i].ResponsavelNome);
                if (!isreadonlygrupo)
                    $.incluirCelulasTabela(row, 4, null, '<input type="button" value="Excluir" class="btn btn-danger btn-sm" onclick="ExcluirRowGrp(this);">');
            }
        }

        if (observacoes.length > 0) {
            MontarGridObs(observacoes);
        }
    }

    //*****************OBREIROS ******************/
    $("#btnAddObr").click(function () {
        var dadosObreiro = CarregarObreiro($("#ObreiroId").val(), $("#ObreiroNome").val(), $("#ObreiroCargo").val());
        var errorObr = $(document).find("[data-valmsg-for='ObreiroId']");
        errorObr.html('');

        var error = false;
        if (dadosObreiro.ObreiroId == null || dadosObreiro.ObreiroId == undefined || dadosObreiro.ObreiroId == 0) {
            error = true;
            errorObr.html('<span id="ObreiroId-error" class="">O campo Código do Obreiro é obrigatório</span>');
        }
        if (error) {
            return false;
        }
        obreiros.push(dadosObreiro);
        obreiros.sort(function (a, b) {
            if (a.ObreiroCargo < b.ObreiroCargo) return -1;
            if (a.ObreiroCargo > b.ObreiroCargo) return 1;
            return 0;
        });

        var table = document.getElementById("tbObreiro").getElementsByTagName("tbody")[0];
        $(table).empty();

        for (var i = 0; i < obreiros.length; i++) {
            var row = table.insertRow(i);
            row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
            $.incluirCelulasTabela(row, 0, null, obreiros[i].ObreiroId);
            $.incluirCelulasTabela(row, 1, null, obreiros[i].ObreiroNome);
            $.incluirCelulasTabela(row, 2, null, obreiros[i].ObreiroCargo);
            $.incluirCelulasTabela(row, 3, null, '<input type="button" value="Excluir" class="btn btn-danger btn-sm" onclick="ExcluirRowObr(this);">');
        }

        $("#ObreiroId").val("");
        $("#ObreiroNome").val("");
        $("#ObreiroCargo").val("");
    });
    //*****************OBREIROS ******************/

    //*****************GRUPO ******************/
    $("#btnAddGrupo").click(function () {
        var dadosGrupo = CarregarGrupo($("#GrupoId").val(), $('#GrupoId :selected').text(), $("#NomeGrupo").val(), $("#ResponsavelId").val(), $("#ResponsavelNome").val());
        var error = false;

        var errorId = $(document).find("[data-valmsg-for='GrupoId']");
        errorId.html('');
        if (dadosGrupo.GrupoId == null || dadosGrupo.GrupoId == undefined || dadosGrupo.GrupoId == 0) {
            error = true;
            errorId.html('<span id="GrupoId-error" class="">O campo Grupo é obrigatório</span>');
        }

        var errorNome = $(document).find("[data-valmsg-for='NomeGrupo']");
        errorNome.html('');
        if (dadosGrupo.NomeGrupo == null || dadosGrupo.NomeGrupo == undefined || dadosGrupo.NomeGrupo == "") {
            error = true;
            errorNome.html('<span id="NomeGrupo-error" class="">O campo Nome do Grupo é obrigatório</span>');
        }

        var errorResp = $(document).find("[data-valmsg-for='ResponsavelId']");
        errorResp.html('');
        if (dadosGrupo.ResponsavelId == null || dadosGrupo.ResponsavelId == undefined || dadosGrupo.ResponsavelId == 0 || dadosGrupo.ResponsavelId == "") {
            error = true;
            errorResp.html('<span id="ResponsavelId-error" class="">O campo Cód. Resp. é obrigatório</span>');
        }

        if (error) {
            return false;
        }
        grupos.push(dadosGrupo);
        var table = document.getElementById("tbGrupo").getElementsByTagName("tbody")[0];
        $(table).empty();

        for (var i = 0; i < grupos.length; i++) {
            var row = table.insertRow(i);
            row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
            $.incluirCelulasTabela(row, 0, null, grupos[i].Grupo);
            $.incluirCelulasTabela(row, 1, null, grupos[i].NomeGrupo);
            $.incluirCelulasTabela(row, 2, null, grupos[i].ResponsavelId);
            $.incluirCelulasTabela(row, 3, null, grupos[i].ResponsavelNome);
            $.incluirCelulasTabela(row, 4, null, '<input type="button" value="Excluir" class="btn btn-danger btn-sm" onclick="ExcluirRowGrp(this);">');
        }
        $("#GrupoId").val("");
        $("#NomeGrupo").val("");
        $("#ResponsavelId").val("");
        $("#ResponsavelNome").val("")
    });
    //*****************GRUPO ******************/

    //*****************OBSERVACAO ******************/
    $("#btnAddObs").click(function (e) {
        e.preventDefault();

        var dadosObs = CarregarObs($("#Observacao").val(), null, $("#idUsuarioObs").val(), $("#nomeUsuarioObs").val());
        var error = false;

        var errorId = $(document).find("[data-valmsg-for='Observacao']");
        errorId.html('');
        if (dadosObs.Observacao == null || dadosObs.Observacao == undefined || dadosObs.Observacao == 0) {
            error = true;
            errorId.html('<span id="Observacao-error" class="">O campo Observação é de preenchimento obrigatório</span>');
        }

        if (error) {
            return false;
        }
        if (indexObs > -1) {
            observacoes.splice(indexObs, 1);
            indexObs = -1;
        }
        observacoes.push(dadosObs);

        MontarGridObs(observacoes);

        $("#btnAddObs").html("<span class=\"glyphicon glyphicon-plus\"></span> Adicionar \n");
        $("#btnAddObs").removeClass("btn-info")
        $("#btnAddObs").addClass("btn-primary")
        $("#Observacao").val("");

        if (!isreadonlyobreiro)
            HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), false);
    });

    $(document).on('click', '#btnCancObs', {}, function (e) {
        e.preventDefault();

        $("#Observacao").val("");
        indexObs = -1;
        $("#btnAddObs").html("<span class=\"glyphicon glyphicon-plus\"></span> Adicionar \n");
        $("#btnAddObs").removeClass("btn-info")
        $("#btnAddObs").addClass("btn-primary")

        HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), false);
    })
    //*****************OBSERVACAO ******************/

    $("#btnBuscarCEP").click(function () {
        PesquisarCEP($("#Endereco_Cep"), $("#Endereco_Logradouro"), $("#Endereco_Bairro"), $("#Endereco_Cidade"), $("#Endereco_Estado"));
    });

    $(document).on('click', '#btnSalvar', {}, function (e) {
        e.preventDefault();

        var msg = "Deseja realmente criar uma nova Congregação?";
        if ($("#Acao").val() == "Update") {
            msg = "Deseja realmente atualizar a Congregação?";
        }
        $.showYesNoAlert(msg, "Congregação", function () {
            var form = $("#frmCongrecacao");
            //form.data("validator").settings.ignore = "";
            form.validate();
            if (form.valid()) {
                $("#Grp").val(JSON.stringify(grupos));
                $("#Obr").val(JSON.stringify(obreiros));
                $("#Obs").val(JSON.stringify(observacoes));
                form.submit();
            }
            else {
                var msg = ValidarFormulario();
                if (msg != null && msg != undefined && msg != "")
                    $.alertError("Membro", msg);
            }
        });
    })

    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("congregregaoid") };

        $.ajaxPost(urlQtdMemb, request, function (response) {
            if (response.findIndex(x => x.Quantidade > 0) >= 0) {
                var table = $('#modalExcluir table')[0].getElementsByTagName("tbody")[0];
                $(table).empty();

                $("#modalExcluir").modal({ backdrop: "static" });
                for (var i = 0; i < response.length; i++) {
                    if (response[i].Quantidade > 0) {
                        AddRowGridModal(i, response[i].TipoDescr, response[i].Quantidade, $('#lnkExcluir').data("congregregaoid"));
                    }
                }
            }
            else {
                $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir esta Congregação?</br>Após a confirmação NÃO será mais possível recuperar os dados.</h3>', "Congregação",
                    function () {
                        $.ajaxPost(urlExcluir, request, function (response) {
                            if (response.status == "OK") {
                                window.location = response.url;
                            }
                            else {
                                $.alertError("Membro", response.mensagem);
                            }
                        });
                    }
                );
            }
        });
    });

    //*****************POPUP EXCLUSÁO ******************/
    $("#lnkBuscarCongregacao").popUp("", "Congregacao", "CongregacaoNovaId");

    $("#CongregacaoNovaId").change(function () {
        var errorCongr = $(document).find("[data-valmsg-for='CongregacaoNovaId']");
        errorCongr.html('');

        var congrOrig = $(this).data("congregorig");
        var congrDest = $("#CongregacaoNovaId").val();

        if (congrDest == congrOrig) {
            $.bsMessageError("Atenção", "Congregação Destino deve ser diferente da Congregação Origem");
            $("#CongregacaoNovaId").val("");
            $("#CongregacaoNome").val("");
            return false;
        }

        if (congrDest > 0) {
            var requestData = {
                CongregacaoId: congrDest
            };

            $.ajaxPost("/Secretaria/Congregacao/BuscarCongregacaoSemFiltroUsuario", requestData, function (congregacao) {
                $("#CongregacaoNome").val("");
                if (congregacao.Id == 0) {
                    $.bsMessageError("Atenção", "Código de Congregação inválido!");
                    $("#CongregacaoNovaId").val("");
                    $("#CongregacaoNome").val("");
                } else if (congregacao.Id > 0) {
                    $("#CongregacaoNome").val(congregacao.Nome);
                }
            });
        }
    });

    $(document).on('click', '#btnExcluirModal', {}, function (e) {
        e.preventDefault();

        let id = $(this).data("congregid");
        let nome = $(this).data("congregnome");
        var congrDest = $("#CongregacaoNovaId").val();
        var congrNomeDest = $("#CongregacaoNome").val();

        if (congrNomeDest == undefined || congrNomeDest == null || congrNomeDest == "") {
            $.bsMessageError("Atenção", "Congregação Destino é de preenchimento obrigatório!");
            $("#CongregacaoNovaId").val("");
            $("#CongregacaoNome").val("");
            return false;
        }

        if (congrDest == id) {
            $.bsMessageError("Atenção", "Congregação Destino deve ser diferente da Congregação Origem");
            $("#CongregacaoNovaId").val("");
            $("#CongregacaoNome").val("");
            return false;
        }

        let request = { id: id, congregacaoId: congrDest }

        $.showYesNoAlert(`<h3 style="color:red">IMPORTANTE:<br/><br/>Todas os dados (Eventos, Obreiros, Arquivos, etc) referentes a Congregação '${nome}' serão excluídos e os Nascimentos, Casamentos, Cursos, Usuários e Membros vinculados a ela serão transferidos para a Congregação '${$("#CongregacaoNome").val()}'.<br/><br/>Você tem certeza que deseja a excluir congregação '${nome}'? <br/> Após a confirmação NÃO será mais possível recuperar os dados.</h3>'`, "Congregação",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Membro", response.mensagem);
                    }
                });

                $.alertError("Membro", "Excluido");
            }
        );
    });


    //*****************POPUP EXCLUSÁO ******************/
    if ($("#Acao").val() != "Create")
        CarregarArquivos(arquivos, $("#Acao").val() == "Update");

    $('input[name="Arquivo"]').change(function (e) {
        if (ValidarArquivoUpload(e.target.files)) {
            files = e.target.files;
            var file = $('input[name="Arquivo"]')[0].files[0];
            $('.size-file').html(` ${bytesToSize(file.size)}`);
        }
        else {
            $('#Arquivo').val("");
            $('.size-file').empty();
            files = null;
        }

    });

    $("#btnCancelar").click((e) => {
        $('#Arquivo').val("");
        $('.size-file').empty();
        files = null;
    });
});

function ValidarFormulario() {
    var erro = "";
    $(".tab-pane").find('span[id*=-error]').each(function (x, item) {
        if ($(item).text() != "") {
            erro += $(item).text() + "<br>";
        }
    });
    return erro;
}

function CarregarMembro(item, target, validarCargo) {
    var value = $(item).val();
    if (value > 0) {
        var requestData = {
            MembroId: value
        };
        $.ajaxPost("/Secretaria/Membro/BuscarMembro", requestData, function (membro) {
            if (membro.Id == 0) {
                $.alertError("Atenção", "Código inválido!");
                $(item).val("");
            } else if (membro.Status != 1) {
                $.alertError("Atenção", "Membro não Ativo!");
                $(item).val("");
            } else if (validarCargo && membro.Cargos.length == 0) {
                $.alertError("Atenção", "Membro não possui Cargos cadastrado!");
                $(item).val("");
            }
            else if (membro.Id > 0) {
                $(target).val(membro.Nome);
                return membro;
            }
        });
    }
}

function CarregarObreiro(id, nome, cargo) {
    return {
        ObreiroId: id,
        ObreiroNome: nome,
        ObreiroCargo: cargo
    };
}
function ExcluirRowObr(value) {
    $.showYesNoAlert("Deseja excluir o Obreiro?", "Excluir Obreiro", function () {
        var i = value.parentNode.parentNode.sectionRowIndex;
        var table = document.getElementById("tbObreiro").getElementsByTagName("tbody")[0];
        table.deleteRow(i);
        obreiros.splice(i, 1);
        if (obreiros.length == 0) {
            var row = table.insertRow(0);
            $.incluirCelulasTabela(row, 0, "100%", "Não há Obreiros cadastrados para o Congregação", 4);
        }
    });
}

function CarregarGrupo(id, grupo, nome, respId, respNome) {
    return {
        GrupoId: id,
        Grupo: grupo,
        NomeGrupo: nome,
        ResponsavelId: respId,
        ResponsavelNome: respNome
    };
}
function ExcluirRowGrp(value) {
    $.showYesNoAlert("Deseja excluir o Grupo?", "Excluir Grupo", function () {
        var i = value.parentNode.parentNode.sectionRowIndex;
        var table = document.getElementById("tbGrupo").getElementsByTagName("tbody")[0];
        table.deleteRow(i);
        grupos.splice(i, 1);
        if (grupos.length == 0) {
            var row = table.insertRow(0);
            $.incluirCelulasTabela(row, 0, "100%", "Não há Grupos cadastrados para o Congregação", 5);
        }
    });
}

function AtualizarRowObs(value) {
    var i = value.parentNode.parentNode.sectionRowIndex;
    indexObs = i;
    $("#Observacao").val(observacoes[i].Observacao);
    $("#btnAddObs").html("<span class=\"glyphicon glyphicon-edit\"></span> Atualizar \n");
    $("#btnAddObs").removeClass("btn-primary")
    $("#btnAddObs").addClass("btn-info")
    $("#Observacao").focus();
    HabilitarBotoesTabela($(document.getElementById("tbObs").getElementsByTagName("tbody")), true);
}
function MontarGridObs(obs) {
    var table = document.getElementById("tbObs").getElementsByTagName("tbody")[0];
    $(table).empty();

    for (var i = 0; i < obs.length; i++) {
        var row = table.insertRow(i);
        row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
        $.incluirCelulasTabela(row, 0, null, obs[i].Observacao);
        $.incluirCelulasTabela(row, 1, null, obs[i].DataCadastroObs);
        $.incluirCelulasTabela(row, 2, null, obs[i].ResponsavelObs);

        var botoes = ' <button type="button" class="btn btn-info btn-sm" onclick="AtualizarRowObs(this);"><span class="glyphicon glyphicon-refresh"></span> Atualizar</button>' +
            ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowObs(this);"><span class="glyphicon glyphicon-edit"></span> Excluir</button>';
        $.incluirCelulasTabela(row, 3, null, botoes);
    }
}
function CarregarObs(obs, data, respId, respNome) {
    return {
        Observacao: obs,
        DataCadastroObs: data,
        IdResponsavelObs: respId,
        ResponsavelObs: respNome
    };
}
function ExcluirRowObs(value) {
    $.showYesNoAlert("Deseja excluir a Observação?", "Excluir Observação", function () {
        var i = value.parentNode.parentNode.sectionRowIndex;
        var table = document.getElementById("tbObs").getElementsByTagName("tbody")[0];
        table.deleteRow(i);
        observacoes.splice(i, 1);
        if (observacoes.length == 0) {
            var row = table.insertRow(0);
            $.incluirCelulasTabela(row, 0, "100%", "Não há Observações cadastradas para o Congregação", 5);
        }
    });
}

function HabilitarBotoesTabela(table, enable) {
    $(table).find('tr').each(function (i, row) {
        var $row = $(row);
        $($row).find('.btn').each(function (i, btn) {
            $(btn).prop('disabled', enable);
        })
    })
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

function Arquivo(nomeArq, tamArq, tipo, id, exibirExcluir) {
    let file =
        `<div class="col-sm-5 col-lg-2 col-xl-1">
            <div class="file-man-box">
                ${exibirExcluir == true ? `<a href="javascript:void(0)" onclick="DeleteArquivo('${nomeArq}', ${id});return false;" class="file-close" title="Excluir Arquivo"><i class="glyphicon glyphicon-floppy-remove"></i></a>` : ''}
                <div class="file-img-box"><img src="/images/${tipo}.svg" alt="${nomeArq}" title="${nomeArq}"></div>
                    <div class="file-man-title">
                        <h5 class="mb-0 text-overflow">${nomeArq}</h5>
                        <p class="mb-0"><small>${bytesToSize(tamArq)}</small></p>
                    </div>
                    <a href="javascript:void(0)" onclick="DownloadArquivo('${nomeArq}', ${id});return false;" class="file-download" title="Baixar Arquivo"><i class="glyphicon glyphicon-download  file-img-dowload" style="margin: 10px"></i></a>
                </div>
            </div>`;
    return file;
}

function DownloadArquivo(nomeArq, id) {
    let url = `${urlDownload}?id=${id}&nomeArquivo=${nomeArq}`;
    $.downloadArquivo(url, null);
}

function DeleteArquivo(nomeArq, id) {
    $.showYesNoAlert(`Deseja excluir o arquivo ${nomeArq}?`, "Excluir Arquivo", function () {
        let requestData = {
            id: id,
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

function AddRowGridModal(index, tipo, qtd, congr) {
    var table = $('#modalExcluir table')[0].getElementsByTagName("tbody")[0];
    var row = table.insertRow(table.rows.length);
    row.setAttribute("name", "row{0}".format(index));
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    var cell3 = row.insertCell(2);
    cell1.innerHTML = '<span id="lblTpMemb{0}">{1}</span>'.format(index, tipo);
    cell2.innerHTML = '<span id="lblStatus{0}">{1}</span>'.format(index, qtd);

    let botao = '';
    switch (tipo) {
        case "Membros":
            botao = ' <button type="button" class="btn btn-info btn-sm" onclick="GerarRelMembro({0}, this);"><span class="glyphicon glyphicon-print"></span> Relatório</button>'.format(congr);
            break;
        case "Eventos":
            botao = ' <button type="button" class="btn btn-info btn-sm" onclick="GerarRelEventos({0}, this);"><span class="glyphicon glyphicon-print"></span> Relatório</button>'.format(congr);
            break;
        case "Casamentos":
            botao = ' <button type="button" class="btn btn-info btn-sm" onclick="GerarRelCasamento({0}, this);"><span class="glyphicon glyphicon-print"></span> Relatório</button>'.format(congr);
            break;
        case "Nascimentos":
            botao = ' <button type="button" class="btn btn-info btn-sm" onclick="GerarRelNascimento({0}, this);"><span class="glyphicon glyphicon-print"></span> Relatório</button>'.format(congr);
            break;
        case "Obreiros":
            botao = ' <button type="button" class="btn btn-info btn-sm" onclick="GerarRelObreiros({0}, this);"><span class="glyphicon glyphicon-print"></span> Relatório</button>'.format(congr);
            break;
    }
    cell3.innerHTML = botao;
}

function GerarRelMembro(id, btn) {
    let url = `${urlRelMembros}?congregacaoId=${id}&simplificado=true`;
    $.downloadArquivo(url, $(btn));
};

function GerarRelEventos(id, btn) {
    let url = `${urlRelEventos}?congregacao=${id}&tipoEvento=1&simplificado=true`;
    $.downloadArquivo(url, $(btn));
};

function GerarRelCasamento(id, btn) {
    let url = `${urlRelCasamentos}?congregacao=${id}&dataInicio=01-01-2000&dataFinal=01-01-2050`;
    $.downloadArquivo(url, $(btn));
};

function GerarRelNascimento(id, btn) {
    let url = `${urlRelNascimentos}?congregacao=${id}&dataInicio=01-01-2000&dataFinal=01-01-2050`;
    $.downloadArquivo(url, $(btn));
};

function GerarRelObreiros(id, btn) {
    let url = `${urlRelObreiros}?congregacao=${id}`;
    $.downloadArquivo(url, $(btn));
};

function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
}

async function AJAXSubmit() {
    let arquivo = $('#Arquivo').val();
    if (arquivo == null || arquivo == "") {
        $.bsMessageError("Atenção", "Favor selecionar o arquivo para Capturar!");
        $("#Arquivo").focus();
        return;
    }

    if (ValidarArquivoUpload(files)) {
        let requestData = {
            id: $("#Id").val(),
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
                            SubmitFormulario();
                        });
                    }
                    else {
                        SubmitFormulario();
                    }
                }
                else {
                    removerOverlay(null);
                    $.bsMessageError("Atenção", ret.Message);
                }
            }
        });
    }

    return false;
}

function SubmitFormulario() {
    var formData = new FormData();
    formData.append("files", files[0]);
    formData.append("id", $("#Id").val());

    var computedProgress = function (evt) {
        if (evt.lengthComputable) {
            var percentComplete = evt.loaded / evt.total;
            var percentComplete = Math.round(event.loaded * 100 / event.total) + "%";
            $('.percent-progress').html(percentComplete);
            $('.progress-bar').css("width", percentComplete);
        }
    };
    var overlay = gerarOverlay();
    var loadingFrame = gerarLoadingFrame();

    overlay.append(loadingFrame);
    $('body').append(overlay);

    setTimeout(function () {
        overlay.focus();
    }, 0);

    $.ajax({
        url: urlUplArq,
        type: 'POST',
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            xhr.addEventListener("progress", computedProgress, false);

            xhr.upload.addEventListener("progress", computedProgress, false);

            return xhr;
        },
        data: formData,
        async: true,
        error: function (xhr, status, error) {
            console.log(error);
            console.log(xhr.responseText);
        },
        success: function (result) {
            removerOverlay(null);
            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);

            if (result.status == "OK") {
                arquivos = result.data;
                CarregarArquivos(arquivos, $("#Acao").val() == "Update");
                $('#Arquivo').val('');
                $('.size-file').empty();
                $('.percent-progress').html("0%");
                $('.progress-bar').css("width", 0);
            }
            else {
                $.alertError('Upload de Arquivo', result.mensagem);
            }
        },
        complete: function (param) {
            removerOverlay(null);
        },
        cache: false,
        contentType: false,
        processData: false
    });
}

function ValidarArquivoUpload(arquivos) {
    let arquivo = $('#Arquivo').val();
    if (arquivo == null || arquivo == "") {
        $.bsMessageError("Atenção", "Favor selecionar o arquivo para Capturar!");
        $("#Arquivo").focus();
        return false;
    }

    if (arquivos != null && arquivos != undefined && arquivos.length > 0 && arquivos[0].size > tamanhoMax) {
        $.bsMessageError("Atenção", `Arquivo maior que ${bytesToSize(tamanhoMax)}. Tamanho do arquivo selecionado: ${bytesToSize(arquivos[0].size)}`);
        $("#Arquivo").focus();
        return false;
    }
    var extArquivo = arquivo.split('.').pop();

    if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
        $.alertError('Upload de Arquivo', 'Somente arquivos dos Tipos Excel, Word, PDF e ZIP são permitidos para Upload.');
        $('#Arquivo').val('');
        return false;
    }

    return true;
}