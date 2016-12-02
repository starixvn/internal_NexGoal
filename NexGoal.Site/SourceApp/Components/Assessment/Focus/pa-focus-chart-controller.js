(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusChartController", paFocusChartController);

    paFocusChartController.$inject = ["CommonModel", "PAFocusModel", "utilitiesFactory", "lockScreenService", "enumerationsFactory"];

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
        var step = 0;
        var points = [];
        var position = {
            None: 0,
            Left: 1,
            Bottom: 2,
            Center: 3,
            Over: 4
        };

        function checkExistPoint(x, y) {
            var isExisted = false;
            angular.forEach(points, function (point) {
                if (point.x == x && point.y == y) {
                    isExisted = true;
                }
            });

            return isExisted;
        }


        function findNumberExist(point) {
            var number = 0;
            angular.forEach(vm.groups, function (group) {
                angular.forEach(group.Members, function (member) {
                    if (point.BonusGiven === member.BonusGiven
                        && point.Contribution === member.Contribution) {
                        number++;
                    }
                });
            });

            return number;
        }

        function calculateThreshold(number) {
            return 1 / number;
        }

        function FindUniquePoint(member) {
            var isExisted = true;
            var number = findNumberExist(member);
            var threshold = calculateThreshold(number);
            var bonus = member.BonusGiven;
            var contribution = member.Contribution;

            while (isExisted) {
                isExisted = checkExistPoint(bonus, contribution);
                if (isExisted) {
                    if (step == position.Left) {
                        bonus -= threshold;
                    } else if (step == position.Bottom) {
                        contribution -= threshold;
                    } else if (step == position.Center || step == position.Over) {
                        bonus -= threshold;
                        contribution -= threshold;
                    }
                    step++;
                    if (step > position.Over) {
                        step = position.None;
                    }
                } else {
                    points.push({ x: bonus, y: contribution });
                }
            }

            return {
                x: bonus,
                y: contribution
            };
        }

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

        function convertToYesNo(value) {
            var text = 'No';
            if (value === true || value === 'true') {
                text = 'Yes';
            }

            return text;
        }

        function getPeriodName(index) {
            var name = '';
            if (vm.periods.length > 0) {
                angular.forEach(vm.periods, function (period) {
                    if (period.PeriodId === index) {
                        name = period.Name;
                    }
                });
            }

            return name;
        }

        function buildReadyPromotion(value) {
            var text = convertToYesNo(value);
            var html = text;
            if (value === true || value === 'true') {
                html = '<span style="color:#F0931F">' + text + '</span>';
            }

            return html;
        }

        function buildPoorPeformance(value) {
            var text = convertToYesNo(value);
            var html = text;
            if (value === true || value === 'true') {
                html = '<span style="color:red">' + text + '</span>';
            }

            return html;
        }

        vm.buildChart = function (groups) {
            var chartData = [];
            angular.forEach(groups, function (group) {
                if (!group.isBuilt) {
                    group.isBuilt = true;
                    angular.forEach(group.Members, function (member) {
                        if (member.BonusGiven !== null) {
                            var color = '#56BDE4';
                            if (member.IsReadyPromotion) {
                                color = '#F0931F';
                            }
                            if (member.IsPoorPerformance) {
                                color = 'red';
                            }
                            var contribution = utilitiesFactory.getFocusEvaluationLevel(member.Contribution);
                            var bonus = utilitiesFactory.getFocusEvaluationLevel(member.BonusGiven);

                            var description = '<span style="color:red">(*)</span>: ' + contribution.Name
                            + '<br> <span style="color:red">(**)</span>: ' + bonus.Name
                            + '<br> Ready for Promotion: ' + buildReadyPromotion(member.IsReadyPromotion)
                            + '<br> Poor Performance: ' + buildPoorPeformance(member.IsPoorPerformance)
                            + '<br> Job title: ' + member.Employee.JobTitle.JobTitleName

                            var point = FindUniquePoint(member);

                            var piece = {
                                name: member.Employee.NickName,
                                color: color,
                                y: point.y,
                                x: point.x,
                                description: description
                            };

                            chartData.push(piece);
                        }
                    });
                }
            });


            $('#pafocus-chart').highcharts({
                chart: {
                    type: 'scatter',
                    zoomType: 'xy',
                    height: 600
                },
                title: {
                    text: 'PA Focus of ' + getPeriodName(vm.filter.periodId)
                },
                export: {
                    enabled: true
                },
                yAxis: {
                    title: { text: ' I would always want him or her on my team (ability to work well with others) <span style="color:red">(*)</span></b>' },
                    tickInterval: 1,
                    min: 0,
                    max: 5,
                    gridLineWidth: 0,
                    lineWidth: 5,
                    tickWidth: 1
                },
                legend: {
                    enabled: false
                },
                xAxis: {
                    title: {
                        text: 'I would want to award this person the highest possible benefit (increase and bonus)<br> which is available for her/his job grade. <span style="color:red">(**)</span>'
                    },
                    tickInterval: 1,
                    lineWidth: 5,
                    min: 0,
                    max: 5
                },
                plotOptions: {
                    scatter: {
                        marker: {
                            radius: 4
                        }
                    }
                },
                tooltip: {
                    headerFormat: '',
                    pointFormat: '<b>{point.name}</b> <br>{point.description}'
                },
                series: [{ data: chartData }]
            });
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                vm.groups = PAFocusModel.evaluations;
                vm.isEmptyResult = vm.groups.length === 0;
                points = [];
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));