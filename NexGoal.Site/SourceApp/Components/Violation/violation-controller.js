(function (violationModule) {
    "use strict";

    violationModule.controller("ViolationController", violationController);

    violationController.$inject = ["CommonModel", "EmployeeModel", "ViolationModel", "utilitiesFactory"];

    function violationController(CommonModel, EmployeeModel, ViolationModel, utilitiesFactory) {
        var vm = this;

        vm.violations = [];
        vm.teams = [];
        vm.searchEmployee = "";
        vm.filteredEmployee = {};
        vm.filter = {
            StatusId: "",
            TeamIds: "",
            EmployeeId: null,
            RawFromDate: new Date(),
            RawToDate: new Date(),
            FromDate: "",
            ToDate: ""
        };
        vm.calendar = {};

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            vm.calendar = ViolationModel.calendar;
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === "") {
                if (vm.filteredEmployee) {
                    vm.filter.EmployeeId = null;
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
            alert("Just Do Something!");
        };

        vm.createViolation = function () {
            ViolationModel.openPopupCreateViolation().then(function () {
                vm.modalInstance = null;
                vm.query();
            });
        };

        initialize();
    }
})(angular.module("violationModule"));