(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory"];

    function paFocusController(CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory) {
        var vm = this;
        var oldGroupsData = [];
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

        function isReadOnly(member) {
            var now = new Date();
            if (now < member.StartDate || now > member.EndDate) {
                return true;
            }

            return false;
        }

        function groupEvaluationByTeam(evaluations) {
            var teams = [];

            angular.forEach(evaluations, function (evaluation) {
                var groupName = evaluation.Employee.Group.GroupName;
                var groupId = evaluation.Employee.Group.GroupId;
                if (teams.length === 0) {
                    teams.push({ teamId: groupId, name: groupName, MaxBonus: 100, TotalBonus: 0, members: [] });
                } else {
                    angular.forEach(teams, function (team) {
                        if (team.name !== groupName) {
                            teams.push({ name: groupName, members: [] });
                        }
                    });
                }
            });

            angular.forEach(teams, function (team) {
                angular.forEach(evaluations, function (evaluation) {
                    var groupName = evaluation.Employee.Group.GroupName;
                    if (groupName === team.name) {
                        team.members.push(evaluation);
                        team.TotalBonus += evaluation.BonusGiven;
                    }
                });
            });

            return teams;
        }

        function unGroupEvaluationByTeam(teams) {
            var evaluations = [];

            angular.forEach(teams, function (team) {
                angular.forEach(team.members, function (member) {
                    evaluations.push(member);
                });
            });

            return evaluations;
        }

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

                vm.query();
            });
        }

        function calculateTeamTotalBonus(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    group.TotalBonus = 0;

                    angular.forEach(group.members, function (member) {
                        group.TotalBonus += member.BonusGiven;
                    });
                }
            });
        }

        function calculateBonusRanking(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    var members = angular.copy(group.members);

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

                    angular.forEach(group.members, function (member) {
                        var rankingMemberIndex = members.map(function (item) { return item.EmployeeId; }).indexOf(member.EmployeeId);

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

            calculateTeamTotalBonus(member.TeamId);
            calculateBonusRanking(member.TeamId);
        };

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');

            console.log(vm.filter);

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                var evaluations = PAFocusModel.evaluations;
                vm.groups = groupEvaluationByTeam(evaluations);
            });
        };

        vm.save = function () {
            if (PAFocusModel.validatePAFocusForm(vm.groups)) {
                var evaluations = unGroupEvaluationByTeam(vm.groups);
                PAFocusModel.updatePAFocus(evaluations);
            }
        };

        vm.isReadOnly = isReadOnly;

        initialize();
    }
})(angular.module("assessmentModule"));