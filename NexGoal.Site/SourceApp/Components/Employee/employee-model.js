'use strict';
/*global employeeModule*/

employeeModule.factory('EmployeeModel',
['$modal', 'employeeFactory', 'errorFactory', 'notificationFactory',
function ($modal, employeeFactory, errorFactory, notificationFactory) {
    var employeeModel = {};

    employeeModel.employees = [];
    employeeModel.groups = [];
    employeeModel.jobTitles = [];
    employeeModel.permission = 0;

    employeeModel.getAllEmployee = function (query, groupId, isLocal, isSpecialUser, isDisabledUser) {
        return employeeFactory.getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser)
        .success(function (employees) {
            employeeModel.employees = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.getAllGroup = function () {
        return employeeFactory.getAllGroup()
        .success(function (groups) {
            employeeModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.getAllJobTitle = function () {
        return employeeFactory.getAllJobTitle().success(function (jobTitles) {
            employeeModel.jobTitles = jobTitles;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.openPopupAssignRole = function (employee) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/assignRole.html',
            controller: "AssignRoleToUserController as assignRoleCtrl",
            backdrop: 'static',
            resolve: {
                employee: function () {
                    return angular.copy(employee);
                }
            },
            windowClass: "modal-confirm-center"
        });

        return modalInstance.result;
    };

    employeeModel.assignRoleToEmployee = function (employeeId, roleIds) {
        var employeeRoles = {
            employeeId: employeeId,
            roleIds: roleIds
        };

        return employeeFactory.assignRoleToEmployee(employeeRoles).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.createRoles = function (roleIds, systemRoles) {
        var roles = [],
            roleIdList = roleIds.split(",");

        angular.forEach(roleIdList, function (roleId) {
            angular.forEach(systemRoles, function (role) {
                if (parseInt(roleId, 10) === role.RoleId) {
                    roles.push(angular.copy(role));
                }
            });
        });

        return roles;
    };

    employeeModel.getUserPermissionOnEmployee = function () {
        return employeeFactory.getUserPermissionOnEmployee()
        .success(function (permission) {
            employeeModel.permission = parseInt(permission, 10);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return employeeModel;
}]);