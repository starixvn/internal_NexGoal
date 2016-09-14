(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYNominationController', updateEOYNominationController);

    updateEOYNominationController.$inject = ['$filter', '$modalInstance', 'EOYModel', 'utilitiesFactory', 'nominationId'];

    function updateEOYNominationController($filter, $modalInstance, EOYModel, utilitiesFactory, nominationId) {
        var vm = this;

        vm.nomination = {
            nominationId: nominationId,
            employeeId: 0,
            eoyQuestions: [],
            fullName: "",
            groupName: "",
            dueDate: ""
        };

        function initialize() {
            utilitiesFactory.getDateFormat(vm);

            EOYModel.getEOYNominationById(nominationId).then(function () {
                angular.forEach(EOYModel.nomination.NominationDetails, function (nominationDetail) {
                    nominationDetail.IsValid = true;
                });

                vm.nomination.fullName = EOYModel.nomination.Nominee.FullName;
                vm.nomination.groupName = EOYModel.nomination.Nominee.Group.GroupName;
                vm.nomination.dueDate = $filter('date')(EOYModel.nomination.Period.EndDate, vm.dateFormat);
                vm.nomination.employeeId = EOYModel.nomination.Nominee.EmployeeId;
                vm.nomination.eoyQuestions = EOYModel.nomination.NominationDetails;
            });
        }

        vm.submit = function () {
            if (EOYModel.validateNominee(vm.nomination)) {
                $modalInstance.close(vm.nomination);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));