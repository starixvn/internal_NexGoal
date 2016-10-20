(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYNominationController', updateEOYNominationController);

    updateEOYNominationController.$inject = ['$filter', '$modalInstance', 'EOYModel', 'utilitiesFactory', 'nominationId'];

    function updateEOYNominationController($filter, $modalInstance, EOYModel, utilitiesFactory, nominationId) {
        var vm = this;
        var originEOYQuestions = [];

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

                originEOYQuestions = angular.copy(EOYModel.nomination.NominationDetails);
            });
        }

        vm.submit = function () {
            if (EOYModel.validateNominee(vm.nomination)) {
                EOYModel.updateEOYNomination(vm.nomination.nominationId, vm.nomination.eoyQuestions, $modalInstance);
            }
        };

        vm.cancel = function () {
            if (EOYModel.isQuestionModified(originEOYQuestions, vm.nomination.eoyQuestions)) {
                var cancelConfirm = confirm("Confirm message goes here.");

                if (cancelConfirm) {
                    $modalInstance.dismiss("cancel");
                }
            } else {
                $modalInstance.dismiss("cancel");
            }
        };

        initialize();
    }
})(angular.module('assessmentModule'));