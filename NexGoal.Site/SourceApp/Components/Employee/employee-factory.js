'use strict';
/*global employeeModule*/

employeeModule.factory('employeeFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAllEmployee: function (query, groupId, isLocal, isSpecialUser, isDisabledUser) {
            return $http.get(baseUrl + 'api/Employee/GetAllEmployee',
                {
                    params: {
                        query: query,
                        groupId: groupId,
                        isLocal: isLocal,
                        isSpecialUser: isSpecialUser,
                        isDisabledUser: isDisabledUser
                    }
                });
        },
        getAllGroup: function () {
            return $http.get(baseUrl + 'api/Employee/GetAllGroup');
        },
        getAllJobTitle: function () {
            return $http.get(baseUrl + 'api/Employee/GetAllJobTitle');
        },
        assignRoleToEmployee: function (employeeRoles) {
            return $http.put(baseUrl + 'api/Employee/AssignRoleToEmployee', employeeRoles);
        },
        getUserPermissionOnEmployee: function () {
            return $http.get(baseUrl + 'api/Employee/GetUserPermissionOnEmployee');
        }
    };
}]);