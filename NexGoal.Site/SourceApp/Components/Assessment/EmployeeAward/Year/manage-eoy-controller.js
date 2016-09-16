(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ManageEOYController', manageEOYController);

    manageEOYController.$inject = ['$scope', '$location', 'CommonModel', 'EOYModel', 'utilitiesFactory',
        'historyFactory', 'enumerationsFactory', 'lockScreenService', 'baseUrl'];

    function manageEOYController($scope, $location, CommonModel, EOYModel, utilitiesFactory,
        historyFactory, enumerationsFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periods = [];
        vm.filteredItems = [];
        vm.filter = {};
        vm.currentPage = 1;
        vm.pageSize = 0;
        vm.searchKey = '';
        vm.modalInstance = null;
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;
        vm.isFirstLoad = true;

        function initFilter() {
            var filter = historyFactory.getState();

            if (!filter) {
                filter = {};
                filter.StatusId = 1; // In Progresss
            }

            return filter;
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);
            utilitiesFactory.getDateFormat(vm);
            vm.filter = initFilter();
            vm.pageSize = 20;

            vm.query();
        }

        $scope.$watch(function () { return $scope.manageNominationCtrl.dataTab.eoy; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();

                vm.permission = $scope.manageNominationCtrl.permission;
                vm.isFirstLoad = false;
            }
        });

        $scope.$watch(function () { return $scope.manageNominationCtrl.permission; },
        function (newVal) {
            if (newVal) {
                vm.permission = newVal;
            }
        });

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.query = function () {
            lockScreenService.start();

            vm.searchKey = '';
            vm.currentPage = 1;

            EOYModel.getEOY(vm.filter.Year, vm.filter.StatusId).then(function () {
                vm.periods = EOYModel.periods;

                utilitiesFactory.searchOnPage(vm, vm.periods);
            })['finally'](function () {
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        };

        vm.createEOY = function () {
            EOYModel.openPopupCreateEOY().then(function () {
                vm.modalInstance = null;
                vm.query();
            });
        };

        vm.updateEOY = function (eoqyId) {
            $location.path(baseUrl + 'ManageNomination/UpdateEOY/' + eoqyId);
        };

        vm.deleteEOY = function (period) {
            EOYModel.openPopupDeleteEOY(vm, period);
        };
    }
})(angular.module('assessmentModule'));