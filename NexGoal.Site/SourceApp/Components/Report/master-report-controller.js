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
            employeeId: 0
        };

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            ReportModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = ReportModel.periods;

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
                vm.filter.employeeId = 0;
            }
        };

        initialize();
    }
})(angular.module("reportModule"));