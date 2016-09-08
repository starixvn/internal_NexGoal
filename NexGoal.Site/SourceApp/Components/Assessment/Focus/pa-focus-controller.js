(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory", 'lockScreenService'];

    function paFocusController(CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory, lockScreenService) {
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
            periodId: 0,
            teamIds: null,
            employeeId: null
        };
        vm.searchEmployee = "";

        function initialize() {
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
        }

        function calculateTeamTotalBonus(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    var total = 0;
                    angular.forEach(group.Members, function (member) {
                        if (member.BonusGiven) {
                            total += member.BonusGiven;
                        }
                    });

                    group.TotalBonus = parseFloat(total.toFixed(2));
                }
            });
        }
        function ranking(members) {
            var tempMembers = angular.copy(members);

            tempMembers.sort(function (memberA, memberB) {
                return memberB.BonusGiven - memberA.BonusGiven;
            });

            angular.forEach(tempMembers, function (member, index) {
                if (member.BonusGiven === "" || member.BonusGiven === null) {
                    member.BonusRanking = null;
                }
                else {
                    if (member.BonusGiven >= 0) {
                        var previousMember = tempMembers[index - 1];

                        if (previousMember) {
                            member.BonusRanking = previousMember.BonusGiven === member.BonusGiven ?
                                previousMember.BonusRanking :
                                previousMember.BonusRanking + 1;
                        } else {
                            member.BonusRanking = index + 1;
                        }
                    }
                }
            });

            return tempMembers;
        }

        function calculateBonusRanking(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    var tempMembers = ranking(group.Members);

                    angular.forEach(group.Members, function (member) {
                        var rankingMemberIndex = tempMembers.map(function (item) { return item.Employee.EmployeeId; }).indexOf(member.Employee.EmployeeId);

                        member.BonusRanking = tempMembers[rankingMemberIndex].BonusRanking;
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
            calculateTeamTotalBonus(member.Employee.Group.GroupId);
            calculateBonusRanking(member.Employee.Group.GroupId);
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                vm.groups = PAFocusModel.evaluations;
            })["finally"](function () {
                lockScreenService.stop();
            });

        };

        vm.save = function () {
            angular.forEach(vm.groups, function (group) {
                if (PAFocusModel.validatePAFocusForm(group)) {
                    PAFocusModel.updatePAFocus(group);
                }
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));