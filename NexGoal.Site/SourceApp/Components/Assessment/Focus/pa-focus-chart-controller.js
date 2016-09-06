(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusChartController", paFocusChartController);

    paFocusChartController.$inject = ["CommonModel", "PAFocusModel", "utilitiesFactory", "enumerationsFactory"];

    function paFocusChartController(CommonModel, PAFocusModel, utilitiesFactory, enumerationsFactory) {
        var vm = this;

        vm.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null
        };
        vm.paFocusPermission = {};

        function initialize() {
            PAFocusModel.getPAFocusFullPermission().then(function () {
                vm.paFocusPermission = PAFocusModel.PAFocusPermission;
                CommonModel.getAllPeriodIncludeImport().then(function () {
                    vm.periods = CommonModel.periodOptions;

                    if (vm.periods.length > 0) {
                        vm.filter.periodId = vm.periods[0].PeriodId;
                    }

                    CommonModel.getDepartment(null, true).then(function () {
                        var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                        vm.teams = angular.copy(treeDepartment);
                        utilitiesFactory.setSelectAll(vm.teams);
                        vm.query();
                    });
                });
            });
        }

        vm.buildChart = function (group) {
            if (!group.isBuilt) {
                group.isBuilt = true;
                var chartData = [];

                angular.forEach(group.Members, function (member) {
                    var piece = {
                        name: member.Employee.NickName,
                        contribution: member.Contribution,
                        y: member.BonusGiven
                    };

                    chartData.push(piece);
                });

                $('#pie-chart-group-' + group.TeamId).highcharts({
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie'
                    },
                    title: {
                        text: group.Name
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: true,
                                format: '<div style="color:{point.color}">{point.name} [{point.contribution}]<br/> {point.percentage:.1f} %</div>',
                                useHTML: true
                            }
                        }
                    },
                    series: [{
                        name: 'Bonus Given',
                        colorByPoint: true,
                        data: chartData
                    }]
                });
            }
        };

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            if (vm.paFocusPermission === enumerationsFactory.policy.None) {
                PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                    vm.groups = PAFocusModel.evaluations;
                });
            } else {
                PAFocusModel.getPAFocusByManagement(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                    vm.groups = PAFocusModel.evaluations;
                });
            }
        };

        //initialize();
    }
})(angular.module("assessmentModule"));