(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ViewEOYNominationController', viewEOYNominationController);

    viewEOYNominationController.$inject = ['$scope', '$filter', '$modalInstance', 'EOYModel',
        'utilitiesFactory', 'lockScreenService', 'nominationId'];

    function viewEOYNominationController($scope, $filter, $modalInstance, EOYModel,
        utilitiesFactory, lockScreenService, nominationId) {
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