(function (reportModule) {

    reportModule.controller("MasterReportController", masterReportController);

    masterReportController.$inject = ["$scope", "ReportModel", "EmployeeModel", "CommonModel", "utilitiesFactory", "lockScreenService"];

    function masterReportController($scope, ReportModel, EmployeeModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.employeeReports = [];
        vm.searchEmployee = "";
        vm.employee = null;
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: '',
            teamIds: '',
            employeeId: null
        };

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            CommonModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = CommonModel.periodOptions;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                vm.employee = null;
                vm.filter.employeeId = null;
            }
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");

            if (vm.employee) {
                vm.filter.employeeId = vm.employee.EmployeeId;
            }

            ReportModel.getMasterReport(vm.filter.periodId, vm.filter.teamIds, vm.filter.employeeId).then(function () {
                vm.employeeReports = ReportModel.masterReports;
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        initialize();
    }
})(angular.module("reportModule"));