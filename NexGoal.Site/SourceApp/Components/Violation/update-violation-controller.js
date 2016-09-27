(function (violationModule) {
    "use strict";

    violationModule.controller("UpdateViolationController", updateViolationController);

    updateViolationController.$inject = ["$modalInstance", "ViolationModel", "violation"];

    function updateViolationController($modalInstance, ViolationModel, violation) {
        var vm = this;

        vm.violation = {
            ViolationId: violation.ViolationId,
            EmployeeId: violation.Employee.EmployeeId,
            Description: violation.Description,
            EffectedDate: violation.EffectedDate,
            RawEffectedDate: new Date(violation.EffectedDate),
            EmployeeFullName: violation.Employee.FullName + " (" + violation.Employee.NickName + ")"
        };
        vm.calendar = { EffectedDateOpened: false };

        vm.openPopupEffectedDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.EffectedDateOpened = true;
        };

        vm.submit = function () {
            if (ViolationModel.validateViolation(vm.violation)) {
                vm.violation.EffectedDate = vm.violation.RawEffectedDate.toDateString();

                ViolationModel.updateViolation(vm.violation.ViolationId,
                                                vm.violation.EffectedDate,
                                                vm.violation.Description,
                                                $modalInstance);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("violationModule"));