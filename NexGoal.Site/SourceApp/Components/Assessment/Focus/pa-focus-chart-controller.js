(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusChartController", paFocusChartController);

    paFocusChartController.$inject = ["CommonModel", "PAFocusModel", "utilitiesFactory"];

    function paFocusChartController(CommonModel, PAFocusModel, utilitiesFactory) {
        var vm = this;

        vm.groups = [];
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

        function groupEvaluationByTeam(evaluations) {
            var teams = [];

            angular.forEach(evaluations, function (evaluation) {
                var groupName = evaluation.Employee.Group.GroupName;
                var groupId = evaluation.Employee.Group.GroupId;
                if (teams.length === 0) {
                    teams.push({ TeamId: groupId, Name: groupName, MaxBonus: 100, TotalBonus: 0, Members: [] });
                } else {
                    angular.forEach(teams, function (team) {
                        if (team.Name !== groupName) {
                            teams.push({ Name: groupName, Members: [] });
                        }
                    });
                }
            });

            angular.forEach(teams, function (team) {
                angular.forEach(evaluations, function (evaluation) {
                    var groupName = evaluation.Employee.Group.GroupName;
                    if (groupName === team.Name) {
                        team.Members.push(evaluation);
                        team.TotalBonus += evaluation.BonusGiven;
                    }
                });
            });

            return teams;
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

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, null).then(function () {
                var evaluations = PAFocusModel.evaluations;
                vm.groups = groupEvaluationByTeam(evaluations);
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));