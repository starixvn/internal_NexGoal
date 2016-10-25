(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ManageEOQController', manageEOQController);

    manageEOQController.$inject = ['$scope', '$location', 'CommonModel', 'EOQModel', 'utilitiesFactory',
        'historyFactory', 'enumerationsFactory', 'lockScreenService', 'baseUrl'];

    function manageEOQController($scope, $location, CommonModel, EOQModel, utilitiesFactory,
        historyFactory, enumerationsFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periods = [];
        vm.filteredItems = [];
        vm.years = [];
        vm.filter = {};
        vm.currentPage = 1;
        vm.searchKey = '';
        vm.modalInstance = null;
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;
        vm.isFirstLoad = true;
        vm.currentYear = new Date().getFullYear();

        function initFilter() {
            var filter = historyFactory.getState();

            if (!filter) {
                filter = {};
                filter.Year = vm.years.length > 0 ? vm.years[0] : vm.currentYear;
                filter.StatusId = 1; // In Progresss
            }

            return filter;
        }

        function initialize() {

            EOQModel.getYearEOQ().then(function () {
                vm.years = EOQModel.years;
                utilitiesFactory.numberEntryList(vm);
                utilitiesFactory.getDateFormat(vm);
                vm.filter = initFilter();

                vm.query();
            });
        }

        $scope.$watch(function () { return $scope.manageNominationCtrl.dataTab.eoq; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();

                vm.permission = $scope.manageNominationCtrl.permission;
                vm.isFirstLoad = false;

                if ($location.url() !== $location.path()) {
                    $location.url($location.path());
                }
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

            EOQModel.getEOQ(vm.filter.Year, vm.filter.StatusId).then(function () {
                vm.periods = EOQModel.periods;
                utilitiesFactory.searchOnPage(vm, vm.periods);
            })['finally'](function () {
                lockScreenService.stop();
            });

        };

        vm.createEOQ = function () {
            EOQModel.openPopupCreateEOQ().then(function () {
                vm.query();
            });
        };

        vm.updateEOQ = function (eoqyId) {
            $location.path(baseUrl + 'ManageNomination/UpdateEOQ/' + eoqyId);
        };

        vm.viewEOQDetail = function (eoqyId) {
            $location.path(baseUrl + 'ManageNomination/ViewEOQDetail/' + eoqyId);
        };

        vm.deleteEOQ = function (period) {
            EOQModel.openPopupDeleteEOQ(vm, period);
        };
    }
})(angular.module('assessmentModule'));