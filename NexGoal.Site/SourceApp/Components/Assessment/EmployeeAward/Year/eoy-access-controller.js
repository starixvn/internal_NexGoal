(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOYAccessController', eoyAccessController);

    eoyAccessController.$inject = ['$scope', 'EOYModel', 'CommonModel', 'enumerationsFactory', 'utilitiesFactory', 'lockScreenService'];

    function eoyAccessController($scope, EOYModel, CommonModel, enumerationsFactory, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.roleId = 0;
        vm.NOMINATOR_ROLE = 1;
        vm.COMMITTEE_ROLE = 2;
        vm.PERIOD_IN_PROGRESS_STATUS = 1;
        vm.PERIOD_CLOSED_STATUS = 2;
        vm.period = null;
        vm.teams = [];
        vm.permission = 0;
        vm.filter = {
            query: '',
            teamIds: '',
            statusId: ''
        };
        vm.eoyNominations = [];
        vm.nominationStatus = [
            {
                Value: 1,
                Name: 'Nominated'
            },
            {
                Value: 2,
                Name: 'Shortlisted'
            },
            {
                Value: 3,
                Name: 'Winner'
            }
        ];
        vm.policyEnum = enumerationsFactory.policy;

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            EOYModel.getCurrentEOYByNominatorId().then(function () {
                vm.period = EOYModel.period;
                vm.roleId = vm.period.Roles.length > 0 ? vm.period.Roles[0] : 0;

                vm.query();
            });

            EOYModel.getUserPermissionOnNomination().then(function () {
                vm.permission = EOYModel.permission;
            });
        }

        vm.query = function () {
            if (vm.period) {
                lockScreenService.start();

                vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, 'DepartmentId');

                EOYModel.getEOYNominationByNominatorId(
                    vm.period.Period.EOQYId,
                    vm.filter.query,
                    vm.filter.teamIds,
                    vm.filter.statusId).then(function () {
                        vm.eoyNominations = EOYModel.eoyNominations;
                    })['finally'](function () {
                        lockScreenService.stop();
                    });
            }
        };

        vm.addNominee = function () {
            EOYModel.openPopupAddNominee(vm.period).then(function () {
                vm.modalInstance = null;
                vm.query();
            });
        };

        vm.deleteNomination = function (nomination) {
            EOYModel.openPopupDeleteEOYNomination(vm, nomination);
        };

        vm.viewNomination = function (nomination) {
            EOYModel.openPopupViewNomination(nomination.NominationId).then(function () {
                vm.modalInstance = null;
            });
        };

        vm.updateNomination = function (nomination) {
            EOYModel.openPopupUpdateNomination(nomination.NominationId).then(function () {
                vm.modalInstance = null;
                vm.query();
            });
        };

        vm.updateStatusNomination = function (nomination) {
            lockScreenService.start();

            EOYModel.updateStatusEOYNomination(nomination.Period.EOQYId, nomination.Nominee.EmployeeId, nomination.StatusId)['finally'](function () {
                lockScreenService.stop();
            });
        };

        vm.openPopupInformation = function () {
            EOYModel.openPopupInformation();
        };

        vm.exportExcel = function () {
            vm.exportCriteria = {
                EOQYId: vm.period.Period.EOQYId,
                Query: vm.filter.query,
                GroupIds: vm.filter.teamIds,
                StatusId: vm.filter.statusId,
                PeriodName: vm.period.Period.Year
            };

            $scope.$broadcast('exportEmployeeOfTheYear');
        };

        initialize();
    }
})(angular.module('assessmentModule'));