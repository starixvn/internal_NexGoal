(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("nominationFactory", nominationFactory);

    nominationFactory.$inject = ["$http", "baseUrl"];

    function nominationFactory($http, baseUrl) {
        return {
            createEmployeeOfQuarterPeriod: function (period) {
                return $http.post(baseUrl + "api/EOQ/CreateEOQ", period);
            },
            updateEmployeeOfQuarterPeriod: function (period) {
                return $http.post(baseUrl + "api/EOQ/UpdateEOQ", period);
            },
            getEmployeeOfQuarterPeriod: function (year, status) {
                return $http.get(baseUrl + "api/EOQ/GetEOQ", { params: { year: year, status: status } });
            }
        };
    }
})(angular.module("assessmentModule"));