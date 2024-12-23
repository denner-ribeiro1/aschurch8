
/// Métodos de extensão do jQuery
/// Desenvolvido por: Wellington Nascimento
/// Data: 30/09/2014

(function ($) {
    RegistrarBinds();

    var configDefault = {
        width: 500,
        height: 500
    };

    /// Para ser usado a partir de um elemento HTML. 
    /// Ex.: $("#lnkModal").bsModal();
    $.fn.extend({
        popUp: function (title, model, target, params, height, width, fnClose) {
            return $(this).on("click", function () {
                var url = "/Search?searchModel=" + model + "&target=" + target;
                if (params !== "" && params !== undefined && params !== null)
                    url = url + "&parametros=" + JSON.stringify(params);

                if (title !== null && title !== undefined && title !== "")
                    url = url + "&title=" + title;

                openFancybox(url, height, width, fnClose);
            });
        },
        popUpUrl: function (title, url, height, width, fnClose) {
            return $(this).on("click", function () {
                if (title !== null && title !== undefined && title !== "")
                    url = url + "&title=" + title;

                openFancybox(url, height, width, fnClose);
            });
        }
    });

    /// Para ser usado sem depender de elementos HTML. 
    /// Ex.: $.bsMessageInfo('Informativo', 'Mensagem teste');
    $.extend({
        dataFormatada: function (data) {
            var dia = data.getDate().toString(),
                diaF = (dia.length === 1) ? '0' + dia : dia,
                mes = (data.getMonth() + 1).toString(), //+1 pois no getMonth Janeiro começa com zero.
                mesF = (mes.length === 1) ? '0' + mes : mes,
                anoF = data.getFullYear();
            return diaF + "/" + mesF + "/" + anoF;
        },

        bsMessage: function (title, message, type) {
            //var dialogType =
            switch (type) {
                case "Information":
                    $.alertInfo(title, message);
                    break;
                case "Success":
                    $.alertSuccess(title, message);
                    break;
                case "Warning":
                    $.alertWarning(title, message);
                    break;
                case "Error":
                    $.alertError(title, message);
                    break;
                default:
            }
        },

        bsMessageInfo: function (title, message) {
            $.alertInfo(title, message)
        },

        bsMessageSuccess: function (title, message) {
            $.alertSuccess(title, message);
        },

        bsMessageWarning: function (title, message) {
            $.alertWarning(title, message);
        },

        bsMessageError: function (title, message) {
            $.alertError(title, message);
        },

        ajaxPost: function (url, requestData, fnCallbackSuccess, returnType) {
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
                url: url,
                contentType: 'application/x-www-form-urlencoded',
                dataType: returnType == null ? "json" : returnType,
                data: requestData,
                error: function (xhr, status, error) {
                    console.log(error);
                    console.log(xhr.responseText);
                },
                success: fnCallbackSuccess == null
                    ? function (event, ui) {
                        $(this).empty();
                    } : fnCallbackSuccess
                ,
                complete: function (param) {
                    removerOverlay(null);
                }
            });
        },

        ajaxGet: function (url, fnCallbackSuccess) {
            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);

            $.ajax({
                cache: false,
                type: "GET",
                url: url,
                error: function (xhr, status, error) {
                    console.log(error);
                    console.log(xhr.responseText);
                },
                success: fnCallbackSuccess == null
                    ? function (event, ui) {
                        $(this).empty();
                    } : fnCallbackSuccess
                ,
                complete: function (param) {
                    removerOverlay(null);
                }
            });
        },

        ajaxPostForm: function (url, form, fnCallbackSuccess) {
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
                url: url,
                data: $(form).serialize(),
                error: function (xhr, status, error) {
                    console.log(error);
                    console.log(xhr.responseText);
                },
                success: fnCallbackSuccess === null
                    ? function (event, ui) {
                        $(this).empty();
                    } : fnCallbackSuccess
                ,
                complete: function (param) {
                    removerOverlay(null);
                }
            });
        },

        downloadArquivo: function (url, btn, urlRedirect) {
            if (btn != null)
                btn.prop("disabled", true);
            var overlay = gerarOverlay();
            var loadingFrame = gerarLoadingFrame();

            overlay.append(loadingFrame);
            $('body').append(overlay);

            setTimeout(function () {
                overlay.focus();
            }, 0);

            var request = new XMLHttpRequest();
            request.open('GET', url);
            request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
            request.responseType = 'blob';

            request.onload = function () {
                if (request.status === 200) {
                    var disposition = request.getResponseHeader('content-disposition').replace(/"/g, '');
                    var strAux = disposition.substring(disposition.indexOf("filename=") + 9);
                    var strAuxArray = strAux.split(";");
                    var filename = strAuxArray[0];
                    if (request.response != null && navigator.msSaveBlob)
                        return navigator.msSaveBlob(new Blob([request.response], { type: request.response.type }), filename);

                    var blob = new Blob([request.response], { type: request.response.type });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = filename;
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    if (urlRedirect != null)
                        window.location = urlRedirect;
                }
                else {
                    removerOverlay(null);

                    var blob2 = new Blob([request.response], { type: request.response.type });
                    var reader = new FileReader();
                    reader.onload = function () {
                        $.alertError("Download", JSON.parse(reader.result).data);
                    };
                    reader.readAsText(blob2);
                }
                removerOverlay(null);
                if (btn != null)
                    btn.prop("disabled", false);
            };
            request.send();
        },

        getQuerystringValue: function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },

        getIdFromCurrentUrlPathLocation: function () {
            return location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
        },

        showYesNoAlert: function (msg, caption, optFunctionYes, optFunctionParamYes, optFunctionNo, optFunctionParamsNo) {
            if (!$.isArray(optFunctionParamYes)) {
                optFunctionParamYes = Array.prototype.slice.call(arguments, 3);
            }

            if (!$.isArray(optFunctionParamsNo)) {
                optFunctionParamsNo = Array.prototype.slice.call(arguments, 3);
            }

            Swal.fire({
                title: '<strong>' + caption + '</strong>',
                html: msg,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: '<i class="fa fa-check-circle"></i> Sim',
                cancelButtonText: '<i class="fa fa-ban"></i> Não',
                width: 500,
                allowOutsideClick: false
            }).then((result) => {
                if (result.value) {
                    if (optFunctionYes && typeof optFunctionYes == "function") {
                        optFunctionYes.apply(window, optFunctionParamYes || []);
                    }
                }
                else {
                    if (optFunctionNo && typeof optFunctionNo == "function") {
                        optFunctionNo.apply(window, optFunctionParamsNo || []);
                    }
                }
            });
        },

        showOkCancelAlert: function (msg, caption, optFunction, optFunctionParams) {
            if (!$.isArray(optFunctionParams)) {
                optFunctionParams = Array.prototype.slice.call(arguments, 3);
            }

            Swal.fire({
                title: '<strong>' + caption + '</strong>',
                html: msg,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'SIM',
                cancelButtonText: 'NÃO',
                width: 500,
                allowOutsideClick: false
            }).then((result) => {
                if (result.value) {
                    if (optFunction && typeof optFunction == "function") {
                        optFunction.apply(window, optFunctionParams || []);
                    }
                }
            });
        },

        alert: function (title, message, type) {
            Swal.fire(
                title,
                message,
                'info'
            );
        },

        alertInfo: function (title, message) {
            Swal.fire(
                title,
                message,
                'info'
            );
        },

        alertSuccess: function (title, message) {
            Swal.fire(
                title,
                message,
                'success'
            );
        },

        alertWarning: function (title, message) {
            Swal.fire(
                title,
                message,
                'warning'
            );
        },

        alertError: function (title, message) {
            Swal.fire(
                title,
                message,
                'error'
            );
        },

        incluirCelulasTabela: function (row, col, width, text, colSpan, align, className) {
            var cell1 = row.insertCell(col);
            if (width != null && width != undefined && width != "")
                cell1.width = width;
            if (colSpan != null && colSpan != undefined && colSpan != "")
                cell1.colSpan = colSpan;
            if (align != null && align != undefined && align != "")
                cell1.style.textAlign = align
            if (className != null && className != undefined && className != "")
                cell1.className = className;

            cell1.innerHTML = text;
        },

        findElement: function (arr, propName, propValue) {
            for (var i = 0; i < arr.length; i++)
                if (arr[i][propName] == propValue)
                    return arr[i];
        },

        popUp: function (title, model, target, params, height, width, fnClose) {
            var url = "/Search?searchModel=" + model + "&target=" + target;
            if (params !== "" && params !== undefined && params !== null)
                url = url + "&parametros=" + JSON.stringify(params);

            if (title !== null && title !== undefined && title !== "")
                url = url + "&title=" + title;

            openFancybox(url, height, width, fnClose);
        },

        popUpUrl: function (url, height, width, fnClose) {
            openFancybox(url, height, width, fnClose);
        }

    });

    var htmlOriginal = $.fn.html;

    // redefine the `.html()` function to accept a callback
    $.fn.html = function (html, callback) {
        // run the old `.html()` function with the first parameter
        var ret = htmlOriginal.apply(this, arguments);
        // run the callback (if it is defined)
        if (typeof callback == "function") {
            callback();
        }
        // make sure chaining is not broken
        return ret;
    };

    if (!String.prototype.padStart) {
        String.prototype.padStart = function padStart(targetLength, padString) {
            targetLength = targetLength >> 0; //truncate if number, or convert non-number to 0;
            padString = String(typeof padString !== 'undefined' ? padString : ' ');
            if (this.length >= targetLength) {
                return String(this);
            } else {
                targetLength = targetLength - this.length;
                if (targetLength > padString.length) {
                    padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
                }
                return padString.slice(0, targetLength) + String(this);
            }
        };
    }

    if (!Array.prototype.filter) {
        Array.prototype.filter = function (fun/*, thisArg*/) {
            'use strict';

            if (this === void 0 || this === null) {
                throw new TypeError();
            }

            var t = Object(this);
            var len = t.length >>> 0;
            if (typeof fun !== 'function') {
                throw new TypeError();
            }

            var res = [];
            var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
            for (var i = 0; i < len; i++) {
                if (i in t) {
                    var val = t[i];

                    // NOTE: Technically this should Object.defineProperty at
                    //       the next index, as push can be affected by
                    //       properties on Object.prototype and Array.prototype.
                    //       But that method's new, and collisions should be
                    //       rare, so use the more-compatible alternative.
                    if (fun.call(thisArg, val, i, t)) {
                        res.push(val);
                    }
                }
            }

            return res;
        };
    }

    if (!Array.prototype.findIndex) {
        Array.prototype.findIndex = function (predicate) {
            if (this === null) {
                throw new TypeError('Array.prototype.findIndex called on null or undefined');
            }
            if (typeof predicate !== 'function') {
                throw new TypeError('predicate must be a function');
            }
            var list = Object(this);
            var length = list.length >>> 0;
            var thisArg = arguments[1];
            var value;

            for (var i = 0; i < length; i++) {
                value = list[i];
                if (predicate.call(thisArg, value, i, list)) {
                    return i;
                }
            }
            return -1;
        };
    }

    if (!Array.prototype.find) {
        Array.prototype.find = function (predicate) {
            if (this === null) {
                throw new TypeError('Array.prototype.find called on null or undefined');
            }
            if (typeof predicate !== 'function') {
                throw new TypeError('predicate must be a function');
            }
            var list = Object(this);
            var length = list.length >>> 0;
            var thisArg = arguments[1];
            var value;

            for (var i = 0; i < length; i++) {
                value = list[i];
                if (predicate.call(thisArg, value, i, list)) {
                    return value;
                }
            }
            return undefined;
        };
    }
})(jQuery);

String.prototype.format = function () {
    a = this;
    for (k in arguments) {
        a = a.replace("{" + k + "}", arguments[k]);
    }
    return a;
};

var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
        timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
        timezoneClip = /[^-+\dA-Z]/g,
        pad = function (val, len) {
            val = String(val);
            len = len || 2;
            while (val.length < len) val = "0" + val;
            return val;
        };

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
            d = date[_ + "Date"](),
            D = date[_ + "Day"](),
            m = date[_ + "Month"](),
            y = date[_ + "FullYear"](),
            H = date[_ + "Hours"](),
            M = date[_ + "Minutes"](),
            s = date[_ + "Seconds"](),
            L = date[_ + "Milliseconds"](),
            o = utc ? 0 : date.getTimezoneOffset(),
            flags = {
                d: d,
                dd: pad(d),
                ddd: dF.i18n.dayNames[D],
                dddd: dF.i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: dF.i18n.monthNames[m],
                mmmm: dF.i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
            };

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
        "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab",
        "Domingo", "Segunda-Feira", "Terça-Feira", "Quarta-Feira", "Quinta-Feira", "Sexta-Feira", "Sábado"
    ],
    monthNames: [
        "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez",
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outobro", "Novembro", "Dezembro"
    ]
};

// For convenience...
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};

function EncontrarItem(lista, predicate) {
    var resultado = [];

    for (var i = 0; i < lista.length; i++) {
        if (predicate(lista[i])) {
            resultado.push(lista[i]);
        }
    }

    if (resultado.length > 0)
        return resultado;
    else
        return undefined;

};

function EncontrarIndice(lista, predicate) {
    for (var i = 0; i < lista.length; i++) {
        if (predicate(lista[i]))
            return i;
    }
    return -1;
};

function RegistrarBinds() {
    /*
    Script para bloquear a ação do backspace quando nenhum controle estiver com o foco
    */
    $(document).on('keydown', function (e) {
        var doPrevent = false;
        if (e.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d) && (d.tagName != undefined) && (d.tagName.toUpperCase() === 'INPUT' || d.tagName.toUpperCase() === 'TEXTAREA')) {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            e.preventDefault();
        }
    });
    /*
    Efetua o bind do evento keydown para as tags do tipo 'input' e 'select' com os atributos seletores
    */
    $('input, select').keydown(function (e) {
        if (e.keyCode === 13) {
            var newField;

            cancelEvent(e);

            //Sem Shift pressionado
            if (!e.shiftKey) {
                newField = getNextElement(this);
            } else { //Com Shift pressionado
                newField = getPreviousElement(this);
            }

            //Define o foco no novo controle
            newField.focus();

            //Seleciona o conteúdo do novo controle
            if (newField[0].type != 'submit')
                newField.select();
        }
    });
}

/*
Resgata o próximo elemento com base no elemento informado
field = elemento usado como referência
*/
function getNextElement(field) {
    var focusable = $('input,a,select,button,textarea').filter(':visible');
    var index = focusable.index(field) + 1;

    if (index == focusable.length)
        index = 0;

    if ((focusable.eq(index) != undefined) && (focusable.eq(index)[0] != undefined)) {
        while ((focusable.eq(index)[0].disabled) || (focusable.eq(index)[0].readOnly)) {
            index++;
        }

        return focusable.eq(index);
    } else
        return field;
}

/*
Resgata o elemento anterior com base no elemento informado
field = elemento usado como referência
*/
function getPreviousElement(field) {
    var focusable = $('input,a,select,button,textarea').filter(':visible');
    var index = focusable.index(field) - 1;

    if ((focusable.eq(index) != undefined) && (focusable.eq(index)[0] != undefined)) {
        while ((focusable.eq(index)[0].disabled) || (focusable.eq(index)[0].readOnly)) {
            index--;
        }

        return focusable.eq(index);
    } else
        return field;
}

/*
Cancela o evento informado
e = evento a ser cancelado
*/
function cancelEvent(e) {
    e.returnValue = false;
    e.cancelBubble = true;

    if (typeof (e.preventDefault) !== "undefined") {
        e.preventDefault();
    }
    if (typeof (e.stopPropagation) !== "undefined") {
        e.stopPropagation();
    }
}

function openFancybox(url, height, width, fnClose) {
    $.fancybox.open({
        src: url,
        type: 'iframe',
        modal: true,
        toolbar: false,
        smallBtn: true,
        iframe: {
            preload: true,
            css: {
                width: width === null || width === undefined || width === '' ? '900px' : width,
                height: height === null || height === undefined || height === '' ? '600px' : height
            }
        },
        afterClose: fnClose === null ? function () { } : fnClose
    });
}

function PesquisarCEP(cep, logradouro, bairro, cidade, estado) {
    var cepsemacento = $(cep).val().replace(/\D/g, '');
    //Verifica se campo cep possui valor informado.
    if (cepsemacento !== "") {
        //Expressão regular para validar o CEP.
        var validacep = /^[0-9]{8}$/;
        //Valida o formato do CEP.
        if (validacep.test(cepsemacento)) {
            try {
                var overlay = gerarOverlay();
                var loadingFrame = gerarLoadingFrame();

                overlay.append(loadingFrame);
                $('body').append(overlay);

                setTimeout(function () {
                    overlay.focus();
                }, 0);

                $.getJSON("https://viacep.com.br/ws/" + cepsemacento + "/json/?callback=?", function (dados) {
                    if (!("erro" in dados)) {
                        $(logradouro).val(dados.logradouro);
                        $(bairro).val(dados.bairro);
                        $(cidade).val(dados.localidade);
                        $(estado).val(DeParaUF(dados.uf));
                    } else {
                        limpa_formulario_cep(logradouro, bairro, cidade, estado);
                        $.alertError("Buscar CEP", "CEP não encontrado.");
                    }
                }).always(function () {
                    removerOverlay(null);
                });
            } catch (e) {
                limpa_formulario_cep(logradouro, bairro, cidade, estado);
                removerOverlay(null);
            }
        } else {
            limpa_formulario_cep(logradouro, bairro, cidade, estado);
            $.alertError("Buscar CEP", "Formato de CEP inválido.");
        }
    } else {
        limpa_formulario_cep(logradouro, bairro, cidade, estado);
    }
}

function DeParaUF(uf) {
    switch (uf) {
        case "SP": return 1;
        case "AC": return 2;
        case "AL": return 3;
        case "AP": return 4;
        case "AM": return 5;
        case "BA": return 6;
        case "CE": return 7;
        case "DF": return 8;
        case "ES": return 9;
        case "GO": return 10;
        case "MA": return 11;
        case "MT": return 12;
        case "MS": return 13;
        case "MG": return 14;
        case "PR": return 15;
        case "PB": return 16;
        case "PA": return 17;
        case "PE": return 18;
        case "PI": return 19;
        case "RJ": return 20;
        case "RN": return 21;
        case "RS": return 22;
        case "RO": return 23;
        case "RR": return 24;
        case "SC": return 25;
        case "SE": return 26;
        case "TO": return 27;
    }
}

function limpa_formulario_cep(logradouro, bairro, cidade, estado) {
    $(logradouro).val("");
    $(bairro).val("");
    $(cidade).val("");
    $(estado).val("");
}

function gerarLoadingFrame() {
    var frame = $('<div class="as-loading-frame">');
    var img = $('<div class="as-loading-img">');
    var text = $('<span class="texto">').text('Carregando...');

    frame.append(img);
    frame.append(text);

    return frame;
}

function gerarOverlay(semAnimacao) {
    semAnimacao = semAnimacao || false;
    var overlay = $('<div class="as-overlay" tab-index="0">');

    if (semAnimacao)
        overlay.addClass('instantaneo');

    return overlay;
}

function removerOverlay(overlay) {
    overlay = overlay ? $(overlay) : $('.as-overlay');
    if (overlay && overlay.length > 0)
        overlay.remove();
}


function threatCpf(cpf) {
    return cpf.trim().replace(/\./gi, '').replace(/-/gi, '');
}

function getFirstDigit(cpf) {
    var multipliers = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    var cpfToWork = cpf.substring(0, 9);
    var sum = 0;
    for (var i = 0; i < multipliers.length; i++) {
        sum += parseInt(cpfToWork[i].toString()) * multipliers[i];
    }

    var rest = sum % 11;
    var firstDigit = rest < 2 ? 0 : 11 - rest;
    return firstDigit;
}

function getSecondDigit(cpf, firstDigit) {
    var multipliers = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
    var cpfToWork = cpf.substring(0, 9) + firstDigit;
    var sum = 0;

    for (var i = 0; i < multipliers.length; i++) {
        sum += parseInt(cpfToWork[i].toString()) * multipliers[i];
    }

    var rest = sum % 11;
    var secondDigit = rest < 2 ? 0 : 11 - rest;
    return secondDigit;
}

function isInvalidLength(cnpj) {
    return cnpj.length !== 11;
}

function isInvalidSequence(cpf) {
    if (cpf === "00000000000" ||
        cpf === "11111111111" ||
        cpf === "22222222222" ||
        cpf === "33333333333" ||
        cpf === "44444444444" ||
        cpf === "55555555555" ||
        cpf === "66666666666" ||
        cpf === "77777777777" ||
        cpf === "88888888888" ||
        cpf === "99999999999") {
        return true;
    }

    return false;
}

function isNotNumbersOnly(cnpj) {
    return !/^\d+$/.test(cnpj);
}

function isInvalidCpf(cpf) {
    if (cpf !== "" && cpf !== null) {
        var firstDigit = getFirstDigit(cpf);
        var secondDigit = getSecondDigit(cpf, firstDigit);
        var expectedSufix = firstDigit.toString() + secondDigit.toString();
        var endsWithPattern = new RegExp(expectedSufix + '$')
        var isInvalid = !endsWithPattern.test(cpf);
    }

    return isInvalid;
}

function ValidarCPF(value) {
    if (value.trim() === "" || value.trim() === "___.___.___-__" || value.trim() === '') {
        return true;
    }
    var cpf = threatCpf(value);
    var isInvalid = isInvalidLength(cpf) || isNotNumbersOnly(cpf) || isInvalidCpf(cpf) || isInvalidSequence(cpf);
    var isValid = !isInvalid;
    return isValid;
}

/*
Função prototype para substituir todas as ocorrências de um caracter em uma string
s1 = caracter a ser substituído
s2 = caracter para substituição
*/
String.prototype.replaceAll = function (s1, s2) {
    var str = this;
    var pos = str.indexOf(s1);
    while (pos > -1) {
        str = str.replace(s1, s2);
        pos = str.indexOf(s1);
    }
    return (str);
}

/*
Adiciona uma determinada quantidade de um determinado caracter a esquerda de uma string
val = string original
ch = caracter para repetição
num = quantidade de vezes para repetição do caracter
*/
function padLeft(val, ch, num) {
    var re = new RegExp(".{" + num + "}$");
    var pad = "";
    if (!ch) ch = " ";
    do {
        pad += ch;
    } while (pad.length < num);
    return re.exec(pad + val)[0];
}

/*
Adiciona uma determinada quantidade de um determinado caracter a direita de uma string
val = string original
ch = caracter para repetição
num = quantidade de vezes para repetição do caracter
*/
function padRight(val, ch, num) {
    var re = new RegExp("^.{" + num + "}");
    var pad = "";
    if (!ch) ch = " ";
    do {
        pad += ch;
    } while (pad.length < num);
    return re.exec(val + pad)[0];
}
