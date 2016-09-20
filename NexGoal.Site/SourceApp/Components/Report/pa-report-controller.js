(function (reportModule) {
    reportModule.controller("PAReportController", paReportController);

    paReportController.$inject = ["ReportModel", "CommonModel", "EmployeeModel", "utilitiesFactory", "lockScreenService"];

    function paReportController(ReportModel, CommonModel, EmployeeModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.paReports = [];
        vm.employeeReports = [];
        vm.searchEmployee = "";
        vm.employee = null;
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodIds: '',
            teamIds: '',
            employeeId: 0
        };

        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.searchKey = '';

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            ReportModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = utilitiesFactory.groupingPeriods(ReportModel.periods);
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

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