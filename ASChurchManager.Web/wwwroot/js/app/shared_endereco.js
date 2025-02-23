﻿
$(function () {
    $("#btnBuscarCEP").click(function () {
        PesquisarCEP($("#Endereco_Cep"), $("#Endereco_Logradouro"), $("#Endereco_Bairro"), $("#Endereco_Cidade"), $("#Endereco_Estado"));
    });

    $("#Endereco_Pais").change(function () {
        $("#Endereco_Cep").val("");
        $("#Endereco_Logradouro").val("");
        $("#Endereco_Numero").val("");
        $("#Endereco_Complemento").val("");
        $("#Endereco_Bairro").val("");
        $("#Endereco_Cidade").val("");
        $("#Endereco_Estado").val("");
        $("#Endereco_BairroEstrangeiro").val("");
        $("#Endereco_CidadeEstrangeiro").val("");
        $("#Endereco_Provincia").val("");
        $("#Endereco_CodigoPostal").val("");

        $("#TelefoneResidencial").val("");
        $("#TelefoneCelular").val("");
        $("#TelefoneComercial").val("");
        $("#TelefoneResidencialEstrangeiro").val("");
        $("#TelefoneCelularEstrangeiro").val("");
        $("#TelefoneComercialEstrangeiro").val("");

        $("#Pessoa_TelefoneResidencial").val("");
        $("#Pessoa_TelefoneCelular").val("");
        $("#Pessoa_TelefoneComercial").val("");
        $("#Pessoa_TelefoneResidencialEstrangeiro").val("");
        $("#Pessoa_TelefoneCelularEstrangeiro").val("");
        $("#Pessoa_TelefoneComercialEstrangeiro").val("");
        LimparErrosEnderecos();

        var opc = $('option:selected', $(this)).text();
        HabilitaCamposEndEstrang(opc != "Brasil");
    });

    let paisStr = $("#Endereco_Pais").val();
    HabilitaCamposEndEstrang(paisStr != "Brasil");
});

function HabilitaCamposEndEstrang(habilita) {
    if (habilita) {
        $("#divCaixaPostal").css("display", "block");
        $("#divCep").css("display", "none");
        $("#divEndBrasil").css("display", "none");
        $("#divEndEstrang").css("display", "block");
        
        $("#divTelResBr").css("display", "none");
        $("#divTelCelBr").css("display", "none");
        $("#divTelComBr").css("display", "none");

        $("#divTelResEst").css("display", "block");
        $("#divTelCelEst").css("display", "block");
        $("#divTelComEst").css("display", "block");
    }
    else {
        $("#divCaixaPostal").css("display", "none");
        $("#divCep").css("display", "block");
        $("#divEndBrasil").css("display", "block");
        $("#divEndEstrang").css("display", "none");

        $("#divTelResBr").css("display", "block");
        $("#divTelCelBr").css("display", "block");
        $("#divTelComBr").css("display", "block");

        $("#divTelResEst").css("display", "none");
        $("#divTelCelEst").css("display", "none");
        $("#divTelComEst").css("display", "none");
    }
}

function LimparErrosEnderecos() {
    let campos = $('span[id*=-error]');
    for (var i = 0; i < campos.length; i++) {
        if (campos[i].id.indexOf("Endereco") > -1) {
            $(campos[i]).text("");
            $(campos[i]).hide()
        }
    }
}