var mes31 = [1, 3, 5, 7, 8, 10, 12];
var isShift = false;
var separator = "/";


$(function () {
    $.validator.setDefaults({
        ignore: []
    });


    "use strict";

    /// --------------------------------------------------Configuração para campos de formulário
    $('body').on('submit', 'form:not([data-sem-bloqueio])', function (e) {
        if (e.isDefaultPrevented())
            return true;

        $('*:focus').blur();

        var overlay = gerarOverlay();
        var loadingFrame = gerarLoadingFrame();

        overlay.append(loadingFrame);
        $('body').append(overlay);

        setTimeout(function () {
            overlay.focus();
        }, 0);
    });
    //$("input[type='text']").css("text-transform", "uppercase");

    $("input[type='checkbox'], input[type='radio']").iCheck({
        checkboxClass: 'icheckbox_square',
        radioClass: 'iradio_square'
    });

    $('.datepicker').datepicker({
        format: "dd/mm/yyyy",
        todayHighlight: true
    }).on('focus', function () {
        $(this).select();
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    }).keypress(function (e) {
        return IsDate(this, e.keyCode);
    }).blur(function (e) {
        if ($(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
            var dataDig = $(this).val().split(separator);

            var dataAtual = [];
            dataAtual.push(new Date().getDate());
            dataAtual.push(new Date().getMonth() + 1);
            dataAtual.push(new Date().getFullYear());

            if (dataDig.length < 3) {
                for (var i = 0; i < dataDig.length; i++) {
                    if (dataDig[i] > 0 && dataDig[i] < 10)
                        dataAtual[i] = Right("0" + dataDig[i], 2);
                    else if (dataDig[i] != 0)
                        dataAtual[i] = dataDig[i];
                    else {
                        if (dataAtual[i] < 10)
                            dataAtual[i] = Right("0" + dataAtual[i], 2);
                    }
                }
                $(this).val(dataAtual[0] + "/" + dataAtual[1] + "/" + dataAtual[2]);
            }
            else if (dataDig.length == 3) {
                if (dataDig[2].length < 4) {
                    dataAtual[2] = ((dataDig[2] <= 20) ? "20" : "19") + Right("0" + dataDig[2], 2);
                    $(this).val(dataDig[0] + "/" + dataDig[1] + "/" + dataAtual[2]);
                }
            }
        }
    });

    $('.datepicker').each(function (i, e) {
        var data = $(e).val();
        if (data != null && data != "") {
            $(e).val(data.substring(0, 10));
        }
    });

    $(".datetime").mask("99/99/9999 99:99:99");

    $(".capitalizedCase").keypress(function (e) {
        var str = String.fromCharCode(e.which);
        $(this).capitalize(str.toLowerCase());
    });

    $(".lowerCase").keyup(function (e) {
        this.value = this.value.toLowerCase();
    });

    $(".upperCase").keyup(function (e) {
        this.value = this.value.toUpperCase();
    });

    $(".Endereco_Cep").mask("99999-999");
    $(".Endereco_Estado").mask("ss");
    $(".Endereco_Estado").css("text-transform", "uppercase");

    $(".cpf").mask("999.999.999-99");

    $('.phone').mask("(00) 0000-00009").on('focusout', function (event) {
        var target, phone, element;
        target = (event.currentTarget) ? event.currentTarget : event.srcElement;
        phone = target.value.replace(/\D/g, '');
        element = $(target);
        element.unmask();
        if (phone.length > 10) {
            element.mask("(00) 00000-0009");
        } else {
            element.mask("(00) 0000-00009");
        }
    });
});

function Right(str, n) {
    if (n <= 0)
        return "";
    else if (n > String(str).length)
        return str;
    else {
        var iLen = String(str).length;
        return String(str).substring(iLen, iLen - n);
    }
}

function IsDate(input, keyCode) {
    if (keyCode == 16) {
        isShift = true;
    }
    //Allow only Numeric Keys.
    if (((keyCode >= 48 && keyCode <= 57) || keyCode == 8 || keyCode <= 37) && isShift == false) {

        if ((input.value.length == 2 || input.value.length == 5) && keyCode != 8) {
            input.value += separator;
        }

        switch (input.value.length) {
            case 0:
                if (keyCode > 51)
                    return false;
                break;
            case 1:
                if (input.value == 3 && keyCode > 49)
                    return false;
                break;
            case 3:
                if (keyCode > 49)
                    return false;
                break;
            case 4:
                if (input.value.substring(4, 3) == 1 && keyCode > 50)
                    return false;
                else {
                    var dia = input.value.substring(0, 2);
                    if (dia == 31) {
                        var mes = input.value.substring(4, 3) + String.fromCharCode(keyCode);
                        var achou = mes31.find(function (e) {
                            return e == mes;
                        });
                        if (achou == null || achou == undefined)
                            return false;
                    }
                }
                break;
            case 6:
                if (keyCode < 49 || keyCode > 50)
                    return false;
                break;
            case 9:
                var data = input.value + String.fromCharCode(keyCode);
                var arrayOfData = data.split(separator);
                IsValidDate(arrayOfData[0], arrayOfData[1], arrayOfData[2], input);
                return false;
            case 10:
                return false;

            default:
        }

        return true;
    }
    else {
        return false;
    }
}

function IsValidDate(day, month, year, input) {
    var d = new Date();
    d.setFullYear(year, month - 1, day);
    // month - 1 since the month index is 0-based (0 = January)
    if ((d.getFullYear() == year) && (d.getMonth() == month - 1) && (d.getDate() == day)) {
        input.value = day + "/" + month + "/" + year;
        return false;
    }

    var day1 = d.getDate();
    var month1 = d.getMonth() + 1;
    if (day1 < 10) {
        day1 = "0" + day1;
    }
    if (month1 < 10) {
        month1 = "0" + month1;
    }
    input.value = day1 + "/" + month1 + "/" + d.getFullYear();
    return true;
}


function contains(str, search) {
    if (str.indexOf(search) >= 0) {
        return true;
    } else {
        return false;
    }
}
$.fn.capitalize = function (str) {
    $.each(this, function () {
        var split = this.value.split(' ');
        for (var i = 0, len = split.length; i < len; i++) {
            var verify = (split[len - 1] == "D" || split[len - 1] == "d") && (str == "a" || str == "A") || (str == "e" || str == "E") || (str == "o" || str == "O");
            if (verify === false) {
                if (contains(split[i], 'da') === false && contains(split[i], 'de') === false && contains(split[i], 'do') === false) {
                    //alert(split[i]);
                    split[i] = split[i].charAt(0).toUpperCase() + split[i].slice(1);
                }
            }
        }
        this.value = split.join(' ');
    });
    return this;
};

