(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('EOQAccessController', eoqAccessController);

    eoqAccessController.$inject = ['$scope', 'EOQModel', 'CommonModel', 'enumerationsFactory', 'utilitiesFactory', 'lockScreenService'];

    function eoqAccessController($scope, EOQModel, CommonModel, enumerationsFactory, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.IN_BOARD_ROLE = 2;
        vm.NOMINATOR_ROLE = 1;
        vm.IN_PROGRESS_STATUS = 1;
        vm.period = {};
        vm.teams = [];
        vm.permission = 0;
        vm.filter = {
            query: '',
            teamIds: '',
            statusId: ''
        };
        vm.eoqNominations = [];
        vm.nominationStatus = [
            {
                Value: 1,
                Name: 'Nominated'
            },
            {
                Value: 2,
                Name: 'Shorted List'
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

            EOQModel.getCurrentEOQByNominatorId().then(function () {
                vm.period = EOQModel.period;

                if (vm.period) {
                    vm.query();
                }
            });

            EOQModel.getUserPermissionOnNomination().then(function () {
                vm.permission = EOQModel.permission;
            });
        }

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, 'DepartmentId');

            EOQModel.getEOQNominationByNominatorId(
                vm.period.Period.EOQYId,
                vm.period.RoleId,
                vm.filter.query,
                vm.filter.teamIds,
                vm.filter.statusId).then(function () {
                    vm.eoqNominations = EOQModel.eoqNominations;
                })['finally'](function () {
                    lockScreenService.stop();
                });
        };

        vm.addNominee = function () {
            EOQModel.openPopupAddNominee(vm.period).then(function (nominee) {
                vm.modalInstance = null;

                EOQModel.createEOQNomination(vm.period.Period.EOQYId, nominee.employeeId, nominee.eoqQuestions).then(function () {
                    vm.query();
                });
            });
        };

        vm.deleteNomination = function (nomination) {
            EOQModel.openPopupDeleteEOQNomination(vm, nomination);
        };

        vm.viewNomination = function (nomination) {
            EOQModel.openPopupViewNomination(nomination.NominationId).then(function () {
                vm.modalInstance = null;
            });
        };

        vm.updateNomination = function (nomination) {
            EOQModel.openPopupUpdateNomination(nomination.NominationId).then(function (nominee) {
                vm.modalInstance = null;

                EOQModel.updateEOQNomination(nominee.nominationId, nominee.eoqQuestions).then(function () {
                    vm.query();
                });
            });
        };

        vm.updateStatusNomination = function (nomination) {
            lockScreenService.start();

            EOQModel.updateStatusEOQNomination(nomination.Period.EOQYId, nomination.Nominee.EmployeeId, nomination.StatusId)['finally'](function () {
                lockScreenService.stop();
            });
        };

        vm.openPopupInformation = function () {
            EOQModel.openPopupInformation();
        };

        vm.exportExcel = function () {
            vm.exportCriteria = {
                EOQYId: vm.period.Period.EOQYId,
                RoleId: vm.period.RoleId,
                Query: vm.filter.query,
                GroupIds: vm.filter.teamIds,
                StatusId: vm.filter.statusId,
                PeriodName: 'Employee of the ' + vm.period.Period.QuarterToString + ' ' + vm.period.Period.Year
            };

            $scope.$broadcast('exportEmployeeOfTheQuarter');
        };

        initialize();
    }
})(angular.module('assessmentModule'));