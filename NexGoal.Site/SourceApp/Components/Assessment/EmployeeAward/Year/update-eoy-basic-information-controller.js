(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYBasicInformationController', updateEOYBasicInformationController);

    updateEOYBasicInformationController.$inject = ['$scope', '$location', 'EOYModel', 'historyFactory', 'enumerationsFactory', 'baseUrl'];

    function updateEOYBasicInformationController($scope, $location, EOYModel, historyFactory, enumerationsFactory, baseUrl) {
        var vm = this;

        vm.years = [];
        vm.period = {};
        vm.calendar = EOYModel.calendar;
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;

        function initialize() {
            var date = new Date();
            var year = date.getFullYear();

            for (var index = year; index <= year + 1 ; index++) {
                vm.years.push(index);
            }
        }

        vm.openPopupStartDate = function (event) {
            EOYModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOYModel.openPopupEndDate(event);
        };

        vm.openPopupRemindDate = function (event) {
            EOYModel.openPopupRemindDate(event);
        };

        $scope.$watch(function () {
            return $scope.updateEOYCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOYCtrl.period;
                vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
                vm.period.RawRemindDate = vm.period.RemindDate ? new Date(vm.period.RemindDate) : null;
            }
        });

        $scope.$watch(function () {
            return $scope.updateEOYCtrl.permission;
        }, function (newVal) {
            if (newVal) {
                vm.permission = $scope.updateEOYCtrl.permission;
            }
        });

        vm.save = function (period) {
            if (EOYModel.validatePeriod(period)) {
                period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
                period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
                period.RemindDate = period.RawRemindDate ? period.RawRemindDate.toDateString() : null;

                EOYModel.updateEOY(period);
            }
        };

        vm.cancel = function () {
            historyFactory.setBackStatus(true);
            $location.path(baseUrl + 'ManageNomination/Index').search({ secondTab: true });
        };

        initialize();
    }
})(angular.module('assessmentModule'));