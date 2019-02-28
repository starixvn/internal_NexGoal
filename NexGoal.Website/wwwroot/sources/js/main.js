import $ from 'jquery';
import 'bootstrap/dist/js/bootstrap.js';
import WebFont from 'webfontloader';

WebFont.load({
    google: {
        families: ['Open Sans:400,600', 'sans-serif']
    }
});

$(document).ready(function () {
    var $navHamburger = $('#js-navHamburger');

    $('#js-btnToggleMenu').on('click', function () {
        if ($navHamburger.hasClass('expended')) {
            $navHamburger.removeClass('expended');
        } else {
            $navHamburger.addClass('expended');
        }
    });

    $navHamburger.find('.dropdown').hover(function () {
        if (!$(this).hasClass('open')) {
            $('.dropdown-toggle', this).trigger('click');
        }
    }, function () {
        if ($(this).hasClass('open')) {
            $('.dropdown-toggle', this).trigger('click');
        }

        $('.dropdown-toggle', this).blur();
    });
});