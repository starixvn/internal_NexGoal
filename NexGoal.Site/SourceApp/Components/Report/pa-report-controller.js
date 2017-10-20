(function (reportModule) {
    "use strict";

    reportModule.controller("PAReportController", paReportController);

    paReportController.$inject = ["$location", "$scope", "ReportModel", "CommonModel",
        "EmployeeModel", "utilitiesFactory", "lockScreenService", "baseUrl", "enumerationsFactory"];

    function paReportController($location, $scope, ReportModel, CommonModel,
        EmployeeModel, utilitiesFactory, lockScreenService, baseUrl, enumerationsFactory) {
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
        vm.permissionOnRecalculateScore = false;

        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.searchKey = "";
        var targetType = enumerationsFactory.targetType;

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

            ReportModel.getPermissionOnRecalculateScore().then(function () {
                vm.permissionOnRecalculateScore = ReportModel.permissionOnRecalculateScore;
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

        vm.exportPDF = function (epaId, targetTypeId) {
            var iframe = $('body iframe');
            var url = baseUrl + 'ExportPDF/ProbationDetail';
            if (targetTypeId === targetType.SemiAnnual) {
                url = baseUrl + 'ExportPDF/SemiDetail';
            }

            url += '?epaId=' + epaId;
            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', url);
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.groupIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");
            vm.filter.periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, "PeriodId");

            ReportModel.getPAReport(
                vm.filter.periodIds,
                vm.filter.groupIds,
                utilitiesFactory.getEmployeeIdFromAutoSearch(vm.searchEmployee, vm.filter.employee)).then(function () {
                    vm.paReports = ReportModel.paReports;
                    utilitiesFactory.searchOnPage(vm, vm.paReports);
                })["finally"](function () {
                    lockScreenService.stop();
                });
        };

        vm.viewEPADetail = function (epaId) {
            $location.path(baseUrl + 'EPA/Detail/' + epaId);
        };

        vm.recalculateScore = function () {
            vm.filter.groupIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");
            vm.filter.periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, "PeriodId");

            ReportModel.recalculateScore(
                vm,
                vm.filter.periodIds,
                vm.filter.groupIds,
                utilitiesFactory.getEmployeeIdFromAutoSearch(vm.searchEmployee, vm.filter.employee));
        };

        initialize();
    }
})(angular.module("reportModule"));