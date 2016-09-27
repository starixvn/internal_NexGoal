(function (violationModule) {
    "use strict";

    violationModule.controller("ViolationController", violationController);

    violationController.$inject = ["CommonModel", "EmployeeModel", "ViolationModel", "enumerationsFactory",
        "utilitiesFactory", "lockScreenService"];

    function violationController(CommonModel, EmployeeModel, ViolationModel, enumerationsFactory,
        utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.searchKey = "";
        vm.filteredItems = [];
        vm.policyEnum = enumerationsFactory.policy;
        vm.permission = 0;
        vm.violations = [];
        vm.teams = [];
        vm.searchEmployee = "";
        vm.filteredEmployee = null;
        vm.filter = {
            StatusId: "",
            GroupIds: "",
            EmployeeId: "",
            RawFromDate: new Date(),
            RawToDate: new Date(),
            FromDate: "",
            ToDate: ""
        };
        vm.calendar = {};

        function getRawFromDate() {
            var currentDate = new Date();
            var sixMonthAgoDate = new Date(currentDate.setMonth(currentDate.getMonth() - 6));
            var month = sixMonthAgoDate.getMonth();
            var year = sixMonthAgoDate.getFullYear();

            return new Date(year, month, 1);
        }

        function initialize() {
            vm.calendar = ViolationModel.calendar;
            vm.filter.RawFromDate = getRawFromDate();

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);

                vm.query();
            });

            ViolationModel.getUserPermissionOnViolation().then(function () {
                vm.permission = ViolationModel.permission;
            });
        }

        function asignFilter() {
            if (vm.filteredEmployee) {
                vm.filter.EmployeeId = vm.filteredEmployee.EmployeeId;
            }

            vm.filter.GroupIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");
            vm.filter.FromDate = vm.filter.RawFromDate ? vm.filter.RawFromDate.toDateString() : null;
            vm.filter.ToDate = vm.filter.RawToDate ? vm.filter.RawToDate.toDateString() : null;
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === "") {
                if (vm.filteredEmployee) {
                    vm.filter.EmployeeId = "";
                    vm.filteredEmployee = null;
                }
            }
        };

        vm.openPopupFromDate = function (event) {
            ViolationModel.openPopupFromDate(event);
        };

        vm.openPopupToDate = function (event) {
            ViolationModel.openPopupToDate(event);
        };

        vm.query = function () {
            asignFilter();

            lockScreenService.start();

            ViolationModel.getViolations(vm.filter.FromDate,
                                        vm.filter.ToDate,
                                        vm.filter.StatusId,
                                        vm.filter.GroupIds,
                                        vm.filter.EmployeeId).then(function () {
                                            vm.violations = ViolationModel.violations;
                                            utilitiesFactory.searchOnPage(vm, vm.violations);
                                        })["finally"](function () {
                                            lockScreenService.stop();
                                        });
        };

        vm.createViolation = function () {
            ViolationModel.openPopupCreateViolation().then(function () {
                vm.query();
            });
        };

        vm.updateViolation = function (violation) {
            ViolationModel.openPopupUpdateViolation(violation).then(function () {
                vm.query();
            });
        };

        vm.modifyViolationStatus = function (violationId) {
            ViolationModel.modifyViolationStatus(violationId).then(function () {
                vm.query();
            });
        };

        initialize();
    }
})(angular.module("violationModule"));