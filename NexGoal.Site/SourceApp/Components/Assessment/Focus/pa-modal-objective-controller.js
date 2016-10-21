(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalObjectiveController", paModalObjectiveController);

    paModalObjectiveController.$inject = ["$scope", "$modalInstance", "notificationFactory", "member"];

    function paModalObjectiveController($scope, $modalInstance, notificationFactory, member) {
        var vm = this;

        vm.employeeId = member.EmployeeId;
        vm.NextDescriptionObjective = member.NextDescriptionObjective;
        vm.IsReadOnly = member.IsReadOnly;

        function initialize() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

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

        initialize();
    }
})(angular.module("assessmentModule"));