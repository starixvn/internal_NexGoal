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
            RawEffectedDate: new Date(),
            EffectedDate: null,
            Description: null
        };
        vm.calendar = { EffectedDateOpened: false };

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

        vm.openPopupEffectedDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.EffectedDateOpened = true;
        };

        vm.submit = function () {
            if (vm.filteredEmployee) {
                vm.violation.EmployeeId = vm.filteredEmployee.EmployeeId;
            }

            if (ViolationModel.validateViolation(vm.violation)) {
                var createConfirmed = confirm("Do you want to send this warning to " + vm.filteredEmployee.Name + "!");
                if (createConfirmed) {
                    vm.violation.EffectedDate = vm.violation.RawEffectedDate.toDateString();

                    ViolationModel.createViolation(vm.violation.EmployeeId,
                                                    vm.violation.EffectedDate,
                                                    vm.violation.Description,
                                                    $modalInstance);
                }


            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("violationModule"));