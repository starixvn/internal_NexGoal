(function (assessmentModule) {
    'use strict';

    assessmentModule.factory('nominationFactory', nominationFactory);

    nominationFactory.$inject = ['$http', 'baseUrl'];

    function nominationFactory($http, baseUrl) {
        return {
            createEOQNomination: function (eoqyId, employeeId, eoqQuestions) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: employeeId,
                    NominationDetails: eoqQuestions
                };

                return $http.post(baseUrl + 'api/Nomination/CreateEOQNomination', criteria);
            },
            createEOYNomination: function (eoqyId, employeeId, eoyQuestions) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: employeeId,
                    NominationDetails: eoyQuestions
                };

                return $http.post(baseUrl + 'api/Nomination/CreateEOYNomination', criteria);
            },
            deleteNomination: function (nominationId) {
                return $http['delete'](baseUrl + 'api/Nomination/DeleteNomination', { params: { Id: nominationId } });
            },
            getCurrentEOQByNominatorId: function () {
                return $http.get(baseUrl + 'api/Nomination/GetCurrentEOQByNominatorId');
            },
            getCurrentEOYByNominatorId: function () {
                return $http.get(baseUrl + 'api/Nomination/GetCurrentEOYByNominatorId');
            },
            getEOQQuestion: function () {
                return $http.get(baseUrl + 'api/Nomination/GetEOQQuestion');
            },
            getEOYQuestion: function () {
                return $http.get(baseUrl + 'api/Nomination/GetEOYQuestion');
            },
            getNominationByNominatorId: function (eoqyId, roleId, query, teamIds, statusId) {
                var params = {
                    EOQYId: eoqyId,
                    RoleId: roleId,
                    Query: query,
                    GroupIds: teamIds,
                    StatusId: statusId
                };

                return $http.get(baseUrl + 'api/Nomination/GetNominationByNominatorId', { params: params });
            },
            getNominationById: function (nominationId) {
                var params = { NominationId: nominationId };

                return $http.get(baseUrl + 'api/Nomination/GetNominationById', { params: params });
            },
            getUserPermissionOnManageNomination: function () {
                return $http.get(baseUrl + 'api/Nomination/GetUserPermissionOnManageNomination');
            },
            getUserPermissionOnNomination: function () {
                return $http.get(baseUrl + 'api/Nomination/GetUserPermissionOnManageNomination');
            },
            updateNomination: function (nominationId, eoyQuestions) {
                var criteria = {
                    NominationId: nominationId,
                    NominationDetails: eoyQuestions
                };

                return $http.put(baseUrl + 'api/Nomination/UpdateNomination', criteria);
            },
            updateStatusNomination: function (eoqyId, nomineeId, statusId) {
                var criteria = {
                    EOQYId: eoqyId,
                    NomineeId: nomineeId,
                    StatusId: statusId
                };

                return $http.put(baseUrl + 'api/Nomination/UpdateStatusNomination', criteria);
            }
        };
    }
})(angular.module('assessmentModule'));