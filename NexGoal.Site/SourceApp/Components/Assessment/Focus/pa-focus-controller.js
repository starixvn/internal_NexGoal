(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory", 'enumerationsFactory'];

    function paFocusController(CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory, enumerationsFactory) {
        var vm = this;
        vm.objectivesAchievementRanges = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
        vm.promotionsRanges = [
            {
                Value: true,
                Name: "Yes"
            },
            {
                Value: false,
                Name: "No"
            }
        ];
        vm.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null,
            employeeId: null
        };
        vm.searchEmployee = "";
        vm.paFocusPermission = {};

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

        function unGroupEvaluationByTeam(teams) {
            var evaluations = [];

            angular.forEach(teams, function (team) {
                angular.forEach(team.Members, function (member) {
                    evaluations.push(member);
                });
            });

            return evaluations;
        }

        function initialize() {
            PAFocusModel.getPAFocusFullPermission().then(function () {
                vm.paFocusPermission = PAFocusModel.PAFocusPermission;
                CommonModel.getAllPeriodIncludeImport().then(function () {
                    vm.periods = CommonModel.periodOptions;

                    if (vm.periods.length > 0) {
                        vm.filter.periodId = vm.periods[0].PeriodId;
                    }
                });

                CommonModel.getDepartment(null, true).then(function () {
                    vm.teams = utilitiesFactory.groupingTeams(CommonModel.departments);

                    utilitiesFactory.setSelectAll(vm.teams);

                    vm.query();
                });
            });
        }

        function calculateTeamTotalBonus(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    group.TotalBonus = 0;

                    angular.forEach(group.Members, function (member) {
                        group.TotalBonus += member.BonusGiven;
                    });
                }
            });
        }

        function calculateBonusRanking(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    var members = angular.copy(group.Members);

                    members.sort(function (memberA, memberB) {
                        return memberB.BonusGiven - memberA.BonusGiven;
                    });

                    angular.forEach(members, function (member, index) {
                        var previousMember = members[index - 1];

                        if (previousMember) {
                            member.BonusRanking = previousMember.BonusGiven === member.BonusGiven ?
                                previousMember.BonusRanking :
                                previousMember.BonusRanking + 1;
                        } else {
                            member.BonusRanking = index + 1;
                        }
                    });

                    angular.forEach(group.Members, function (member) {
                        var rankingMemberIndex = members.map(function (item) { return item.Employee.EmployeeId; }).indexOf(member.Employee.EmployeeId);

                        member.BonusRanking = members[rankingMemberIndex].BonusRanking;
                    });
                }
            });
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employeeId) {
                    vm.filter.employeeId = null;
                }
            }
        };

        vm.openPopupObjective = function (member) {
            PAFocusModel.openPopupObjective(member).then(function (NextDescriptionObjective) {
                member.NextDescriptionObjective = NextDescriptionObjective;
            });
        };

        vm.inputBonus = function (member) {
            if (member.BonusGiven === "" || member.BonusGiven < 0) {
                member.BonusGiven = 0;
            }

            calculateTeamTotalBonus(member.Employee.Group.GroupId);
            calculateBonusRanking(member.Employee.Group.GroupId);
        };

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            if (vm.paFocusPermission === enumerationsFactory.policy.None) {
                PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                    var evaluations = PAFocusModel.evaluations;
                    vm.groups = groupEvaluationByTeam(evaluations);
                });
            } else {
                PAFocusModel.getPAFocusByManagement(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                    var evaluations = PAFocusModel.evaluations;
                    vm.groups = groupEvaluationByTeam(evaluations);
                });
            }
        };

        vm.save = function () {
            if (PAFocusModel.validatePAFocusForm(vm.groups)) {
                var evaluations = unGroupEvaluationByTeam(vm.groups);
                if (vm.paFocusPermission === enumerationsFactory.policy.None) {
                    PAFocusModel.updatePAFocus(evaluations);
                } else {
                    PAFocusModel.updatePAFocusByManagement(evaluations);
                }
            }
        };

        initialize();
    }
})(angular.module("assessmentModule"));