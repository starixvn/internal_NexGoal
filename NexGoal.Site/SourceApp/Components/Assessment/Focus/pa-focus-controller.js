(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "utilitiesFactory"];

    function paFocusController(CommonModel, EmployeeModel, utilitiesFactory) {
        var vm = this;

        vm.employees = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null,
            employeeId: null
        };
        vm.searchEmployee = "";

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

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            //vm.filter.employeeId = vm.searchEmployee;

            console.log(vm.filter);

            alert("Implement Query Later");
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
            });
        }

        initialize();
    }
})(angular.module("assessmentModule"));