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
            deleteEOY: function (eoyId) {
                return $http['delete'](baseUrl + "api/EOY/DeleteEOY", { params: { Id: eoyId } });
            },
            updateEOY: function (period) {
                var criteria = {
                    EOQYId: period.EOQYId,
                    Year: period.Year,
                    Description: period.Description,
                    StartDate: period.StartDate,
                    EndDate: period.EndDate
                };

                return $http.put(baseUrl + "api/EOY/UpdateEOY", criteria);
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
            updateEOYNominatorSettingById: function (eoqyId, nominators) {
                var criteria = {
                    EOQYId: eoqyId,
                    Nominators: nominators
                };

                return $http.put(baseUrl + "api/EOY/UpdateEOYNominatorSettingById", criteria);
            },
            getCurrentEOYByNominatorId: function () {
                return $http.get(baseUrl + "api/Nomination/GetCurrentEOYByNominatorId");
            }
        };
    }
})(angular.module("assessmentModule"));