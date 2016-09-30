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
        vm.focusEvaluationLevel = PAFocusModel.focusEvaluationLevel;
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

        function openPopupLostData() {
            PAFocusModel.openPopupLostData().then(function () {
                query();
            });
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