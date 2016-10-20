(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("CreateEOYNominationController", createEOYNominationController);

    createEOYNominationController.$inject = ["$filter", "$modalInstance", "EmployeeModel", "EOYModel", "utilitiesFactory", "eoyNominationPeriod"];

    function createEOYNominationController($filter, $modalInstance, EmployeeModel, EOYModel, utilitiesFactory, eoyNominationPeriod) {
        var vm = this;
        var originEOYQuestions = [];

        vm.eoyNominationPeriod = eoyNominationPeriod;
        vm.searchEmployee = "";
        vm.employee = null;
        vm.nominee = {
            employeeId: 0,
            teamName: "",
            eoyQuestions: []
        };

        function initialize() {
            EOYModel.getEOYQuestion().then(function () {
                angular.forEach(EOYModel.eoyQuestions, function (question) {
                    question.IsValid = true;
                });

                vm.nominee.eoyQuestions = EOYModel.eoyQuestions;
                originEOYQuestions = angular.copy(EOYModel.eoyQuestions);
            });

            utilitiesFactory.getDateFormat(vm);

            vm.eoyNominationPeriod.DueDay = $filter('date')(vm.eoyNominationPeriod.Period.EndDate, vm.dateFormat);
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
            if (EOYModel.validateNominee(vm.nominee)) {
                EOYModel.createEOYNomination(vm.eoyNominationPeriod.Period.EOQYId, vm.nominee.employeeId, vm.nominee.eoyQuestions, $modalInstance);
            }
        };

        vm.cancel = function () {
            if (EOYModel.isQuestionModified(originEOYQuestions, vm.nominee.eoyQuestions)) {
                var cancelConfirm = confirm(EOYModel.confirmCancelFormMessage);

                if (cancelConfirm) {
                    $modalInstance.dismiss("cancel");
                }
            } else {
                $modalInstance.dismiss("cancel");
            }
        };

        initialize();
    }
})(angular.module("assessmentModule"));