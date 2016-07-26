'use strict';
/*global employeeModule*/

employeeModule.factory('EmployeeModel',
['$modal', 'employeeFactory', 'errorFactory', 'notificationFactory',
function ($modal, employeeFactory, errorFactory, notificationFactory) {
    var employeeModel = {},
        emailPattern = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

    employeeModel.employees = [];
    employeeModel.groups = [];
    employeeModel.jobTitles = [];
    employeeModel.permission = 0;

    employeeModel.getAllEmployee = function (query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
        return employeeFactory.getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA)
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

    var findEmployeeJobTitleById = function (jobTitleId) {
        var jobTitleResult = {};

        angular.forEach(employeeModel.jobTitles, function (jobTitle) {
            if (jobTitle.JobTitleId === jobTitleId) {
                jobTitleResult = jobTitle;
            }
        });

        return jobTitleResult;
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

        modalInstance.result.then(function (editedRole) {
            employeeModel.assignRoleToEmployee(employee.EmployeeId, editedRole.roleIds).then(function () {
                employee.Roles = editedRole.roles;
            });
        })['finally'](function () {
            modalInstance = '';
        });
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

    employeeModel.openPopupUpdateHasMonthlyEPA = function (vm, employee) {
        employee.HasMonthlyEPA = !employee.HasMonthlyEPA;

        var action = employee.HasMonthlyEPA ? "set" : "unset",
        newEmployee = {
            EmployeeId: employee.EmployeeId,
            HasMonthlyEPA: employee.HasMonthlyEPA
        };

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/modalConfirmSetUnsetMonthlyEvaluation.html',
            controller: "ModalConfirmSetUnsetMonthlyEvaluationController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return employee.NickName;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            employeeFactory.updateHasMonthlyEPA(newEmployee)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            });
        }, function () {
            employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
        })['finally'](function () {
            modalInstance = '';
        });
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
} ]);