(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ManageNominationController", manageNominationController);

    manageNominationController.$inject = ["NominationModel", "utilitiesFactory", "lockScreenService"];

    function manageNominationController(NominationModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.periods = [];
        vm.filteredItems = [];

        vm.years = [];

        vm.filter = {
            Year: null,
            Status: 2 // In Progress
        };

        vm.currentPage = 1;
        vm.pageSize = 20;

        vm.searchKey = "";

        vm.modalInstance = null;

        vm.query = function () {
            //alert("Implement Later");
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

        function initialize() {
            var date = new Date();
            var year = date.getFullYear();

            for (var index = year; index > (year - 5) ; index--) {
                vm.years.push(index);
            }

            vm.filter.Year = year;

            utilitiesFactory.numberEntryList(vm);
        }

        initialize();
    }
})(angular.module("assessmentModule"));