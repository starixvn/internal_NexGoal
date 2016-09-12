(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory", 'lockScreenService', 'notificationFactory', 'enumerationsFactory'];

    function paFocusController(CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory, lockScreenService, notificationFactory, enumerationsFactory) {
        var vm = this;
        vm.objectivesAchievementRanges = ["0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"];
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
            employee: {}
        };
        vm.searchEmployee = "";

        vm.showEmployee = function (member) {
            var isShowEmployeeRow = vm.filter.employee.EmployeeId === undefined
                || vm.filter.employee.EmployeeId === member.Employee.EmployeeId;

            return isShowEmployeeRow;
        };

        function initialize() {
            CommonModel.getPeriod(enumerationsFactory.targetType.SemiAnnual).then(function () {
                vm.periods = CommonModel.periods;

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
                if (vm.filter.employee) {
                    vm.filter.employee = {};
                }
            }
        };

        vm.openPopupObjective = function (member) {
            PAFocusModel.openPopupObjective(member).then(function (NextDescriptionObjective) {
                member.NextDescriptionObjective = NextDescriptionObjective;
            });
        };

        vm.inputBonus = function (member,group) {
            calculateTeamTotalBonus(member.Employee.Group.GroupId);
            calculateBonusRanking(member.Employee.Group.GroupId);
            if (group.TotalBonus > group.MaxBonus) {
                notificationFactory.error("TotalBonusGivenMoreThanMaxMessage");
            }
        };

        function CalculateBonusGivenOneGroup(group){
            var total = 0;
            angular.forEach(group.Members, function (member) {
                if (member.BonusGiven) {
                    total += member.BonusGiven;
                }
            });

            return parseFloat(total.toFixed(2));
        }
        vm.query = function () {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employee.EmployeeId).then(function () {
                vm.groups = PAFocusModel.evaluations;
            })["finally"](function () {
                lockScreenService.stop();
            });

        };

        vm.save = function () {
            var isValid = PAFocusModel.validatePAFocusForm(vm.groups);
            if (isValid) {
                var isWarning = PAFocusModel.validateWarningPAFocusForm(vm.groups);
                if (isWarning) {
                    PAFocusModel.OpenPopupContinue(vm.groups);
                } else {
                    PAFocusModel.updatePAFocus(vm.groups);
                }
            }
        };

        initialize();
    }
})(angular.module("assessmentModule"));