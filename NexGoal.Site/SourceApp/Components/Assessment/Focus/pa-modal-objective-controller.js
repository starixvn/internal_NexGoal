(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalObjectiveController", paModalObjectiveController);

    paModalObjectiveController.$inject = ["$modalInstance", "member"];

    function paModalObjectiveController($modalInstance, member) {
        var vm = this;

        vm.employeeId = member.EmployeeId;
        vm.objectives = member.Objectives;

        vm.ok = function () {
            $modalInstance.close(vm.objectives);
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("assessmentModule"));