(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["$scope", "CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory",
        'lockScreenService', 'notificationFactory', 'enumerationsFactory'];

    function paFocusController($scope, CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory,
        lockScreenService, notificationFactory, enumerationsFactory) {
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
        vm.process = [];

        vm.isFirst = true;
        vm.isChangedData = false;
        $scope.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: 0,
            teamIds: null,
            employee: {}
        };
        vm.searchEmployee = "";
        vm.processStatus = enumerationsFactory.processStatusType;

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
                    query();
                });
            });
        }

        function calculateTeamTotalBonus(teamId) {
            angular.forEach($scope.groups, function (group) {
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
            angular.forEach($scope.groups, function (group) {
                if (group.TeamId === teamId) {
                    var tempMembers = ranking(group.Members);

                    angular.forEach(group.Members, function (member) {
                        var rankingMemberIndex = tempMembers.map(function (item) { return item.Employee.EmployeeId; }).indexOf(member.Employee.EmployeeId);

                        member.BonusRanking = tempMembers[rankingMemberIndex].BonusRanking;
                    });
                }
            });
        }

        function openPopupReset() {
            PAFocusModel.openPopupReset().then(function () {
                query();
            });
        }

        function openPopupQuery() {
            PAFocusModel.openPopupQuery().then(function () {
                query();
            });
        }

        function CalculateBonusGivenOneGroup(group) {
            var total = 0;
            angular.forEach(group.Members, function (member) {
                if (member.BonusGiven) {
                    total += member.BonusGiven;
                }
            });

            return parseFloat(total.toFixed(2));
        }

        function query() {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            PAFocusModel.getPAFocusProcess(vm.filter.periodId).then(function () {
                vm.process = PAFocusModel.process;
            });

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.teamIds, vm.filter.employee.EmployeeId).then(function () {
                $scope.groups = PAFocusModel.evaluations;
                vm.isFirst = true;
            })["finally"](function () {
                lockScreenService.stop();
            });
        }

        vm.inputBonus = function (member, group) {
            member.BonusGiven = member.BonusGiven > group.MaxBonus ? group.MaxBonus : member.BonusGiven;
            calculateTeamTotalBonus(member.Employee.Group.GroupId);
            calculateBonusRanking(member.Employee.Group.GroupId);
            if (group.TotalBonus > group.MaxBonus) {
                notificationFactory.error("TotalBonusGivenMoreThanMaxMessage");
            }
        };

        $scope.$watch("groups", function (newValue, oldValue) {
            if (newValue !== oldValue) {
                if (vm.isFirst) {
                    vm.isFirst = false;
                    vm.isChangedData = false;
                } else {
                    vm.isChangedData = true;
                }
            }

        }, true);

        vm.reset = function () {
            if (vm.isChangedData) {
                openPopupReset();
            } else {
                query();
            }
        };

        vm.query = function () {
            if (vm.isChangedData) {
                openPopupQuery();
            } else {
                query();
            }
        };

        vm.save = function () {
            var isValid = PAFocusModel.validatePAFocusForm($scope.groups);
            if (isValid) {
                var isWarning = PAFocusModel.isWarningPAFocusForm($scope.groups);
                if (isWarning) {
                    PAFocusModel.OpenPopupContinue($scope.groups);
                } else {
                    PAFocusModel.updatePAFocus($scope.groups);
                }
            }
        };


        vm.getClassProgress = function (processStatus) {
            if (processStatus === vm.processStatus.InProgress) {
                return 'in-process';
            }
            else if (processStatus === vm.processStatus.Closed) {
                return 'end-process';
            }
            else {
                return 'open';
            }
        };

        vm.showEmployee = function (member) {
            var isShowEmployeeRow = vm.filter.employee.EmployeeId === undefined
                || vm.filter.employee.EmployeeId === member.Employee.EmployeeId;

            return isShowEmployeeRow;
        };

        vm.exportExcel = function () {
            $scope.$broadcast('exportPAFocus');
        };

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

        initialize();
    }
})(angular.module("assessmentModule"));