(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalInformationController", paModalInformationController);

    paModalInformationController.$inject = ["$scope", "$modalInstance"];

    function paModalInformationController($scope, $modalInstance) {
        var vm = this;

        function initialize() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("assessmentModule"));