(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("eoqFactory", eoqFactory);

    eoqFactory.$inject = ["$http", "baseUrl"];

    function eoqFactory($http, baseUrl) {
        return {
            createEOQ: function (period) {
                var criteria = {
                    Quarter: period.Quarter,
                    Year: period.Year,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.post(baseUrl + "api/EOQ/CreateEOQ", criteria);
            },
            updateEOQ: function (period) {
                return $http.post(baseUrl + "api/EOQ/UpdateEOQ", period);
            },
            deleteEOQ: function (eoqId) {
                return $http.delete(baseUrl + "api/EOQ/DeleteEOQ", { params: { EOQId: eoqId } });
            },
            getEOQ: function (year, statusId) {
                return $http.get(baseUrl + "api/EOQ/GetEOQ", { params: { Year: year, StatusId: statusId } });
            },
        };
    }
})(angular.module("assessmentModule"));