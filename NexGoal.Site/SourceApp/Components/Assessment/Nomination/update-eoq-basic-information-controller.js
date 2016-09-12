(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOQBasicInformationController', updateEOQBasicInformationController);

    updateEOQBasicInformationController.$inject = ['$scope', 'EOQModel', 'historyFactory', 'enumerationsFactory', 'lockScreenService'];

    function updateEOQBasicInformationController($scope, EOQModel, historyFactory, enumerationsFactory, lockScreenService) {
        var vm = this;

        vm.period = {};
        vm.calendar = EOQModel.calendar;
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;

        vm.openPopupStartDate = function (event) {
            EOQModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOQModel.openPopupEndDate(event);
        };

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOQCtrl.period;
                vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
            }
        });

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.permission;
        }, function (newVal) {
            if (newVal) {
                vm.permission = $scope.updateEOQCtrl.permission;
            }
        });

        vm.save = function (period) {
            if (EOQModel.validatePeriod(period)) {
                period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
                period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;

                EOQModel.updateEOQ(period);
            }
        };

        vm.cancel = function () {
            historyFactory.back();
        };
    }
})(angular.module('assessmentModule'));