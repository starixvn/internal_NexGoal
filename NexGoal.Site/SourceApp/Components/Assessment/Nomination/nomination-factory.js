(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("nominationFactory", nominationFactory);

    nominationFactory.$inject = ["$http", "baseUrl"];

    function nominationFactory($http, baseUrl) {
        return {
            createEmployeeOfQuarterPeriod: function (period) {
                var criteria = {
                    Quarter: period.Quarter,
                    Year: period.Year,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.post(baseUrl + "api/EOQ/CreateEOQ", criteria);
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