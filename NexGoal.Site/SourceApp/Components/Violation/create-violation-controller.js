(function (violationModule) {
    "use strict";

    violationModule.controller("CreateViolationController", createViolationController);

    createViolationController.$inject = ["$scope", "$modalInstance", "EmployeeModel", "ViolationModel"];

    function createViolationController($scope, $modalInstance, EmployeeModel, ViolationModel) {
        var vm = this;

        function initialize() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        vm.searchEmployee = "";
        vm.filteredEmployee = null;
        vm.violation = {
            EmployeeId: null,
            RawEffectedDate: new Date(),
            EffectedDate: null,
            Description: null
        };
        vm.calendar = { EffectedDateOpened: false };

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === "") {
                if (vm.filteredEmployee) {
                    vm.violation.EmployeeId = null;
                    vm.filteredEmployee = null;
                }
            }
        };

        vm.openPopupEffectedDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.EffectedDateOpened = true;
        };

        vm.submit = function () {
            vm.violation.Employee = vm.searchEmployee;
            if (vm.filteredEmployee) {
                vm.violation.EmployeeId = vm.filteredEmployee.EmployeeId;
            }

            if (ViolationModel.validateViolation(vm.violation)) {
                var employeeFullName = vm.filteredEmployee.FullName + " (" + vm.filteredEmployee.Name + ")";
                var createConfirmed = confirm("Do you want to send this warning to " + employeeFullName + "!");
                if (createConfirmed) {
                    vm.violation.EffectedDate = vm.violation.RawEffectedDate.toDateString();

                    ViolationModel.createViolation(vm.violation.EmployeeId,
                                                    vm.violation.EffectedDate,
                                                    vm.violation.Description,
                                                    $modalInstance);
                }
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("violationModule"));