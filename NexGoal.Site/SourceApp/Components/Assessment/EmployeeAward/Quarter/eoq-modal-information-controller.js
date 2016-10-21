(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOQModalInformationController', eoqModalInformationController);

    eoqModalInformationController.$inject = ["$modalInstance", "$scope"];

    function eoqModalInformationController($modalInstance, $scope) {
        var vm = this;

        function initialize() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        vm.close = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));