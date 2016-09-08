(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQRolesManagementController", updateEOQRolesManagementController);

    updateEOQRolesManagementController.$inject = ["$scope", "EOQModel", "EmployeeModel", "CommonModel",
        "utilitiesFactory", "lockScreenService"];

    function updateEOQRolesManagementController($scope, EOQModel, EmployeeModel, CommonModel,
        utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.nominatorSettings = [];
        vm.searchEmployee = "";
        vm.searchKey = "";
        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.filter = {
            query: "",
            teamIds: "",
            roleId: "",
            showAllEmployee: false
        };
        vm.teams = [];

        function initialize() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.query) {
                    vm.filter.query = null;
                }
            }
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");
            EOQModel.GetEOQNominatorSettingById(vm.filter.query, vm.filter.teamIds, vm.filter.roleId, vm.filter.showAllEmployee).then(function () {
                vm.nominatorSettings = EOQ.nominatorSettings;
            })["finally"](function () {
                lockScreenService.stop();
            });
        };

        vm.search = function () { };

        initialize();
    }
})(angular.module("assessmentModule"));