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
            createEOQNomination: function (eoqyId, employeeId, eoqQuestions) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: employeeId,
                    NominationDetails: eoqQuestions
                };

                return $http.post(baseUrl + "api/EOQ/CreateEOQNomination", criteria);
            },
            updateEOQ: function (period) {
                var criteria = {
                    EOQYId: period.EOQYId,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.put(baseUrl + "api/EOQ/UpdateEOQ", criteria);
            },
            deleteEOQ: function (eoqId) {
                return $http.delete(baseUrl + "api/EOQ/DeleteEOQ", { params: { Id: eoqId } });
            },
            getEOQ: function (year, statusId) {
                return $http.get(baseUrl + "api/EOQ/GetEOQ", { params: { Year: year, StatusId: statusId } });
            },
            getEOQById: function (eoqId) {
                return $http.get(baseUrl + "api/EOQ/GetEOQById", { params: { Id: eoqId } });
            },
            getEOQNominatorSettingById: function (eoqyId, query, groupIds, roleId, showAllEmployee) {
                var params = {
                    EOQYId: eoqyId,
                    Query: query,
                    GroupIds: groupIds,
                    RoleId: roleId,
                    ShowAllEmployee: showAllEmployee
                };

                return $http.get(baseUrl + "api/EOQ/GetEOQNominatorSettingById", { params: params });
            },
            getUserPermissionOnManageNomination: function () {
                return $http.get(baseUrl + "api/EOQ/GetUserPermissionOnManageNomination");
            },
            updateEOQNominatorSettingById: function (eoqyId, nominators) {
                var criteria = {
                    EOQYId: eoqyId,
                    Nominators: nominators
                };

                return $http.put(baseUrl + "api/EOQ/UpdateEOQNominatorSettingById", criteria);
            },
            getEOQQuestion: function () {
                return $http.get(baseUrl + "api/EOQ/GetEOQQuestion");
            },
            getCurrentEOQByNominatorId: function () {
                return $http.get(baseUrl + "api/EOQ/GetCurrentEOQByNominatorId");
            },
            getEOQNominationByNominatorId: function (eoqyId, roleId, query, teamIds, statusId) {
                var params = {
                    EOQYId: eoqyId,
                    RoleId: roleId,
                    Query: query,
                    GroupIds: teamIds,
                    StatusId: statusId
                };

                return $http.get(baseUrl + "api/EOQ/GetEOQNominationByNominatorId", { params: params });
            }
        };
    }
})(angular.module("assessmentModule"));