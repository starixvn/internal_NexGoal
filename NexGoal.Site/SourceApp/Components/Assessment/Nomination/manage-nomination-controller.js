(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ManageNominationController", manageNominationController);

    manageNominationController.$inject = ["CommonModel", "NominationModel", "utilitiesFactory", "historyFactory", "lockScreenService"];

    function manageNominationController(CommonModel, NominationModel, utilitiesFactory, historyFactory, lockScreenService) {
        var vm = this;

        vm.periods = [];
        vm.filteredItems = [];

        vm.years = [];

        vm.filter = {};

        vm.currentPage = 1;
        vm.pageSize = 20;

        vm.searchKey = "";

        vm.modalInstance = null;

        function initFilter() {
            var filter = historyFactory.getState();

            if (!filter) {
                var date = new Date();
                var year = date.getFullYear();

                filter = {};
                filter.Year = year;
                filter.StatusId = null; // In Progresss
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

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.getStatusName = function (statusId) {
            var statusName = "";

            switch (statusId) {
                case 0:
                    statusName = "Not Started";
                    break;
                case 1:
                    statusName = "In Progress";
                    break;
                case 2:
                    statusName = "Closed";
                    break;
            }

            return statusName;
        }

        vm.query = function () {
            lockScreenService.start();

            vm.searchKey = '';
            vm.currentPage = 1;

            NominationModel.getEmployeeOfQuarterPeriod(vm.filter.Year, vm.filter.StatusId).then(function () {
                vm.periods = NominationModel.periods;
                utilitiesFactory.searchOnPage(vm, vm.periods);
            })["finally"](function () {
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        };

        vm.createNominationPeriod = function () {
            NominationModel.openPopupCreateNominationPeriod().then(function (period) {
                vm.modalInstance = null;

                lockScreenService.start();

                NominationModel.createEmployeeOfQuarterPeriod(period).then(function () {
                    vm.query();
                })["finally"](function () {
                    lockScreenService.stop();
                });
            });
        };



        initialize();
    }
})(angular.module("assessmentModule"));