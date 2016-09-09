(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOQModalInformationController', eoqModalInformationController);

    eoqModalInformationController.$inject = ['$modalInstance'];

    function eoqModalInformationController($modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss('cancel');
        };
    }
})(angular.module('assessmentModule'));