'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getLastRating: function (employeeId, targetTypeId, epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/getLastRating', { params: { employeeId: employeeId, targetTypeId: targetTypeId, epaId: epaId } });
        },
        getAllPeriodByReviewer: function () {
            return $http.get(baseUrl + 'api/Period/getAllPeriodByReviewer');
        },
        getUserPermissionOnEmployee: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnEmployee');
        },
        getEPAByEmployee: function (periodIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEmployee', { params: { periodIds: periodIds } });
        },
        getEPAInfoByEmployee: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', { params: { epaId: epaId } });
        },
        getUserPermissionOnEvaluator: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnEvaluator');
        },
        getGroupByEvaluator: function (periodIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetGroupByEvaluator', { params: { periodIds: periodIds } });
        },
        getMemberByEvaluator: function (periodIds, groupIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByEvaluator', {
                params: {
                    groupIds: groupIds,
                    periodIds: periodIds
                }
            });
        },
        getEPAByEvaluator: function (periodIds, groupIds, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupIds: groupIds,
                    periodIds: periodIds,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator',
            {
                params: {
                    epaId: epaId,
                    evaluatorId: null
                }
            });
        },
        getUserPermissionOnReviewer: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnReviewer');
        },
        getGroupByReviewer: function (periodIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetGroupByReviewer', {
                params: {
                    periodIds: periodIds
                }
            });
        },
        getMemberByReviewer: function (periodIds, groupIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByReviewer', {
                params: {
                    periodIds: periodIds,
                    groupIds: groupIds
                }
            });
        },
        getEPAByReviewer: function (periodIds, groupIds, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    periodIds: periodIds,
                    groupIds: groupIds,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByReviewer: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', { params: { epaId: epaId } });
        },
        getEPAInfoById: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoById', { params: { epaId: epaId } });
        },
        updateEPAScoreByEmployee: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEmployee', model);
        },
        updateEPAScoreByEvaluator: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEvaluator', model);
        },
        adjustScoreByReviewer: function (models) {
            return $http.put(baseUrl + 'api/EmployeeEPA/AdjustScoreByReviewer', models);
        },
        convertEPA: function () {
            return $http.put(baseUrl + 'api/EmployeeEPA/ConvertEPA');
        },
        resetScore: function (criteria) {
            return $http.put(baseUrl + 'api/EmployeeEPA/ResetScore', criteria);
        },
        getPeriodToUpdateFinalScore: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetPeriodToUpdateFinalScore');
        },
        getEmployeeToUpdateFinalScore: function (periodId, groupId, query) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeToUpdateFinalScore',
            {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    query: query
                }
            });
        },
        getEPAToUpdateFinalScore: function (periodId, groupId, employeeId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAToUpdateFinalScore',
            {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    employeeId: employeeId
                }
            });
        },
        updateFinalScore: function (selectedEPAs) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateFinalScore', selectedEPAs);
        }
    };
}]);