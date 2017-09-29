(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOQBasicInformationController', updateEOQBasicInformationController);

    updateEOQBasicInformationController.$inject = ['$scope', '$route', 'EOQModel', 'historyFactory', 'enumerationsFactory'];

    function updateEOQBasicInformationController($scope, $route, EOQModel, historyFactory, enumerationsFactory) {
        var vm = this;

        vm.PERIOD_CLOSED_STATUS = 2;
        vm.years = [];
        vm.period = {};
        vm.calendar = EOQModel.calendar;
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
            EOQModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOQModel.openPopupEndDate(event);
        };

        vm.openPopupRemindDate = function (event) {
            EOQModel.openPopupRemindDate(event);
        };

        vm.openPopupEndDateForCommittee = function (event) {
            EOQModel.openPopupEndDateForCommittee(event);
        };

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOQCtrl.period;
                vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
                vm.period.RawRemindDate = vm.period.RemindDate ? new Date(vm.period.RemindDate) : null;
                vm.period.RawEndDateForCommittee = vm.period.EndDateForCommittee ? new Date(vm.period.EndDateForCommittee) : null;
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
                period.RemindDate = period.RawRemindDate ? period.RawRemindDate.toDateString() : null;
                period.EndDateForCommittee = period.RawEndDateForCommittee ? period.RawEndDateForCommittee.toDateString() : null;

                EOQModel.updateEOQ(period).then(function () {
                    $route.reload();
                });
            }
        };

        initialize();
    }
})(angular.module('assessmentModule'));