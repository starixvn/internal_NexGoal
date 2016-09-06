(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ManageNominationController", manageNominationController);

    manageNominationController.$inject = ["NominationModel", "utilitiesFactory"];

    function manageNominationController(NominationModel, utilitiesFactory) {
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
            alert("Implement Later");
        };

        vm.createNominationPeriod = function () {
            NominationModel.openPopupCreateNominationPeriod().then(function () { 
                vm.modalInstance = null;
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