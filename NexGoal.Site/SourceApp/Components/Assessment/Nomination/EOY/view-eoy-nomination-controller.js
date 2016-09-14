(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ViewEOYNominationController', viewEOYNominationController);

    viewEOYNominationController.$inject = ['$filter', '$modalInstance', 'EOYModel', 'utilitiesFactory', 'lockScreenService', 'nominationId'];

    function viewEOYNominationController($filter, $modalInstance, EOYModel, utilitiesFactory, lockScreenService, nominationId) {
        var vm = this;

        vm.nomination = {};

        function initialize() {
            lockScreenService.start();

            utilitiesFactory.getDateFormat(vm);

            EOYModel.getEOYNominationById(nominationId).then(function () {
                vm.nomination = EOYModel.nomination;
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