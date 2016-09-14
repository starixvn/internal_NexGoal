(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("CreateEOQNominationController", createEOQNomination);

    createEOQNomination.$inject = ["$filter", "$modalInstance", "EmployeeModel", "EOQModel", "utilitiesFactory", "eoqNominationPeriod"];

    function createEOQNomination($filter, $modalInstance, EmployeeModel, EOQModel, utilitiesFactory, eoqNominationPeriod) {
        var vm = this;

        vm.eoqNominationPeriod = eoqNominationPeriod;
        vm.searchEmployee = "";
        vm.employee = null;
        vm.nominee = {
            employeeId: 0,
            teamName: "",
            eoqQuestions: []
        };

        function initialize() {
            EOQModel.getEOQQuestion().then(function () {
                angular.forEach(EOQModel.eoqQuestions, function (question) {
                    question.IsValid = true;
                });

                vm.nominee.eoqQuestions = EOQModel.eoqQuestions;
            });

            utilitiesFactory.getDateFormat(vm);

            vm.eoqNominationPeriod.DueDay = $filter('date')(vm.eoqNominationPeriod.Period.EndDate, vm.dateFormat);
        }

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
        };

        vm.submit = function () {
            if (EOQModel.validateNominee(vm.nominee)) {
                $modalInstance.close(vm.nominee);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("assessmentModule"));