(function ($) {
    $.validator.addMethod('cpf', function (value, element) {
        return ValidarCPF(value);
    }, '');

    $.validator.unobtrusive.adapters.add('cpf', function (options) {
        options.rules["cpf"] = '#' + options.params;
        options.messages['cpf'] = options.message;
    });
})(jQuery);
