(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("eoyFactory", eoyFactory);

    eoyFactory.$inject = ["$http", "baseUrl"];

    function eoyFactory($http, baseUrl) {
        return {
            createEOY: function (period) {
                var criteria = {
                    Quarter: period.Quarter,
                    Year: period.Year,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.post(baseUrl + "api/EOY/CreateEOY", criteria);
            },
            createEOYNomination: function (eoqyId, employeeId, eoyQuestions) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: employeeId,
                    NominationDetails: eoyQuestions
                };

                return $http.post(baseUrl + "api/Nomination/CreateEOYNomination", criteria);
            },
            deleteEOY: function (eoyId) {
                return $http.delete(baseUrl + "api/EOY/DeleteEOY", { params: { Id: eoyId } });
            },
            deleteEOYNomination: function (nominationId) {
                return $http.delete(baseUrl + "api/Nomination/DeleteNomination", { params: { Id: nominationId } });
            },
            updateEOY: function (period) {
                var criteria = {
                    EOQYId: period.EOQYId,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.put(baseUrl + "api/EOY/UpdateEOY", criteria);
            },
            updateEOYNomination: function (nominationId, eoyQuestions) {
                var criteria = {
                    NominationId: nominationId,
                    NominationDetails: eoyQuestions
                };

                return $http.put(baseUrl + "api/Nomination/UpdateNomination", criteria);
            },
            getEOY: function (year, statusId) {
                return $http.get(baseUrl + "api/EOY/GetEOY", { params: { Year: year, StatusId: statusId } });
            },
            getEOYById: function (eoyId) {
                return $http.get(baseUrl + "api/EOY/GetEOYById", { params: { Id: eoyId } });
            },
            getEOYNominatorSettingById: function (eoqyId, query, groupIds, roleId, showAllEmployee) {
                var params = {
                    EOQYId: eoqyId,
                    Query: query,
                    GroupIds: groupIds,
                    RoleId: roleId,
                    ShowAllEmployee: showAllEmployee
                };

                return $http.get(baseUrl + "api/EOY/GetEOYNominatorSettingById", { params: params });
            },
            getUserPermissionOnManageNomination: function () {
                return $http.get(baseUrl + "api/Nomination/GetUserPermissionOnManageNomination");
            },
            getUserPermissionOnNomination: function () {
                return $http.get(baseUrl + "api/Nomination/GetUserPermissionOnManageNomination");
            },
            updateEOYNominatorSettingById: function (eoqyId, nominators) {
                var criteria = {
                    EOQYId: eoqyId,
                    Nominators: nominators
                };

                return $http.put(baseUrl + "api/EOY/UpdateEOYNominatorSettingById", criteria);
            },
            updateStatusEOYNomination: function (eoqyId, nomineeId, statusId) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: nomineeId,
                    StatusId: statusId
                };

                return $http.put(baseUrl + "api/Nomination/UpdateStatusNomination", criteria);
            },
            getEOYQuestion: function () {
                return $http.get(baseUrl + "api/Nomination/GetEOYQuestion");
            },
            getCurrentEOYByNominatorId: function () {
                return $http.get(baseUrl + "api/Nomination/GetCurrentEOYByNominatorId");
            },
            getEOYNominationByNominatorId: function (eoqyId, roleId, query, teamIds, statusId) {
                var params = {
                    EOQYId: eoqyId,
                    RoleId: roleId,
                    Query: query,
                    GroupIds: teamIds,
                    StatusId: statusId
                };

                return $http.get(baseUrl + "api/Nomination/GetNominationByNominatorId", { params: params });
            },
            getEOYNominationById: function (nominationId) {
                var params = { NominationId: nominationId };

                return $http.get(baseUrl + "api/Nomination/GetNominationById", { params: params });
            }
        };
    }
})(angular.module("assessmentModule"));