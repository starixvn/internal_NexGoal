(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOQNominationController', updateEOQNominationController);

    updateEOQNominationController.$inject = ['$filter', '$modalInstance', 'EOQModel', 'utilitiesFactory', 'nominationId'];

    function updateEOQNominationController($filter, $modalInstance, EOQModel, utilitiesFactory, nominationId) {
        var vm = this;

        vm.nomination = {
            nominationId: nominationId,
            employeeId: 0,
            eoqQuestions: [],
            fullName: "",
            groupName: "",
            dueDate: ""
        };

        function initialize() {
            utilitiesFactory.getDateFormat(vm);

            EOQModel.getEOQNominationById(nominationId).then(function () {
                angular.forEach(EOQModel.nomination.NominationDetails, function (nominationDetail) {
                    nominationDetail.IsValid = true;
                });

                vm.nomination.fullName = EOQModel.nomination.Nominee.FullName;
                vm.nomination.groupName = EOQModel.nomination.Nominee.Group.GroupName;
                vm.nomination.dueDate = $filter('date')(EOQModel.nomination.Period.EndDate, vm.dateFormat);
                vm.nomination.employeeId = EOQModel.nomination.Nominee.EmployeeId;
                vm.nomination.eoqQuestions = EOQModel.nomination.NominationDetails;
            });
        }

        vm.submit = function () {
            if (EOQModel.validateNominee(vm.nomination)) {
                $modalInstance.close(vm.nomination);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular.module('assessmentModule'));