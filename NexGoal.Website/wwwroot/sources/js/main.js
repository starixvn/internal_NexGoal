import $ from 'jquery';
import 'bootstrap/dist/js/bootstrap.js';
import WebFont from 'webfontloader';

WebFont.load({
    google: {
        families: ['Open Sans:400,600', 'sans-serif']
    }
}); 

$(document).ready(function () {
    var $topNavigation = $('#topNavigation'),
        $sidebar = $('#sidebar'),
        $main = $('#main');

    function modifyMenuVisibility() {
        var menuVisibility = localStorage.MenuVisibility === 'true' || localStorage.MenuVisibility === undefined;

        if (menuVisibility) {
            $topNavigation.removeClass('left-bar-xs').addClass('left-bar');
            $sidebar.removeClass('sidebar-xs').addClass('sidebar');
            $main.removeClass('main-xs').addClass('main');
        } else {
            $topNavigation.removeClass('left-bar').addClass('left-bar-xs');
            $sidebar.removeClass('sidebar').addClass('sidebar-xs');
            $main.removeClass('main').addClass('main-xs');
        }
    }

    modifyMenuVisibility();

    $('#modifyMenuVisibility').on('click', function () {
        var menuVisibility = localStorage.MenuVisibility === 'true' || localStorage.MenuVisibility === undefined;

        localStorage.setItem("MenuVisibility", !menuVisibility);
        modifyMenuVisibility();
    });
});