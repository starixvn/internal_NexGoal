(function (periodModule) {
    "use strict";

    periodModule.controller("FocusSettingController", focusSettingController);

    focusSettingController.$inject = ["$scope"];

    function focusSettingController($scope) {
        var vm = this;

        vm.period = {
            StartDate: null,
            EndDate: null,
            RawStartDate: null,
            RawEndDate: null,
            EvaluatorDuration: 0,
            ReviewerEvaluation: 0,
            ManagementDuration: 0
        };

        return vm;
    }
})(angular.module("periodModule"));