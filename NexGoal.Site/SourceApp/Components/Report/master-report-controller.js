(function (reportModule) {
    "use strict";

    reportModule.controller("MasterReportController", masterReportController);

    masterReportController.$inject = ["$scope", "ReportModel", "EmployeeModel", "CommonModel", "utilitiesFactory", "lockScreenService", "enumerationsFactory"];

    function masterReportController($scope, ReportModel, EmployeeModel, CommonModel, utilitiesFactory, lockScreenService, enumerationsFactory) {
        var vm = this;

        vm.currentOnFocusRecord = null;
        vm.masterReports = [];
        vm.searchEmployee = "";
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: '',
            teamIds: '',
            employee: {}
        };

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            CommonModel.getPeriod(enumerationsFactory.targetType.SemiAnnual).then(function () {
                vm.periods = CommonModel.periods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });
        }

        vm.nominationOnClick = function (record) {
            record.onNominationExpand = !record.onNominationExpand;
        };

        vm.groupOnClick = function (group) {
            group.onGroupExpand = !group.onGroupExpand;
        };

        vm.recordOnFocus = function (record) {
            if (vm.currentOnFocusRecord) {
                vm.currentOnFocusRecord.onFocus = !vm.currentOnFocusRecord.onFocus;
            }

            record.onFocus = !record.onFocus;
            vm.currentOnFocusRecord = record;
        };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === '' && vm.filter.employee) {
                vm.filter.employee = {};
            }
        };

        vm.query = function () {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");

            ReportModel.getMasterReport(vm.filter.periodId, vm.filter.teamIds, vm.filter.employee.EmployeeId).then(function () {
                vm.masterReports = ReportModel.masterReports;
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        vm.openPopupEvaluatorComment = function (employee) {
            ReportModel.openPopupEvaluatorComment(employee.EvaluatorComment);
        };

        vm.openPopupNextObjectives = function (employee) {
            ReportModel.openPopupNextObjectives(employee.PAFocus.openPopupNextObjectives);
        };

        vm.openPopupProjectComment = function (member, project) {
            project.Employee = {
                FullName: member.PAFocus.Employee.FullName,
                NickName: member.PAFocus.Employee.NickName
            };

            ReportModel.openPopupProjectComment(project);
        };

        initialize();
    }
})(angular.module("reportModule"));