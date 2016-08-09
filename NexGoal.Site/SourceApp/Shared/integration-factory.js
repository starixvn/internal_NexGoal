(function (appModule) {
    "use strict";

    appModule.factory("integrateFactory", integrateFactory);

    integrateFactory.$inject = ["$http", "baseUrl"];

    function integrateFactory($http, baseUrl) {
        return {
            getHRMSummary: function (employeeId) {
                return $http.get(baseUrl + "api/Integration/GetHRMSummary",{ params: { employeeId: employeeId } });
            },
            getEmployeeProject: function (employeeId) {
                return $http.get(baseUrl + "api/Integration/GetEmployeeWorking",{ params: { employeeId: employeeId } });
            }
        };
    }

})(angular.module("appModule"));