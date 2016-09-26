(function (violationModule) {
    "use strict";

    violationModule.controller("CreateViolationController", createViolationController);

    createViolationController.$inject = ["$modalInstance", "EmployeeModel", "ViolationModel"];

    function createViolationController($modalInstance, EmployeeModel, ViolationModel) {
        var vm = this;

        vm.searchEmployee = "";
        vm.filteredEmployee = null;
        vm.violation = {
            EmployeeId: null,
            RawEffectiveDate: new Date(),
            EffectiveDate: null,
            WarningContent: null
        };
        vm.calendar = { EffectiveDateOpened: false };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === "") {
                if (vm.filteredEmployee) {
                    vm.violation.EmployeeId = null;
                    vm.filteredEmployee = null;
                }
            }
        };

        vm.openPopupEffectiveDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.EffectiveDateOpened = true;
        };

        vm.submit = function () {
            if (vm.filteredEmployee) {
                vm.violation.EmployeeId = vm.filteredEmployee.EmployeeId;
            }

            if (ViolationModel.validateViolation(vm.violation)) {
                vm.violation.EffectiveDate = vm.violation.RawEffectiveDate.toDateString();

                ViolationModel.createViolation(vm.violation, $modalInstance);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("violationModule"));