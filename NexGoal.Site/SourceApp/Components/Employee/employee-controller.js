'use strict';
/*global employeeModule*/

employeeModule.controller('EmployeeController',
['$window', 'EmployeeModel', 'utilitiesFactory', 'CommonModel', 'enumerationsFactory', "lockScreenService",
function ($window, EmployeeModel, utilitiesFactory, CommonModel, enumerationsFactory, lockScreenService) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.permission = EmployeeModel.permission;
    vm.policyEnum = enumerationsFactory.policy;

    vm.filter = {
        query: '',
        groupId: '',
        isLocal: '',
        isSpecialUser: '',
        isDisabledUser: '',
        hasMonthlyEPA: ''
    };

    vm.groupOptions = [];
    vm.currentPage = 1;

    utilitiesFactory.numberEntryList(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getUserPermissionOnEmployee().then(function () {
        vm.permission = EmployeeModel.permission;
    });

    var getEmployee = function (query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
        if (!hasMonthlyEPA) {
            hasMonthlyEPA = '';
        }

        lockScreenService.start();

        EmployeeModel.getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA).then(function () {
            vm.employees = EmployeeModel.employees;
            utilitiesFactory.searchOnPage(vm, vm.employees);
        })["finally"](function () {
            lockScreenService.stop();
        });
    };

    getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);

    vm.queryEmployee = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getEmployee(filter.query, filter.groupId, filter.isLocal, filter.isSpecialUser, filter.isDisabledUser, filter.hasMonthlyEPA);
    };

    /*
    ** Get index of row in table.
    */
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.assignRole = function (employee) {
        EmployeeModel.openPopupAssignRole(employee);
    };

    vm.updateHasMonthlyEPA = function (employee) {
        EmployeeModel.openPopupUpdateHasMonthlyEPA(vm, employee);
    };
}]);

employeeModule.controller('AssignRoleToUserController',
['$modalInstance', 'EmployeeModel', 'employee', 'CommonModel',
function ($modalInstance, EmployeeModel, employee, CommonModel) {
    var vm = this;

    vm.employee = employee;
    vm.systemRoles = [];

    CommonModel.getSystemRoles(true).then(function () {
        vm.systemRoles = CommonModel.systemRoles;
    });

    vm.checkRolePermission = function (roleId) {
        var isChecked = false;
        angular.forEach(vm.employee.Roles, function (role) {
            if (role.RoleId === roleId) {
                isChecked = true;
            }
        });

        return isChecked;
    };

    vm.save = function (roleIds) {
        var roles = [];

        if (roleIds) {
            roles = EmployeeModel.createRoles(roleIds, vm.systemRoles);
        }

        var editedRole = {
            roles: roles,
            roleIds: roleIds
        };
        $modalInstance.close(editedRole);
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

employeeModule.controller("ModalConfirmSetUnsetMonthlyEvaluationController",
['$modalInstance', 'name', 'action',
function ($modalInstance, name, action) {
    var vm = this;

    vm.name = name;
    vm.action = action;

    vm.ok = function () {
        $modalInstance.close();
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);