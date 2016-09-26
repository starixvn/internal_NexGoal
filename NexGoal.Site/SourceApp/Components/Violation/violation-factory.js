(function (violationModule) {
    "use strict";

    violationModule.factory("violationFactory", violationFactory);

    violationFactory.$inject = ["$http", "baseUrl"];

    function violationFactory($http, baseUrl) {
        return {
            getViolations: function (fromDate, toDate, isActive, groupIds, employeeId) {
                var params = {
                    FromDate: fromDate,
                    ToDate: toDate,
                    IsActive: isActive,
                    GroupIds: groupIds,
                    EmployeeId: employeeId
                };

                return $http.get(baseUrl + "api/EOQ/GetEOQ", { params: params });
            },
            createViolation: function (employeeId, effectiveDate, description) {
                var criteria = {
                    EmployeeId: employeeId,
                    EffectedDate: effectiveDate,
                    Description: description
                };

                return $http.post(baseUrl + "api/Violation/Create", criteria);
            }
        };
    }
})(angular.module("violationModule"));