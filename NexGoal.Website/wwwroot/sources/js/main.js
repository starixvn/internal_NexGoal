import $ from 'jquery';
import 'bootstrap/dist/js/bootstrap.js';
import WebFont from 'webfontloader';
import axios from 'axios';

WebFont.load({
    google: {
        families: ['Open Sans:400,600', 'sans-serif']
    }
});

window.overlay = (function () {
    let element = document.getElementById("loading-overlay");

    return {
        show: function () {
            element.classList.remove('hidden');
        },
        hide: function () {
            element.classList.add('hidden');
        }
    };
})();

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
        $('.dropdown-toggle', this).trigger('click'); 
    });
});