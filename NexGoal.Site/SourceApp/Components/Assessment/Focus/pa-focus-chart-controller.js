(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusChartController", paFocusChartController);

    paFocusChartController.$inject = ["CommonModel", "PAFocusModel", "utilitiesFactory"];

    function paFocusChartController(CommonModel, PAFocusModel, utilitiesFactory) {
        var vm = this;

        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null
        };

        function initialize() {
            CommonModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = CommonModel.periodOptions;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });

            CommonModel.getDepartment(null, true).then(function () {
                vm.teams = utilitiesFactory.groupingTeams(CommonModel.departments);

                utilitiesFactory.setSelectAll(vm.teams);
            });
        }

        initialize();
    }
})(angular.module("assessmentModule"));