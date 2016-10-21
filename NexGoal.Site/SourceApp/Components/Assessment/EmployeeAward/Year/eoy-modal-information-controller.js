(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOYModalInformationController', eoyModalInformationController);

    eoyModalInformationController.$inject = ['$scope', '$modalInstance'];

    function eoyModalInformationController($scope, $modalInstance) {
        var vm = this;

        function initialze() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss("cancel");
            });
        }

        vm.close = function () {
            $modalInstance.dismiss('cancel');
        };

        initialze();
    }
})(angular.module('assessmentModule'));