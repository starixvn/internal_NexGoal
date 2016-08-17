'use strict';
/*global reportModule*/

reportModule.controller('EPASummaryReportController',
['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService',
function ($scope, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = ReportModel.periods;
    vm.epaSummary = [];

    vm.pageOption = utilitiesFactory.pageOption;

    vm.filter = {
        periodIds: '',
        departmentId: '',
        isLocal: '',
        pageIndex: utilitiesFactory.pageOption.pageIndex,
        itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
        sortColumnName: '',
        sortDirection: ''
    };

    function getEPASummaryReport(filter) {
        lockScreenService.start();

        ReportModel.getEPASummaryReport(
            filter.periodIds,
            filter.departmentId,
            filter.isLocal,
            filter.sortColumnName,
            filter.sortDirection,
            filter.pageIndex,
            filter.itemsPerPage)
        .then(function () {
            vm.epaSummary = ReportModel.epaSummary;
        })["finally"](function () {
            lockScreenService.stop();
        });
    }

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        getEPASummaryReport(vm.filter);
    };

    vm.sortChanging = function (sortColumnName, sortDirection) {
        vm.filter.sortColumnName = sortColumnName;
        vm.filter.sortDirection = sortDirection;

        getEPASummaryReport(vm.filter);
    };

    vm.query = function (filter) {
        filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
        filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');
        filter.sortColumnName = '';
        filter.sortDirection = '';

        getEPASummaryReport(filter);
    };

    vm.exportReport = function () {
        vm.exportCriteria = {
            periodIds: utilitiesFactory.getSelectedData(vm.periods, 'PeriodId'),
            departmentId: vm.filter.departmentId,
            isLocal: vm.filter.isLocal,
            sortColumnName: vm.filter.sortColumnName,
            sortDirection: vm.filter.sortDirection
        };

        $scope.$broadcast('exportEpaSummaryReport');
    };

    function initialize() {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        ReportModel.getAllPeriodIncludeImport().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(ReportModel.periods);

            getEPASummaryReport(vm.filter);
        });
    }

    initialize();
}]);