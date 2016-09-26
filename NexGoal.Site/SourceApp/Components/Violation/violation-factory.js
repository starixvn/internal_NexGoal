(function (violationModule) {
    "use strict";

    violationModule.factory("violationFactory", violationFactory);

    violationFactory.$inject = ["$http", "baseUrl"];

    function violationFactory($http, baseUrl) {
        return {
        };
    }
})(angular.module("violationModule"));