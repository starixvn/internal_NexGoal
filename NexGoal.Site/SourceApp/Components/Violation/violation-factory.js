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

                return $http.get(baseUrl + "api/Violation/Get", { params: params });
            },
            getViolation: function (violationId) {
                var params = {
                   ViolationId: violationId
                };

                return $http.get(baseUrl + "api/Violation/GetById", { params: params });
            },
            createViolation: function (employeeId, effectiveDate, description) {
                var criteria = {
                    EmployeeId: employeeId,
                    EffectedDate: effectiveDate,
                    Description: description
                };

                return $http.post(baseUrl + "api/Violation/Create", criteria);
            },
            updateViolation: function (violationId, effectiveDate, description) {
                var criteria = {
                    ViolationId: violationId,
                    EffectedDate: effectiveDate,
                    Description: description
                };

                return $http.post(baseUrl + "api/Violation/Update", criteria);
            },
            modifyViolationStatus: function (violationId) {
                var criteria = {
                    ViolationId: violationId
                };

                return $http.post(baseUrl + "api/Violation/UpdateActive", criteria);
            }
        };
    }
})(angular.module("violationModule"));