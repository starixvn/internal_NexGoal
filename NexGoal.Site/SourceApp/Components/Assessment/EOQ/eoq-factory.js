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
            GetEOQNominatorSettingById: function (query, groupIds, roleId, showAllEmployee) {
                var params = {
                    Query: query,
                    GroupIds: groupIds,
                    RoleId: roleId,
                    ShowAllEmployee: showAllEmployee
                };

                return $http.get(baseUrl + "api/EOQ/GetEOQNominatorSettingById", { params: params });
            }
        };
    }
})(angular.module("assessmentModule"));