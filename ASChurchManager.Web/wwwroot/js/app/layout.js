var mySkins = [
    'skin-blue',
    'skin-black',
    'skin-red',
    'skin-yellow',
    'skin-purple',
    'skin-green',
    'skin-blue-light',
    'skin-black-light',
    'skin-red-light',
    'skin-yellow-light',
    'skin-purple-light',
    'skin-green-light'
];

$(function () {
    'use strict';
    var skin = $('#skinUser').val();
    if (skin !== undefined && skin !== '')
        store('skin', skin);
    else
        store('skin', 'skin-blue');
    setup();
});



function get(name) {
    if (typeof (Storage) !== 'undefined') {
        return localStorage.getItem(name);
    } else {
        window.alert('Favor usar um browser moderno para visualização do sistema!');
    }
}

function store(name, val) {
    if (typeof (Storage) !== 'undefined') {
        localStorage.setItem(name, val);
    } else {
        window.alert('Favor usar um browser moderno para visualização do sistema!');
    }
}

function changeSkin(cls) {
    $.each(mySkins, function (i) {
        $('body').removeClass(mySkins[i]);
    });

    $('body').addClass(cls);
    store('skin', cls);
    return false;
}

function setup() {
    var tmp = get('skin');
    if (tmp && $.inArray(tmp, mySkins))
        changeSkin(tmp);

    // Add the change skin listener
    $('[data-skin]').on('click', function (e) {
        if ($(this).hasClass('knob'))
            return;
        e.preventDefault();

        var request = {
            novoSkin: $(this).data('skin')
        };
        $.ajaxPost(urlAltSkin, request);
        changeSkin($(this).data('skin'));
    });
}