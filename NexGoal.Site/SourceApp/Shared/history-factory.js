'use strict';
(function (angular) {
    function historyFactory($window) {
        var model = {};

        var service = {
            saveState: function (filter, name) {
                model[name] = filter;
            },

            getState: function (name) {
                if (model[name] && model[name].isBack) {
                    model[name].isBack = false;

                    return model[name];
                }

                return null;
            },

            back: function (name) {
                model[name].isBack = true;
                $window.history.back();
            },

            setBackStatus: function (status, name) {
                if (!model[name])
                    model[name] = {};

                model[name].isBack = status;
            }
        };

        return service;
    }

    historyFactory.$inject = ['$window'];

    angular
        .module('appModule')
        .factory('historyFactory', historyFactory);
})(angular);
