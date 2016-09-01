(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalObjectiveController", paModalObjectiveController);

    paModalObjectiveController.$inject = ["$modalInstance","notificationFactory", "member"];

    function paModalObjectiveController($modalInstance, notificationFactory, member) {
        var vm = this;

        vm.employeeId = member.EmployeeId;
        vm.NextDescriptionObjective = member.NextDescriptionObjective;

        vm.ok = function () {
            if (!vm.NextDescriptionObjective) {
                notificationFactory.error("ObjectivesRequired");
            } else {
                $modalInstance.close(vm.NextDescriptionObjective);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("assessmentModule"));