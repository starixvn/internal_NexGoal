(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ViewEOQNominationController', viewEOQNominationController);

    viewEOQNominationController.$inject = ['$filter', '$modalInstance', 'EOQModel', 'utilitiesFactory', 'lockScreenService', 'nominationId'];

    function viewEOQNominationController($filter, $modalInstance, EOQModel, utilitiesFactory, lockScreenService, nominationId) {
        var vm = this;

        vm.nomination = {};

        function initialize() {
            lockScreenService.start();

            utilitiesFactory.getDateFormat(vm);

            EOQModel.getEOQNominationById(nominationId).then(function () {
                vm.nomination = EOQModel.nomination;
                vm.nomination.Period.DueDate = $filter('date')(vm.nomination.Period.EndDate, vm.dateFormat);
            })['finally'](function () {
                lockScreenService.stop();
            });
        }

        vm.close = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));