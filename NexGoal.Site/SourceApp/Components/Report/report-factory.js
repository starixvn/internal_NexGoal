'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        requestExportEPA: function (epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportEPA/', { params: { epaId: epaId } });
        },
        requestExportSemiEPA: function (epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportSemiEPA/', { params: { epaId: epaId } });
        },
        getPAReport: function (periodIds, groupIds, employeeId) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport',
                {
                    params: {
                        PeriodIds: periodIds,
                        GroupIds: groupIds,
                        EmployeeId: employeeId
                    }
                });
        },
        getMasterReport: function (periodId, groupIds, employeeId) {
            return $http.get(baseUrl + 'api/Report/GetMasterReport',
                {
                    params: {
                        PeriodId: periodId,
                        GroupIds: groupIds,
                        EmployeeId: employeeId
                    }
                });
        },
        getNominationReport: function (eoqyId, query, groupIds, statusId) {
            return $http.get(baseUrl + 'api/Report/GetNominationReport',
                {
                    params: {
                        EOQYId: eoqyId,
                        Query: query,
                        GroupIds: groupIds,
                        StatusId: statusId
                    }
                });
        },
        getPermissionOnRecalculateScore: function () {
            return $http.get(baseUrl + 'api/Report/GetPermissionOnRecalculateScore');
        },
        recalculateScore: function (criteria) {
            return $http.put(baseUrl + 'api/Report/RecalculateScore', criteria);
        }
    };
}]);