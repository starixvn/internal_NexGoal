(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ManageEOQController", manageEOQController);

    manageEOQController.$inject = ["$scope", "$location", "CommonModel", "EOQModel", "utilitiesFactory",
        "historyFactory", "enumerationsFactory", "lockScreenService", "baseUrl"];

    function manageEOQController($scope, $location, CommonModel, EOQModel, utilitiesFactory,
        historyFactory, enumerationsFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periods = [];
        vm.filteredItems = [];
        vm.years = [];
        vm.filter = {};
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.searchKey = "";
        vm.modalInstance = null;
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;
        vm.isFirstLoad = true;

        function initFilter() {
            var filter = historyFactory.getState();

            if (!filter) {
                var date = new Date();
                var year = date.getFullYear();

                filter = {};
                filter.Year = year;
                filter.StatusId = 1; // In Progresss
            }

            return filter;
        }

        function initialize() {
            var date = new Date();
            var year = date.getFullYear();

            for (var index = year; index > (year - 5) ; index--) {
                vm.years.push(index);
            }

            utilitiesFactory.numberEntryList(vm);
            utilitiesFactory.getDateFormat(vm);
            vm.filter = initFilter();

            vm.query();
        }

        $scope.$watch(function () { return $scope.manageNominationCtrl.dataTab.eoq; },
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

            EOQModel.getEOQ(vm.filter.Year, vm.filter.StatusId).then(function () {
                vm.periods = EOQModel.periods;

                angular.forEach(vm.periods, function (period) {
                    period.QuarterName = EOQModel.getQuarterName(period.Quarter);
                    period.StatusName = EOQModel.getStatusName(period.StatusId);
                });

                utilitiesFactory.searchOnPage(vm, vm.periods);
            })["finally"](function () {
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        };

        vm.createEOQ = function () {
            EOQModel.openPopupCreateEOQ().then(function (period) {
                vm.modalInstance = null;

                lockScreenService.start();

                EOQModel.createEOQ(period).then(function () {
                    vm.query();
                })["finally"](function () {
                    lockScreenService.stop();
                });
            });
        };

        vm.updateEOQ = function (eoqyId) {
            $location.path(baseUrl + "ManageNomination/UpdateEOQ/" + eoqyId);
        };

        vm.deleteEOQ = function (period) {
            EOQModel.openPopupDeleteEOQ(vm, period);
        };
    }
})(angular.module("assessmentModule"));