(function (violationModule) {
    "use strict";

    violationModule.controller("UpdateViolationController", updateViolationController);

    updateViolationController.$inject = ["$modalInstance", "ViolationModel", "violation"];

    function updateViolationController($modalInstance, ViolationModel, violation) {
        var vm = this;

        vm.violation = violation;
        vm.calendar = { EffectiveDateOpened: false };

        vm.openPopupEffectiveDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.EffectiveDateOpened = true;
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("violationModule"));