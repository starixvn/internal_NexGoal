(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("CreateNominationPeriodController", createNominationPeriodController);

    createNominationPeriodController.$inject = ["$modalInstance", "NominationModel"];

    function createNominationPeriodController($modalInstance, NominationModel) {
        var vm = this;

        vm.years = [];
        vm.rawStartDate = null;
        vm.rawEndDate = null;
        vm.period = {
            Quarter: null,
            Year: null,
            StartDate: null,
            RawStartDate: null,
            EndDate: null,
            RawEndDate: null,
            Description: null
        };
        vm.calendar = {};

        vm.openPopupStartDate = function (event) {
            NominationModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            NominationModel.openPopupEndDate(event);
        };

        vm.submit = function () {
            if (NominationModel.validatePeriod(vm.period)) {
                vm.period.StartDate = vm.period.RawStartDate ? vm.period.RawStartDate.toDateString() : null;
                vm.period.EndDate = vm.period.RawEndDate ? vm.period.RawEndDate.toDateString() : null;

                alert("Proceed Save");

                $modalInstance.dismiss("cancel");
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        function initialize() {
            var date = new Date();
            var year = date.getFullYear();

            for (var index = year; index > (year - 5) ; index--) {
                vm.years.push(index);
            }

            vm.calendar = NominationModel.calendar;
        }

        initialize();
    }
})(angular.module("assessmentModule"));