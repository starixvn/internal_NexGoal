(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOQNominationController', updateEOQNominationController);

    updateEOQNominationController.$inject = ['$filter', '$modalInstance', 'EOQModel', 'utilitiesFactory', 'nominationId'];

    function updateEOQNominationController($filter, $modalInstance, EOQModel, utilitiesFactory, nominationId) {
        var vm = this;

        vm.nomination = {};

        function initialize() {
            utilitiesFactory.getDateFormat(vm);

            EOQModel.getEOQNominationById(nominationId).then(function () {
                vm.nomination = EOQModel.nomination;
                vm.nomination.Period.DueDate = $filter('date')(vm.nomination.Period.EndDate, vm.dateFormat);
            });
        }

        vm.submit = function () {
            var nominee = {
                nominationId: nominationId,
                employeeId: vm.nomination.Nominee.EmployeeId,
                eoqQuestions: vm.nomination.NominationDetails
            };

            if (EOQModel.validateNominee(nominee)) {
                $modalInstance.close(nominee);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));