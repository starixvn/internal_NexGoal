(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYRolesManagementController', updateEOYRolesManagementController);

    updateEOYRolesManagementController.$inject = ['$scope', 'EOYModel', 'CommonModel', 'utilitiesFactory',
        'enumerationsFactory', 'lockScreenService'];

    function updateEOYRolesManagementController($scope, EOYModel, CommonModel, utilitiesFactory,
        enumerationsFactory, lockScreenService) {
        var vm = this;

        vm.COMMITTEE_ROLE = 2;
        vm.NOMINATOR_ROLE = 1;
        vm.period = {};
        vm.nominatorSettings = [];
        vm.searchKey = '';
        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.filter = {
            query: '',
            teamIds: '',
            roleId: '',
            showAllEmployee: false
        };
        vm.teams = [];
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;
        vm.isFirstLoad = true;

        function initialize() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);

                vm.query();
            });
        }

        $scope.$watch(function () {
            return $scope.updateEOYCtrl.dataTab.rolesManagement;
        }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
            }
        });

        $scope.$watch(function () {
            return $scope.updateEOYCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOYCtrl.period;
            }
        });

        $scope.$watch(function () {
            return $scope.updateEOYCtrl.permission;
        }, function (newVal) {
            if (newVal) {
                vm.permission = newVal;
            }
        });

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, 'DepartmentId');

            EOYModel.getEOYNominatorSettingById(
                vm.period.EOQYId,
                vm.filter.query,
                vm.filter.teamIds,
                vm.filter.roleId,
                vm.filter.showAllEmployee).then(function () {
                    vm.nominatorSettings = EOYModel.nominatorSettings;

                    vm.searchKey = '';
                    vm.currentPage = 1;

                    utilitiesFactory.searchOnPage(vm, vm.nominatorSettings);
                })['finally'](function () {
                    lockScreenService.stop();
                });
        };

        vm.modifyRole = function (nominator, role) {
            if (role === vm.NOMINATOR_ROLE) {
                if (nominator.IsNominator) {
                    nominator.Roles.push(vm.NOMINATOR_ROLE);
                } else {
                    nominator.Roles.splice(nominator.Roles.indexOf(vm.NOMINATOR_ROLE), 1);
                }
            } else {
                if (nominator.IsCommittee) {
                    nominator.Roles.push(vm.COMMITTEE_ROLE);
                } else {
                    nominator.Roles.splice(nominator.Roles.indexOf(vm.COMMITTEE_ROLE), 1);
                }
            }

            nominator.RoleIds = nominator.Roles.join(',');

            lockScreenService.start();

            var nominators = [];

            nominators.push(nominator);

            EOYModel.updateEOYNominatorSettingById(vm.period.EOQYId, nominators)['finally'](function () {
                lockScreenService.stop();
            });
        };
    }
})(angular.module('assessmentModule'));