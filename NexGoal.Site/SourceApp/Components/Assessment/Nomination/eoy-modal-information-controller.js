(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOYModalInformationController', eoyModalInformationController);

    eoyModalInformationController.$inject = ['$modalInstance'];

    function eoyModalInformationController($modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss('cancel');
        };
    }
})(angular.module('assessmentModule'));