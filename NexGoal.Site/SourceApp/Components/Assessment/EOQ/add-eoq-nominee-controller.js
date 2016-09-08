(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("AddEOQNomineeController", addEOQNomineeController);

    addEOQNomineeController.$inject = ["$modalInstance", "EmployeeModel"];

    function addEOQNomineeController($modalInstance, EmployeeModel) {
        var vm = this;

        vm.searchEmployee = "";
        vm.employee = null;
        vm.nominee = {
            employeeId: 0,
            teamName: ""
        };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                vm.employee = null;
                vm.nominee.teamName = "";
                vm.nominee.employeeId = 0;
            }
        };

        vm.recordEmployee = function () {
            if (vm.employee) {
                vm.nominee.teamName = vm.employee.TeamName;
                vm.nominee.employeeId = vm.employee.EmployeeId;
            } else {
                vm.nominee.teamName = "";
                vm.nominee.employeeId = 0;
            }
        }

        vm.submit = function () {
            $modalInstance.dismiss("cancel");
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("assessmentModule"));