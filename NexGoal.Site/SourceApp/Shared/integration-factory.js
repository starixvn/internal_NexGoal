(function (appModule) {
    "use strict";

    appModule.factory("integrateFactory", integrateFactory);

    integrateFactory.$inject = ["$http", "baseUrl"];

    function integrateFactory($http, baseUrl) {
        return {
            getHRMSummary: function () {
                return $http.get(baseUrl + "api/Integration/GetHRMSummary");
            },
            getEmployeeProject: function () {
                return $http.get(baseUrl + "api/Integration/GetEmployeeWorking");
            }
        };
    }

})(angular.module("appModule"));