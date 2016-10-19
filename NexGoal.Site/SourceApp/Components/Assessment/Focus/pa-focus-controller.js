(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["$scope", "CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory",
        'lockScreenService', 'notificationFactory', 'enumerationsFactory'];

    function paFocusController($scope, CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory,
        lockScreenService, notificationFactory, enumerationsFactory) {
        var vm = this;

        vm.booleanRanges = [
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
        vm.focusEvaluationLevel = enumerationsFactory.focusEvaluationLevel;
        vm.isFirst = true;
        vm.isChangedData = false;
        $scope.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: 0,
            groupIds: null,
            employee: {}
        };
        vm.searchEmployee = "";
        vm.processStatus = enumerationsFactory.processStatusType;
        vm.permission = [];
        vm.policy = enumerationsFactory.policy;
        vm.allowToEdit = false;

        function initialize() {
            CommonModel.getReadyPeriod(enumerationsFactory.targetType.SemiAnnual).then(function () {
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

        function openPopupLostData() {
            PAFocusModel.openPopupLostData().then(function () {
                query();
            });
        }

        function query() {
            lockScreenService.start();
            vm.filter.groupIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');

            PAFocusModel.getPermission(vm.filter.periodId).then(function () {
                vm.permission = PAFocusModel.permission;
            });

            PAFocusModel.getPAFocusProcess(vm.filter.periodId).then(function () {
                vm.process = PAFocusModel.process;

                vm.allowToEdit = vm.process.length > 0
                    && vm.process[0].ProcessStatus === vm.processStatus.InProgress
                    && vm.permission === vm.policy.Host;
            });

            PAFocusModel.getPAFocus(vm.filter.periodId, vm.filter.groupIds, utilitiesFactory.getEmployeeIdFromAutoSearch(vm.searchEmployee, vm.filter.employee)).then(function () {
                $scope.groups = PAFocusModel.evaluations;
                vm.isFirst = true;
            })["finally"](function () {
                lockScreenService.stop();
            });
        }

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

        vm.query = function () {
            if (vm.isChangedData) {
                openPopupLostData();
            } else {
                query();
            }
        };

        vm.save = function () {
            lockScreenService.start();

            PAFocusModel.updatePAFocus($scope.groups).then(function () {
                vm.isChangedData = false;
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        vm.getFocusEvaluationLevel = function (value) {
            var level = utilitiesFactory.getFocusEvaluationLevel(value);

            if (level && level.Name.length === 0) {
                level.Name = "-";
            }

            return level.Name;
        }

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

        vm.exportExcel = function () {
            $scope.$broadcast('exportPAFocus');
        };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.employeeFilterChanged = function () {
            vm.filter.employee = {};
        };

        vm.openPopupLastObjective = function (member) {
            var copiedMember = angular.copy(member);

            copiedMember.IsReadOnly = !vm.allowToEdit;

            PAFocusModel.openPopupCurrentObjective(copiedMember).then(function (LastCompletedPercentObjective) {
                member.LastCompletedPercentObjective = LastCompletedPercentObjective;
            });
        };

        vm.openPopupObjective = function (member) {
            var copiedMember = angular.copy(member);

            copiedMember.IsReadOnly = !vm.allowToEdit;

            PAFocusModel.openPopupObjective(copiedMember).then(function (NextDescriptionObjective) {
                member.NextDescriptionObjective = NextDescriptionObjective;
            });
        };

        vm.onIsReadyPromotionFieldChange = function (member) {
            if (member.IsReadyPromotion !== null) {
                member.IsPoorPerformance = !member.IsPoorPerformance ? memberember.IsPoorPerformance : !member.IsReadyPromotion;
            }
        }

        vm.onIsPoorPerformanceFieldChange = function (member) {
            if (member.IsPoorPerformance !== null) {
                member.IsReadyPromotion = !member.IsReadyPromotion ? member.IsReadyPromotion : !member.IsPoorPerformance;
            }
        }

        initialize();
    }
})(angular.module("assessmentModule"));