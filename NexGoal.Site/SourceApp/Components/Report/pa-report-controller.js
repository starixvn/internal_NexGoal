(function (reportModule) {
    "use strict";

    reportModule.controller("PAReportController", paReportController);

    paReportController.$inject = ["$scope", "ReportModel", "CommonModel", "EmployeeModel", "utilitiesFactory", "lockScreenService"];

    function paReportController($scope, ReportModel, CommonModel, EmployeeModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.paReports = [];
        vm.employeeReports = [];
        vm.searchEmployee = "";
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodIds: null,
            groupIds: "",
            employee: {}
        };

        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.searchKey = "";

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);

                CommonModel.getReadyPeriod().then(function () {
                    vm.periods = utilitiesFactory.groupingPeriods(CommonModel.periods); 

                    if (!utilitiesFactory.checkInProcessPeriod(vm.periods)) {
                        utilitiesFactory.checkLastestPeriods(vm.periods);
                    }

                    utilitiesFactory.numberEntryList(vm);
                    vm.query();
                });
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === '' && vm.filter.employee) {
                vm.filter.employee = {};
            }
        };

        vm.exportExcel = function () {
            $scope.$broadcast("exportPAReport");
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.groupIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");
            vm.filter.periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, "PeriodId");

            ReportModel.getPAReport(vm.filter.periodIds, vm.filter.groupIds, utilitiesFactory.getEmployeeIdFromAutoSearch(vm.searchEmployee,vm.filter.employee)).then(function () {
                vm.paReports = ReportModel.paReports;
                utilitiesFactory.searchOnPage(vm, vm.paReports);
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        initialize();
    }
})(angular.module("reportModule"));