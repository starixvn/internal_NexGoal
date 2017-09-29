(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('CreateEOQController', createEOQController);

    createEOQController.$inject = ["$scope", "$modalInstance", "EOQModel"];

    function createEOQController($scope, $modalInstance, EOQModel) {
        var vm = this;

        vm.years = [];
        vm.rawStartDate = null;
        vm.rawEndDate = null;
        vm.rawRemindDate = null;
        vm.period = {
            Quarter: "",
            Year: "",
            StartDate: null,
            RawStartDate: null,
            EndDate: null,
            RawEndDate: null,
            RemindDate: null,
            RawRemindDate: null,
            EndDateForCommittee: null,
            RawEndDateForCommittee: null,
            Description: null
        };
        vm.calendar = {};

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

        vm.submit = function () {
            if (EOQModel.validatePeriod(vm.period)) {
                vm.period.StartDate = vm.period.RawStartDate ? vm.period.RawStartDate.toDateString() : null;
                vm.period.EndDate = vm.period.RawEndDate ? vm.period.RawEndDate.toDateString() : null;
                vm.period.RemindDate = vm.period.RawRemindDate ? vm.period.RawRemindDate.toDateString() : null;
                vm.period.EndDateForCommittee = vm.period.RawEndDateForCommittee ? vm.period.RawEndDateForCommittee.toDateString() : null;

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

            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        initialize();
    }
})(angular.module('assessmentModule'));