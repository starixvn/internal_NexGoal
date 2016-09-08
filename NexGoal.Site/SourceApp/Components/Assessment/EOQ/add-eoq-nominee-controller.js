(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("AddEOQNomineeController", addEOQNomineeController);

    addEOQNomineeController.$inject = ["$modalInstance", "EmployeeModel","EOQModel"];

    function addEOQNomineeController($modalInstance, EmployeeModel, EOQModel) {
        var vm = this;

        vm.searchEmployee = "";
        vm.employee = null;
        vm.nominee = {
            employeeId: 0,
            teamName: ""
        };
        vm.eoqQuestions = [];

        function initialize() {
            EOQModel.getEOQQuestion().then(function () {
                vm.eoqQuestions = EOQModel.eoqQuestions;
            });
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

        initialize();
    }
})(angular.module("assessmentModule"));