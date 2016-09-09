(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusChartController", paFocusChartController);

    paFocusChartController.$inject = ["CommonModel", "PAFocusModel", "utilitiesFactory", "lockScreenService","enumerationsFactory"];

    function paFocusChartController(CommonModel, PAFocusModel, utilitiesFactory, lockScreenService, enumerationsFactory) {
        var vm = this;

        vm.isEmptyResult = false;
        vm.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: 0,
            teamIds: null
        };

        function initialize() {
            CommonModel.getPeriod(enumerationsFactory.targetType.SemiAnnual).then(function () {
                vm.periods = CommonModel.periods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });
        }

        vm.buildChart = function (group) {
            if (!group.isBuilt) {
                group.isBuilt = true;
                var chartData = [];

                angular.forEach(group.Members, function (member) {
                    if (member.BonusGiven !== null) {
                        var piece = {
                            name: member.Employee.NickName,
                            contribution: member.Contribution === null ? '' : '[' + member.Contribution + ']',
                            bonus: member.BonusGiven + '%',
                            y: member.BonusGiven,
                        };

                        chartData.push(piece);
                    }
                });
                if (group.TotalBonus < group.MaxBonus) {
                    var emptyPiece = {
                        name: 'OutStanding',
                        contribution: '',
                        bonus: '',
                        y: group.MaxBonus - group.TotalBonus
                    };
                    chartData.push(emptyPiece);
                }

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
                                format: '<div style="color:{point.color}">{point.name} {point.contribution}<br/> {point.bonus}</div>',
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
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                vm.groups = PAFocusModel.evaluations;
                vm.isEmptyResult = vm.groups.length == 0;
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));