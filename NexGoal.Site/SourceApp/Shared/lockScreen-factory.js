'use strict';

(function (appModule) {
    appModule.factory('lockScreenService', lockScreenService);

    function lockScreenService() {
        var service = {
            start: function () {
                $('.loading-process').fadeIn();
            },
            stop: function () {
                $('.loading-process').fadeOut(300);
            }
        };

        return service;
    }
})(angular.module("appModule"));