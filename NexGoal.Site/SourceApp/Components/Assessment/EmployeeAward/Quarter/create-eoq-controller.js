(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('CreateEOQController', createEOQController);

    createEOQController.$inject = ['$modalInstance', 'EOQModel'];

    function createEOQController($modalInstance, EOQModel) {
        var vm = this;

        vm.years = [];
        vm.rawStartDate = null;
        vm.rawEndDate = null;
        vm.period = {
            Quarter: "",
            Year: "",
            StartDate: null,
            RawStartDate: null,
            EndDate: null,
            RawEndDate: null,
            Description: null
        };
        vm.calendar = {};

        vm.openPopupStartDate = function (event) {
            EOQModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOQModel.openPopupEndDate(event);
        };

        vm.submit = function () {
            if (EOQModel.validatePeriod(vm.period)) {
                vm.period.StartDate = vm.period.RawStartDate ? vm.period.RawStartDate.toDateString() : null;
                vm.period.EndDate = vm.period.RawEndDate ? vm.period.RawEndDate.toDateString() : null;

                EOQModel.createEOQ(vm.period, $modalInstance);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        function initialize() {
            var date = new Date();
            var year = date.getFullYear();

            for (var index = year; index <= year + 1 ; index++) {
                vm.years.push(index);
            }

            vm.calendar = EOQModel.calendar;
        }

        initialize();
    }
})(angular.module('assessmentModule'));