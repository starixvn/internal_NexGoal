(function (appModule) {
    "use strict";

    appModule.factory("integrateFactory", integrateFactory);

    integrateFactory.$inject = ["$http", "baseUrl"];

    function integrateFactory($http, baseUrl) {
        return {
            getHRMSummary: function (employeeId, periodId) {
                return $http.get(baseUrl + "api/Integration/GetHRMSummary",{ params: { employeeId: employeeId, PeriodId: periodId } });
            },
			getHRMSummaryOnProbation: function (employeeId) {
                return $http.get(baseUrl + "api/Integration/GetHRMSummaryOnProbation",{ params: { employeeId: employeeId} });
            },
            getEmployeeProject: function (employeeId, periodId) {
                return $http.get(baseUrl + "api/Integration/GetEmployeeWorking", { params: { employeeId: employeeId, PeriodId: periodId } });
            }
        };
    }

})(angular.module("appModule"));