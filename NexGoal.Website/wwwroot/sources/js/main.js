import $ from 'jquery';
import 'bootstrap/dist/js/bootstrap.js';
import WebFont from 'webfontloader';

WebFont.load({
    google: {
        families: ['Open Sans:400,600', 'sans-serif']
    }
});

$(document).ready(function () {
    const $navHamburger = $('#js-navHamburger');

    $('#js-btnToggleMenu').on('click', function () {
        if ($navHamburger.hasClass('expended')) {
            $navHamburger.removeClass('expended');
        } else {
            $navHamburger.addClass('expended');
        }
    });

    $navHamburger.find('.dropdown').hover(function () {
        if (!$(this).hasClass('open')) {
            $(this).addClass('open');
        }
    }, function () {
        if ($(this).hasClass('open')) {
            $(this).removeClass('open');
        }

        $('.dropdown-toggle', this).blur();
    });

    $('.user-avatar').on('click', function () {
        let dropdown = $(this).find('.dropdown-menu');
        if ($(dropdown).hasClass('open')) {
            $(dropdown).removeClass('open');
        } else {
            $(dropdown).addClass('open');
        }
    });

    $(document).click(function (event) {
        let $target = $(event.target);
        if (!$target.closest('.user-avatar').length && $('.user-avatar .dropdown-menu').hasClass('open')) {
            $('.user-avatar .dropdown-menu').removeClass('open');
        }
    });
});