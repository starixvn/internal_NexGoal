(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalLastObjectiveController", paModalLastObjectiveController);

    paModalLastObjectiveController.$inject = ["$scope", "$modalInstance", "notificationFactory", "member"];

    function paModalLastObjectiveController($scope, $modalInstance, notificationFactory, member) {
        var vm = this;

        vm.employeeId = member.EmployeeId;
        vm.LastCompletedPercentObjective = member.LastCompletedPercentObjective;
        vm.IsReadOnly = member.IsReadOnly;

        function initialize() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss("cancel");
            });
        }

        vm.ok = function () {
            if (!vm.LastCompletedPercentObjective) {
                notificationFactory.error("ObjectivesRequired");
            } else {
                $modalInstance.close(vm.LastCompletedPercentObjective);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("assessmentModule"));