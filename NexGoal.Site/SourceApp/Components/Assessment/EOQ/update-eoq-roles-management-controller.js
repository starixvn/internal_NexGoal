(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQRolesManagementController", updateEOQRolesManagementController);

    updateEOQRolesManagementController.$inject = ["$scope", "EOQModel", "EmployeeModel", "CommonModel", "utilitiesFactory"];

    function updateEOQRolesManagementController($scope, EOQModel, EmployeeModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.employees = [];
        vm.searchEmployee = "";
        vm.searchKey = "";
        vm.filteredItems = [];
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.filter = {
            EmployeeId: "",
            TeamId: "",
            RoleId: "",
            showAllEmployee: false
        };

        function initialize() {
            utilitiesFactory.numberEntryList(vm);
        }

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

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.query = function () { };

        vm.search = function () { };

        initialize();
    }
})(angular.module("assessmentModule"));