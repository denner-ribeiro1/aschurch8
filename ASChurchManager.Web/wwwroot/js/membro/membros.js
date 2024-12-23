//Por padrão é o validator.defaults é :hidden, colocando ele vazio para não ignorar campos escondidos
//$.validator.defaults.ignore = "";

var dadosCargos = [];
var dadosCursos = [];
var indexObs = -1;
var indexCargo = -1;
var indexSit = -1;
var files;
var fileCurso;
var isreadonly = false;
var urlFechar = "";
var arquivos = [];
var permiteexcluircargomembro = false;
var permiteexcluirobservacaomembro = false;
var permiteexcluirsituacaomembro = false;

$(function () {
    var acao = $("#Acao").val();
    isreadonly = $("#Pessoa_somenteleitura").val().toLowerCase() == "true";

    if (cargos == null || cargos == undefined)
        cargos = [];
    if (obs == null || obs == undefined)
        obs = [];
    if (sit == null || sit == undefined)
        sit = [];


    if ((acao === "Create") || (acao === "Read") ||
        (acao === "Update") || (acao === "Delete")) {

        permiteexcluircargomembro = ($("#PermiteExcluirCargoMembro").val().toLowerCase() == "true");
        permiteexcluirobservacaomembro = ($("#PermiteExcluirObservacaoMembro").val().toLowerCase() == "true");
        permiteexcluirsituacaomembro = ($("#PermiteExcluirSituacaoMembro").val().toLowerCase() == "true");

        if (sit && sit.length > 0) {
            MontarGridSituacao(sit);
        }

        if (cargos && cargos.length > 0) {
            MontarGridCargos(cargos);
        }

        if (obs && obs.length > 0) {
            MontarGridObs(obs);
        }

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
            MontarGridCursos();
        }

        /************************************************************************************************************
        * SITUAÇÃO 
        ************************************************************************************************************/
        $(document).on('click', '#btnCancSit', {}, function (e) {
            e.preventDefault();

            $("#IdSit").val("");
            $("#DataSit").val("");
            $("#ObservacaoSit").val("");
            indexSit = -1;
            $("#btnAddSit").val("Adicionar");
            if (permiteexcluirsituacaomembro && !isreadonly)
                HabilitarBotoesTabela($(document.getElementById("tbSit").getElementsByTagName("tbody")), false);
        })

        $(document).on('click', '#btnAddSit', {}, function (e) {
            e.preventDefault();
            var dadosSit = CarregarDadosSituacao($("#IdSit").val(), $('#IdSit :selected').text(), $("#DataSit").val(), $("#ObservacaoSit").val(), true);


            var errorSit = $(document).find("[data-valmsg-for='IdSit']");
            errorSit.html('');

            var errordata = $(document).find("[data-valmsg-for='DataSit']");
            errordata.html('');


            var error = false;
            if (dadosSit.IdSit == null || dadosSit.IdSit == undefined || dadosSit.IdSit == 0) {
                error = true;
                errorSit.html('<span id="IdSit-error" class="">O campo Situação é obrigatório</span>');
            }

            if (dadosSit.DataSit == null || dadosSit.DataSit == undefined || dadosSit.DataSit == 0) {
                error = true;
                errordata.html('<span id="DataSit-error" class="">O campo Data é obrigatório</span>');

            }
            if (dadosSit.DataSit != null && dadosSit.DataSit != undefined && dadosSit.DataSit != "") {
                var objDate = new Date();
                objDate.setYear(dadosSit.DataSit.split("/")[2]);
                objDate.setMonth(dadosSit.DataSit.split("/")[1] - 1);//- 1 pq em js é de 0 a 11 os meses
                objDate.setDate(dadosSit.DataSit.split("/")[0]);

                if (objDate.getTime() > new Date().getTime()) {
                    errordata.html('<span id="DataSit-error" class="">A Data da Situação deve ser menor que a Data Atual</span>');
                    error = true;
                }
            }

            if (error) {
                return false;
            }
            if (indexSit > -1) {
                sit.splice(indexSit, 1);
                indexSit = -1;
            }
            sit.push(dadosSit);

            sit.sort(function (a, b) {
                var dtS = a.DataSit.split("/");
                var dt1 = new Date(dtS[2], dtS[1] - 1, dtS[0]);
                dtS = b.DataSit.split("/");
                var dt2 = new Date(dtS[2], dtS[1] - 1, dtS[0]);

                return ((dt1 < dt2) ? 1 : ((dt1 > dt2) ? -1 : 0));
            });
            MontarGridSituacao(sit);

            $("#btnAddSit").val("Adicionar");
            $("#IdSit").val("");
            $("#DataSit").val("");
            $("#ObservacaoSit").val("");
            if (permiteexcluirsituacaomembro && !isreadonly)
                HabilitarBotoesTabela($(document.getElementById("tbSit").getElementsByTagName("tbody")), false);
        })

        /************************************************************************************************************
         * CARGO 
         ************************************************************************************************************/
        $("#Confradesp").prop("disabled", true);
        $("#CGADB").prop("disabled", true);

        $.ajaxPost("/Secretaria/Membro/DadosCargos", null, function (c) {
            dadosCargos = c;
        });

        $(document).on('click', '#btnCancCargo', {}, function (e) {
            e.preventDefault();

            $("#CargoId").val("");
            $("#DataCargo").val("");
            $("#LocalConsagracao").val("");
            $("#Confradesp").val("");
            $("#CGADB").val("");
            $("#Confradesp").prop("disabled", true);
            $("#CGADB").prop("disabled", true);
            indexCargo = -1;

            $("#btnAddCargo").val("Adicionar");
            if (permiteexcluircargomembro && !isreadonly)
                HabilitarBotoesTabela($(document.getElementById("tbCargos").getElementsByTagName("tbody")), false);
        })

        $(document).on('click', '#btnAddCargo', {}, function (e) {
            e.preventDefault();
            var dadosCargo = CarregarDadosCargos($("#CargoId").val(), $('#CargoId :selected').text(), $("#LocalConsagracao").val(), $("#DataCargo").val(), $("#Confradesp").val(), $("#CGADB").val(), true);

            var errorCargo = $(document).find("[data-valmsg-for='CargoId']");
            errorCargo.html('');

            var errordata = $(document).find("[data-valmsg-for='DataCargo']");
            errordata.html('');

            var error = false;
            if (dadosCargo.CargoId == null || dadosCargo.CargoId == undefined || dadosCargo.CargoId == 0) {
                error = true;
                errorCargo.html('<span id="CargoId-error" class="">O campo Cargo é obrigatório</span>');
            }

            if (dadosCargo.DataCargo == null || dadosCargo.DataCargo == undefined || dadosCargo.DataCargo == 0) {
                error = true;
                errordata.html('<span id="DataCargo-error" class="">O campo Data do Cargo é obrigatório</span>');
            }

            if (dadosCargo.DataCargo != null && dadosCargo.DataCargo != undefined && dadosCargo.DataCargo != "") {
                var objDate = new Date();
                objDate.setYear(dadosCargo.DataCargo.split("/")[2]);
                objDate.setMonth(dadosCargo.DataCargo.split("/")[1] - 1);//- 1 pq em js é de 0 a 11 os meses
                objDate.setDate(dadosCargo.DataCargo.split("/")[0]);

                if (objDate.getTime() > new Date().getTime()) {
                    errordata.html('<span id="DataCargo-error" class="">A Data do Cargo deve ser menor que a Data Atual</span>');
                    error = true;
                }
            }

            if (error) {
                return false;
            }

            if (indexCargo > -1) {
                cargos.splice(indexCargo, 1);
                indexCargo = -1;
            }
            cargos.push(dadosCargo);
            cargos.sort(function (a, b) {
                var dtS = a.DataCargo.split("/");
                var dt1 = new Date(dtS[2], dtS[1] - 1, dtS[0]);
                dtS = b.DataCargo.split("/");
                var dt2 = new Date(dtS[2], dtS[1] - 1, dtS[0]);

                return ((dt1 < dt2) ? 1 : ((dt1 > dt2) ? -1 : 0));
            });
            MontarGridCargos(cargos);


            $("#btnAddCargo").val("Adicionar");
            $("#CargoId").val("");
            $("#DataCargo").val("");
            $("#LocalConsagracao").val("");
            $("#Confradesp").val("");
            $("#CGADB").val("");
            $("#Confradesp").prop("disabled", true);
            $("#CGADB").prop("disabled", true);
            if (permiteexcluircargomembro && !isreadonly)
                HabilitarBotoesTabela($(document.getElementById("tbCargos").getElementsByTagName("tbody")), false);
        })

        $("#CargoId").blur(function () {
            var value = $(this).val();
            var hab = EncontrarItem(dadosCargos, function (f) {
                return f.Id == value
            });

            if (hab != undefined && hab != null && hab.length > 0) {
                if (hab[0].Confradesp) {
                    $("#Confradesp").prop("disabled", false);
                }
                else {
                    $("#Confradesp").prop("disabled", true);
                    $("#Confradesp").val("");
                }
                if (hab[0].CGADB) {
                    $("#CGADB").prop("disabled", false);
                }
                else {
                    $("#CGADB").prop("disabled", true);
                    $("#CGADB").val("");
                }
            }
            else {
                $("#Confradesp").prop("disabled", true);
                $("#Confradesp").val("");
                $("#CGADB").prop("disabled", true);
                $("#CGADB").val("");
            }

        })

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


        /************************************************************************************************************
        * CURSOS
        ************************************************************************************************************/
        $("#Cadastrado").change(function () {
            LimparCamposCurso();
            $("#CursoId").val("");
            var opc = $('option:selected', $(this)).text();
            HabilitarCamposCurso(opc == "Sim");
        });

        HabilitarCamposCurso(false);

        $.ajaxPost("/Secretaria/Membro/DadosCursos", null, function (c) {
            dadosCursos = c;
        });

        $('#CursoId').on('change', function (e) {
            var id = $('option:selected', $("#CursoId")).val();
            CarregarDadosCurso(id);
        });

        $('#ArquivoCurso').on('change', function (e) {
            var extPermitidas = ['pdf'];
            var extArquivo = this.value.split('.').pop();

            if (typeof extPermitidas.find((ext) => extArquivo.toLowerCase() == ext) == 'undefined') {
                $.alertError('Upload de Arquivo', 'Somente arquivos do Tipo PDF são permitidos para Upload.');
                this.value = '';
            }
            else {
                fileCurso = e.target.files;
            }
        });

        $(document).on('click', '#btnAddCurso', {}, function (e) {
            var tamanhoMax = 2097152; // 2MB EM BYTES
            var membroId = $('#Id').val();
            var opc = $('option:selected', $("#Cadastrado")).text();

            var erroCursoId = $(document).find("[data-valmsg-for='CursoId']");
            erroCursoId.html("");
            var errolocal = $(document).find("[data-valmsg-for='Local']");
            errolocal.html("");
            var erronomeCurso = $(document).find("[data-valmsg-for='NomeCurso']");
            erronomeCurso.html("");
            var errodataInicioCurso = $(document).find("[data-valmsg-for='DataInicioCurso']");
            errodataInicioCurso.html("");
            var errodataEncerramentoCurso = $(document).find("[data-valmsg-for='DataEncerramentoCurso']");
            errodataEncerramentoCurso.html("");
            var errocargaHoraria = $(document).find("[data-valmsg-for='CargaHoraria']");
            errocargaHoraria.html("");
            var erroarquivoCurso = $(document).find("[data-valmsg-for='ArquivoCurso']");
            erroarquivoCurso.html("");

            var cursoId = 0;
            var local = $("#Local").val();
            var nomeCurso = $("#NomeCurso").val();
            var dataInicioCurso = $("#DataInicioCurso").val();
            var dataEncerramentoCurso = $("#DataEncerramentoCurso").val();
            var cargahoraria = $("#CargaHoraria").val();
            var erro = false;

            if (opc == "Sim") {
                cursoId = $('option:selected', $("#CursoId")).val();
                if (cursoId == 0) {
                    erroCursoId.html('<span id="DescricaoArquivo-error" class="">Curso é obrigatório</span>');
                    erro = true;
                }
            } else {
                var dtIniArr = $("#DataInicioCurso").val().split("/");
                var dtIni = new Date(dtIniArr[1] + "/" + dtIniArr[0] + "/" + dtIniArr[2]);

                var dtFimArr = $("#DataEncerramentoCurso").val().split("/");
                var dtEnc = new Date(dtFimArr[1] + "/" + dtFimArr[0] + "/" + dtFimArr[2]);


                if (local == null || local == "" || local == undefined) {
                    errolocal.html('<span id="Local-error" class="">Local do curso é obrigatório</span>');
                    erro = true;
                }

                if (nomeCurso == null || nomeCurso == "" || nomeCurso == undefined) {
                    erronomeCurso.html('<span id="NomeCurso-error" class="">Nome do curso é obrigatório</span>');
                    erro = true;
                }

                if (dataInicioCurso == null || dataInicioCurso == "" || dataInicioCurso == undefined) {
                    errodataInicioCurso.html('<span id="DataInicioCurso-error" class="">Data Início do curso é obrigatório</span>');
                    erro = true;
                }

                if (dataEncerramentoCurso == null || dataEncerramentoCurso == "" || dataEncerramentoCurso == undefined) {
                    errodataEncerramentoCurso.html('<span id="DataEncerramentoCurso-error" class="">Data de Encerramento do curso é obrigatório</span>');
                    erro = true;
                }

                if (dtEnc < dtIni) {
                    errodataEncerramentoCurso.html('<span id="DataEncerramentoCurso-error" class="">Data de Encerramento deve ser maior ou igual a Data de Início.</span>');
                    erro = true;
                }

                if (cargahoraria == null || cargahoraria == "" || cargahoraria == undefined || parseInt(cargahoraria) == 0) {
                    errocargaHoraria.html('<span id="CargaHoraria-error" class="">Carga Horária é obrigatório</span>');
                    erro = true;
                }

                if (fileCurso != null && fileCurso != undefined && fileCurso.length > 0 && fileCurso[0].size > tamanhoMax) {
                    erroarquivoCurso.html('<span id="ArquivoCurso-error" class="">Arquivo maior que 2 MB. Tamanho do arquivo selecionado: {0}.</span>'.format(bytesToSize(fileCurso[0].size)));
                    erro = true;
                }
            }

            if (!erro) {
                var file = null;
                var filename = null;
                var filesize = null;
                if (fileCurso != null && fileCurso != undefined && fileCurso.length > 0) {
                    file = fileCurso[0];
                    filename = file.name;
                    filesize = file.size;
                }
                arquivos.push(CarregarDadosArquivo(true, opc, 0, membroId, cursoId, nomeCurso, local, dataInicioCurso, dataEncerramentoCurso, cargahoraria, null, file, null, filename, filesize, false, false, gera_id()));
                MontarGridCursos();
                LimparCamposCurso(true);
            }
        });

        $(document).on('click', '#btnCancCurso', {}, function (e) {
            LimparCamposCurso(true);
            $("#CursoId").val("");
        });

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
        PesquisarCongregacao(value);
    });

            

    /************************************************************************************************************
    * BATISMO
    ************************************************************************************************************/
    $("#DataPrevistaBatismo").blur(function () {
        var data = $(this).val();

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

    /************************************************************************************************************
    * APROVAÇÃO/REPROVAÇÃO DE MEMBRO
    ************************************************************************************************************/
    $(document).on('click', '#btnReprovar', {}, function (e) {
        e.preventDefault();

        var requestData = {
            Id: $(this).data("membroid")
        };

        var url = "/Secretaria/Membro/ReprovarMembroMotivo/" + requestData.Id;
        $.popUpUrl(url, 440, 640);
    });

    $(document).on('click', '#btnAprovarMembro', {}, function (e) {
        e.preventDefault();
        $.showYesNoAlert("Deseja realmente Aprovar o membro?", "Membro", function () {
            var request = {
                id: $("#Id").val()
            };
            $.ajaxPost(urlAprovarMembro, request, function (response) {
                if (response.status === "OK") {
                    window.location.href = response.url;
                }
                else {
                    $.alertError("Membro", response.mensagem);
                }
            });
        });
    });

    /************************************************************************************************************
    * IMPRESSÃO DE CARTEIRINHAS E FICHA
    ************************************************************************************************************/
    $(document).on('click', '#lnkCarteirinha', {}, function (e) {
        e.preventDefault();

        var id = $(this).data("membroid");
        GerarCarteirinha(id, $(this));
    });

    $("#lnkFichaMembro").click((e) => {
        e.preventDefault();
        Swal.fire({
            title: '<strong>Membro</strong>',
            html: "Deseja imprimir a Ficha de Membro?",
            icon: 'question',
            input: 'checkbox',
            inputValue: 0,
            inputPlaceholder: 'Imprimir Cursos/Eventos do Membro',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: '<i class="fa fa-check-circle"></i> Sim',
            cancelButtonText: '<i class="fa fa-ban"></i> Não',
            width: 500,
            allowOutsideClick: false,
        }).then((result) => {
            if (result.value === 0 || result.value) {
                let id = $("#lnkFichaMembro").data("membroid");
                let url = `${urlFichaMembro}?id=${id}&imprimirCurso=${result.value === 1}`;
                $.downloadArquivo(url, $("#lnkFichaMembro"));
            }
        })
    });

    /************************************************************************************************************
    * IMPRESSÃO DE CARTEIRINHAS E FICHA
    ************************************************************************************************************/
    $(document).on('click', '#lnkExcluir', {}, function (e) {
        e.preventDefault();

        var request = { id: $(this).data("membroid") };
        $.showYesNoAlert('<h3 style="color:red">Você tem certeza que deseja excluir este Membro?</br>Após a confirmação NÃO será mais possível recuperar os dados e nem os arquivos salvos para o Membro.</h3>', "Membro",
            function () {
                $.ajaxPost(urlExcluir, request, function (response) {
                    if (response.status == "OK") {
                        window.location = response.url;
                    }
                    else {
                        $.alertError("Membro", response.mensagem);
                    }
                });
            });
    });

    $(document).on('click', '#btnSalvar', {}, function (e) {
        e.preventDefault;
        var msg = "Deseja realmente criar um novo Membro?";
        if ($("#Acao").val() == "Update") {
            msg = "Deseja realmente atualizar o Membro?";
        }
        $.showYesNoAlert(msg, "Membro", function () {
            var form = $("#frmMembro");
            //form.data("validator").settings.ignore = "";
            form.validate();
            if (form.valid()) {
                $("#Obs").val(JSON.stringify(obs));
                $("#Sit").val(JSON.stringify(sit));
                $("#Carg").val(JSON.stringify(cargos));

                if (!ExistsFileToSave()) {
                    $.ajaxPostForm(urlSubmit, $("#frmMembro"), function (result) {
                        if (result.status === "OK") {
                            window.location = result.url;
                        }
                        else {
                            $.alertError("Membro", result.msg);
                        }
                    });
                }
                else {
                    var table = $('#modalSalvar table')[0].getElementsByTagName("tbody")[0];
                    $(table).empty();

                    $("#modalSalvar").modal({ backdrop: "static" });
                    AddRowGridModal(0, "Salvando os dados do Membro")

                    $.ajaxPostForm(urlSubmit, $("#frmMembro"), function (result) {
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
                                        if ((arquivos[x].ArquivoCurso != null && arquivos[x].ArquivoCurso != undefined) ||
                                            (arquivos[x].Arquivo != null && arquivos[x].Arquivo != undefined)) {
                                            if (arquivos[x].IsCurso)
                                                formData.append("files", arquivos[x].ArquivoCurso);
                                            else
                                                formData.append("files", arquivos[x].Arquivo);
                                        }

                                        var cadastrado = null;
                                        if (arquivos[x].IsCurso) {
                                            if (arquivos[x].Cadastrado == "Não")
                                                cadastrado = 0;
                                            else
                                                cadastrado = 1;
                                        }
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
                                                if (arquivos[x].IsCurso) {
                                                    if (arquivos[x].IsDelete)
                                                        AddRowGridModal(arquivos[x].Index, 'Excluindo o Curso <b>"{0}"</b>'.format(arquivos[x].NomeCurso));
                                                    else
                                                        AddRowGridModal(arquivos[x].Index, 'Salvando o Curso <b>"{0}"</b>'.format(arquivos[x].NomeCurso));
                                                }
                                                else {
                                                    if (arquivos[x].IsDelete)
                                                        AddRowGridModal(arquivos[x].Index, 'Excluindo o Documento <b>"{0}"</b>'.format(arquivos[x].DescricaoArquivo));
                                                    else
                                                        AddRowGridModal(arquivos[x].Index, 'Salvando o Documento <b>"{0}"</b>'.format(arquivos[x].DescricaoArquivo));
                                                }
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
            }
            else {
                var msg = ValidarFormulario();
                if (msg != null && msg != undefined && msg != "")
                    $.alertError("Membro", msg);
            }
        });
    })

    $(document).on('click', '#btnFecharModal', {}, function (e) {
        if (urlFechar != "" && urlFechar != null && urlFechar != undefined) {
            e.preventDefault;
            window.location = urlFechar;
        }
    });

    $(document).on('click', '#btnEnviarBatismo', {}, function (e) {
        e.preventDefault;

        $.showYesNoAlert("Deseja realmente enviar o Membro para o Batismo?", "Envio para o Batismo", function () {
            var form = $("#frmMembro");
            //form.data("validator").settings.ignore = "";
            form.validate();
            if (form.valid()) {
                $.ajaxPostForm(urlEnvBatismo, $("#frmMembro"), function (result) {
                    if (result.status === "OK") {
                        window.location = result.url;
                    }
                    else {
                        $.alertError("Envio para o Batismo", result.msg);
                    }
                });
            }
            else {
                var msg = ValidarFormEnvioBatismo();
                if (msg !== null && msg !== undefined && msg !== "")
                    $.alertError("Envio para o Batismo", msg);
            }
        });
    });

    $("#btnMembroConfirmado").click((e) => {
        let id = $("#Id").val();
        let url = `${urlMembroConfirmado}?id=${id}`;
        $.popUpUrl(url, 600, 700);
    });
});

function PesquisarCongregacao(value) {
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
}

function ValidarFormEnvioBatismo() {
    var erro = "";
    $('span[id*=-error]').each(function (x, item) {
        if ($(item).text() != "") {
            erro += $(item).text() + "<br>";
        }
    });
    return erro;
}
function ValidarFormulario() {
    var erro = "";
    $(".tab-pane").find('span[id*=-error]').each(function (x, item) {
        if ($(item).text() != "") {
            erro += $(item).text() + "<br>";
        }
    });
    return erro;
}
function formatJsonToDate(dataJSON) {
    var d = dataJSON.substring(0, 10).split("-");
    return d[2] + "/" + d[1] + "/" + d[0];
}
function formatJsonToTime(dataJSON) {
    var dt = new Date(parseInt(dataJSON.substr(6))).toUTCString();
    return dt.substring(17, 22);
}
function GerarCarteirinha(id, btn) {
    var requestData = {
        Id: id
    };
    $.ajaxPost("/Secretaria/Membro/VerificaVencimentoCarteirinha", requestData, function (data) {
        if (data.DataValidade != "" && data.DataValidade != null) {
            $.showYesNoAlert("A Carteirinha do Membro vence em " + data.DataValidade + ". Deseja gerar um novo vencimento para ela?", "Carteirinha Membro", function () {
                var url = "/Secretaria/Membro/CarteirinhaMembros?id=" + id.toString() + "&atualizaValidade=true";
                $.downloadArquivo(url, btn);

            }, null, function () {
                var url = "/Secretaria/Membro/CarteirinhaMembros?id=" + id.toString() + "&atualizaValidade=false";
                $.downloadArquivo(url, btn);
            }, null);
        }
        else {
            var url = "/Secretaria/Membro/CarteirinhaMembros?id=" + id.toString() + "&atualizaValidade=false";
            $.downloadArquivo(url, btn);
        }
    });
}
function buscarMembro(requestId, elementTargetId) {
    if (requestId > 0) {
        var requestData = {
            MembroId: requestId
        };
        elementTargetId = "#" + elementTargetId;

        $.ajaxPost("/Secretaria/Membro/BuscarMembro", requestData, function (membro) {
            $(elementTargetId).val("");
            if (membro.Id == 0) {
                $.bsMessageError("Atenção", "Membro não cadastrado!");
                $(this).val("");
            } else if (membro.Id > 0) {
                $(elementTargetId).val(membro.Nome);
            }
        });
    }
}
function ExcluirRowSit(value) {
    $.showYesNoAlert("Deseja excluir a Situação?", "Excluir Situação",
        function () {
            var i = value.parentNode.parentNode.sectionRowIndex;
            var table = document.getElementById("tbSit").getElementsByTagName("tbody")[0];
            table.deleteRow(i);
            sit.splice(i, 1);
            if (sit.length == 0) {
                var row = table.insertRow(0);
                row.className = "jtable-data-row";
                IncluirCelulasTabela(row, 0, "100%", "Não há situação cadastrada para o Membro", permiteexcluirsituacaomembro ? 4 : 3);
            }
        });
}
function AtualizarRowSit(value) {
    var i = value.parentNode.parentNode.sectionRowIndex;
    indexSit = i;
    $("#IdSit").val(sit[i].IdSit);
    $("#DataSit").val(sit[i].DataSit);
    $("#ObservacaoSit").val(sit[i].ObservacaoSit);
    $("#btnAddSit").val("Atualizar");
    $("#IdSit").focus();

    HabilitarBotoesTabela($(document.getElementById("tbSit").getElementsByTagName("tbody")), true);
}
function CarregarDadosSituacao(sit, descr, data, observacao, mostrarBotao) {
    return {
        IdSit: sit,
        DescricaoSit: descr,
        DataSit: data,
        ObservacaoSit: observacao,
        MostrarBotao: mostrarBotao
    }
}
function MontarGridSituacao(situacao) {
    var table = document.getElementById("tbSit").getElementsByTagName("tbody")[0];
    $(table).empty();

    for (var i = 0; i < situacao.length; i++) {
        var row = table.insertRow(i);
        row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
        IncluirCelulasTabela(row, 0, null, situacao[i].DescricaoSit);
        IncluirCelulasTabela(row, 1, null, situacao[i].DataSit);
        IncluirCelulasTabela(row, 2, null, situacao[i].ObservacaoSit);
        if (situacao[i].MostrarBotao || (permiteexcluirsituacaomembro && !isreadonly)) {
            var botoes = ' <button type="button" class="btn btn-info btn-sm" onclick="AtualizarRowSit(this);"><span class="glyphicon glyphicon-refresh"></span> Atualizar</button>' +
                ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowSit(this);"><span class="glyphicon glyphicon-trash"></span> Excluir</button>';
            IncluirCelulasTabela(row, 3, null, botoes);
        }
    }
}

function ExcluirRowCargos(value) {
    $.showYesNoAlert("Deseja excluir o Cargo?", "Excluir Cargo", function () {
        var i = value.parentNode.parentNode.sectionRowIndex;
        var table = document.getElementById("tbCargos").getElementsByTagName("tbody")[0];
        table.deleteRow(i);
        cargos.splice(i, 1);
        if (cargos.length == 0) {
            var row = table.insertRow(0);
            row.className = "jtable-data-row";
            IncluirCelulasTabela(row, 0, "100%", "Não há cargos cadastrados para o Membro", permiteexcluircargomembro ? 6 : 5);
        }
    });
}
function AtualizarRowCargos(value) {
    var i = value.parentNode.parentNode.sectionRowIndex;
    indexCargo = i;
    $("#CargoId").val(cargos[i].CargoId);
    $("#CargoId").blur();
    $("#DataCargo").val(cargos[i].DataCargo);
    $("#LocalConsagracao").val(cargos[i].LocalConsagracao);
    $("#Confradesp").val(cargos[i].Confradesp);
    $("#CGADB").val(cargos[i].CGADB);
    $("#btnAddCargo").val("Atualizar");
    $("#CargoId").focus();
    HabilitarBotoesTabela($(document.getElementById("tbCargos").getElementsByTagName("tbody")), true);
}
function CarregarDadosCargos(cargoid, descr, loccons, datacargo, confradesp, cgadb, mostrarBotao) {
    return {
        CargoId: cargoid,
        DescricaoCargo: descr,
        LocalConsagracao: loccons,
        DataCargo: datacargo,
        Confradesp: confradesp,
        CGADB: cgadb,
        MostrarBotao: mostrarBotao
    }
}
function MontarGridCargos(cargo) {
    var table = document.getElementById("tbCargos").getElementsByTagName("tbody")[0];
    $(table).empty();

    for (var i = 0; i < cargo.length; i++) {
        var row = table.insertRow(i);
        row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");
        IncluirCelulasTabela(row, 0, null, cargo[i].DescricaoCargo);
        IncluirCelulasTabela(row, 1, null, cargo[i].LocalConsagracao);
        IncluirCelulasTabela(row, 2, null, cargo[i].DataCargo);
        IncluirCelulasTabela(row, 3, null, cargo[i].Confradesp);
        IncluirCelulasTabela(row, 4, null, cargo[i].CGADB);
        if (cargo[i].MostrarBotao || (permiteexcluircargomembro && !isreadonly)) {
            var botoes = ' <button type="button" class="btn btn-info btn-sm" onclick="AtualizarRowCargos(this);"><span class="glyphicon glyphicon-refresh"></span> Atualizar</button>' +
                ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowCargos(this);"><span class="glyphicon glyphicon-trash"></span> Excluir</button>';
            IncluirCelulasTabela(row, 5, null, botoes);
        }
    }
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
        IncluirCelulasTabela(row, 0, "100%", "Não há Documentos cadastrados para o Membro", 4);
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

function HabilitarCamposCurso(hab) {
    if (hab) {
        $("#CursoId").removeAttr("disabled");
        $("#Local").attr("disabled", "disabled");
        $("#NomeCurso").attr("disabled", "disabled");
        $("#DataInicioCurso").attr("disabled", "disabled");
        $("#DataEncerramentoCurso").attr("disabled", "disabled");
        $("#CargaHoraria").attr("disabled", "disabled");
        $("#ArquivoCurso").removeAttr("disabled");
    }
    else {
        $("#CursoId").attr("disabled", "disabled");
        $("#Local").removeAttr("disabled");
        $("#NomeCurso").removeAttr("disabled");
        $("#DataInicioCurso").removeAttr("disabled");
        $("#DataEncerramentoCurso").removeAttr("disabled");
        $("#CargaHoraria").removeAttr("disabled");
        $("#ArquivoCurso").removeAttr("disabled");
    }
}

function CarregarDadosCurso(id) {
    LimparCamposCurso();
    if (id != null && id != undefined && id != "" && parseInt(id) != 0) {
        var dados = EncontrarItem(dadosCursos, function (f) {
            return f.Id == id
        });

        if (dados.length > 0) {
            $("#Local").val("Sede");
            $("#NomeCurso").val(dados[0].Descricao);
            $("#DataInicioCurso").val(formatJsonToDate(dados[0].DataInicio));
            $("#DataEncerramentoCurso").val(formatJsonToDate(dados[0].DataEncerramento));
            $("#CargaHoraria").val(dados[0].CargaHoraria);
        }
        else {
            $.alertError("Membro", "Não foi encontrado os dados para o Curso");
        }
    }
}
function LimparCamposCurso(limpar) {
    if (limpar) {
        $(document).find("[data-valmsg-for='Cadastrado']").html("");
        $(document).find("[data-valmsg-for='CursoId']").html("");
    }
    $(document).find("[data-valmsg-for='Local']").html("");
    $(document).find("[data-valmsg-for='NomeCurso']").html("");
    $(document).find("[data-valmsg-for='DataInicioCurso']").html("");
    $(document).find("[data-valmsg-for='DataEncerramentoCurso']").html("");
    $(document).find("[data-valmsg-for='CargaHoraria']").html("");
    $(document).find("[data-valmsg-for='ArquivoCurso']").html("");

    if (limpar) {
        $("#Cadastrado").val("0");
        HabilitarCamposCurso(false);
        $("#CursoId").val("");
    }
    $("#Local").val("");
    $("#NomeCurso").val("");
    $("#DataInicioCurso").val("");
    $("#DataEncerramentoCurso").val("");
    $("#CargaHoraria").val("");
    $("#ArquivoCurso").val("");
}
function CarregarCurso(cursoid, local, nomecurso, datainicurso, dataenccurso, cargahoraria, arquivo, id) {
    return {
        CursoId: cursoid,
        Local: local,
        NomeCurso: nomecurso,
        DataInicioCurso: datainicurso,
        DataEncerramentoCurso: dataenccurso,
        CargaHoraria: cargahoraria,
        ArquivoCurso: arquivo,
        Id: id
    }
}
function MontarGridCursos() {
    var cursos = EncontrarItem(arquivos, function (c) {
        return c.IsCurso && c.IsDelete == false
    });

    var table = document.getElementById("tbCursos").getElementsByTagName("tbody")[0];
    $(table).empty();

    var isreadonly = $('#IsReadOnly').val().toLowerCase() == "true";
    if (cursos == undefined || cursos.length == 0) {
        var row = table.insertRow(0);
        row.className = "jtable-data-row";

        IncluirCelulasTabela(row, 0, "100%", "Não há Cursos cadastrados para o Membro", 6);
    }
    else {
        for (var i = 0; i < cursos.length; i++) {
            var row = table.insertRow(i);
            row.className = "jtable-data-row" + (i % 2 == 0 ? " jtable-row-even" : "");

            IncluirCelulasTabela(row, 0, null, cursos[i].NomeCurso);
            IncluirCelulasTabela(row, 1, null, cursos[i].Local);
            IncluirCelulasTabela(row, 2, null, cursos[i].DataInicioCurso);
            IncluirCelulasTabela(row, 3, null, cursos[i].DataEncerramentoCurso);
            IncluirCelulasTabela(row, 4, null, cursos[i].CargaHoraria);
            var botao = "";

            if (!isreadonly)
                if (cursos[i].IsSave)
                    botao += ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowCurso(this,{0});"><span class="glyphicon glyphicon-trash"></span> Excluir</button>'.format(cursos[i].Id);
                else
                    botao += ' <button type="button" class="btn btn-danger btn-sm" onclick="ExcluirRowCurso(this,{0});"><span class="glyphicon glyphicon-trash"></span> Excluir</button>'.format(cursos[i].Index);

            if (cursos[i].IsSave && cursos[i].NomeOriginal != null && cursos[i].NomeOriginal != undefined && cursos[i].NomeOriginal != "")
                botao += ' <button type="button" class="btn btn-info btn-sm" onclick="DownloadRowCurso(this,{0});"><span class="glyphicon glyphicon-download-alt"></span> Download</button>'.format(cursos[i].Id);

            IncluirCelulasTabela(row, 5, null, botao);
        }
    }
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

function AddRowGridModal(index, message) {
    var table = $('#modalSalvar table')[0].getElementsByTagName("tbody")[0];
    var row = table.insertRow(table.rows.length);
    row.setAttribute("name", "row{0}".format(index));
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    cell1.innerHTML = '<span id="lbl{0}">{1}</span>'.format(index, message);
    cell2.innerHTML = '<div id="div{0}"><div class="as-reload-img"><div></div>'.format(index);
}

function ExcluirRowCurso(value, index) {
    $.showYesNoAlert("Deseja excluir o Curso?", "Excluir Curso", function () {
        ExcluirArquivo(index);
        MontarGridCursos();
    });
}
function ExcluirArquivo(index) {
    var posicao = EncontrarIndice(arquivos, function (f) {
        return f.Index == index;
    });
    if (posicao > -1) {
        arquivos[posicao].IsDelete = true;
    }
}

function DownloadRowCurso(value, index) {
    Download(index)
}
function Download(index, btn) {
    var url = "/Secretaria/Membro/DownloadArquivo?id={0}".format(index);
    $.downloadArquivo(url, btn);

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
function HabilitarBotaoModal(arquivo) {
    count = arquivo.reduce(function (r, a) {
        return r + + (a.IsDone);
    }, 0);
    return arquivo.length == count;
}

function ExistsFileToSave() {
    for (var i = 0; i < arquivos.length; i++) {
        if ((arquivos[i].IsDelete && arquivos[i].IsSave) || (!arquivos[i].IsDelete && !arquivos[i].IsSave)) {
            return true;
        }
    }
    return false;
}