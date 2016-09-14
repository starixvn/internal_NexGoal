(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYNominationController', updateEOYNominationController);

    updateEOYNominationController.$inject = ['$filter', '$modalInstance', 'EOYModel', 'utilitiesFactory', 'nominationId'];

    function updateEOYNominationController($filter, $modalInstance, EOYModel, utilitiesFactory, nominationId) {
        var vm = this;

        vm.nomination = {};

        function initialize() {
            utilitiesFactory.getDateFormat(vm);

            EOYModel.getEOYNominationById(nominationId).then(function () {
                vm.nomination = EOYModel.nomination;
                vm.nomination.Period.DueDate = $filter('date')(vm.nomination.Period.EndDate, vm.dateFormat);
            });
        }

        vm.submit = function () {
            var nominee = {
                nominationId: nominationId,
                employeeId: vm.nomination.Nominee.EmployeeId,
                eoqQuestions: vm.nomination.NominationDetails
            };

            if (EOYModel.validateNominee(nominee)) {
                $modalInstance.close(nominee);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));