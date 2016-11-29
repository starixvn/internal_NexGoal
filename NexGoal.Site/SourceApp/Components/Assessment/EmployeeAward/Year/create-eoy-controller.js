(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('CreateEOYController', createEOQController);

    createEOQController.$inject = ['$scope', '$modalInstance', 'EOYModel'];

    function createEOQController($scope, $modalInstance, EOYModel) {
        var vm = this;

        vm.years = [];
        vm.rawStartDate = null;
        vm.rawEndDate = null;
        vm.rawRemindDate = null,

        vm.period = {
            Year: "",
            StartDate: null,
            RawStartDate: null,
            EndDate: null,
            RawEndDate: null,
            RemindDate: null,
            rawRemindDate: null, 
            Description: null
        };
        vm.calendar = {};

        vm.openPopupStartDate = function (event) {
            EOYModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOYModel.openPopupEndDate(event);
        };
        vm.openPopupRemindDate = function (event) {
            EOYModel.openPopupRemindDate(event);
        };
        vm.submit = function () {
            if (EOYModel.validatePeriod(vm.period)) {
                vm.period.StartDate = vm.period.RawStartDate ? vm.period.RawStartDate.toDateString() : null;
                vm.period.EndDate = vm.period.RawEndDate ? vm.period.RawEndDate.toDateString() : null;
                vm.period.RemindDate = vm.period.RawEndDate ? vm.period.RawRemindDate.toDateString() : null;

                EOYModel.createEOY(vm.period, $modalInstance);
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

            vm.calendar = EOYModel.calendar;

            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        initialize();
    }
})(angular.module('assessmentModule'));