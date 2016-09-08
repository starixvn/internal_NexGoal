(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQRolesManagementController", updateEOQRolesManagementController);

    updateEOQRolesManagementController.$inject = ["$scope", "EOQModel", "CommonModel", "utilitiesFactory",
        "enumerationsFactory", "lockScreenService"];

    function updateEOQRolesManagementController($scope, EOQModel, CommonModel, utilitiesFactory,
        enumerationsFactory, lockScreenService) {
        var vm = this;

        vm.IN_BOARD_ROLE = 2;
        vm.NOMINATOR_ROLE = 1;
        vm.period = {};
        vm.nominatorSettings = [];
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
        vm.permission = 0;
        vm.policyEnum = enumerationsFactory.policy;

        function initialize() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");

            EOQModel.getEOQNominatorSettingById(
                vm.period.EOQYId,
                vm.filter.query,
                vm.filter.teamIds,
                vm.filter.roleId,
                vm.filter.showAllEmployee).then(function () {
                    vm.nominatorSettings = EOQModel.nominatorSettings;

                    angular.forEach(vm.nominatorSettings, function (nominator) {
                        nominator.IsInBoard = nominator.RoleId === vm.IN_BOARD_ROLE;
                        nominator.IsNominator = nominator.RoleId === vm.NOMINATOR_ROLE;
                    });

                    utilitiesFactory.searchOnPage(vm, vm.nominatorSettings);
                })["finally"](function () {
                    lockScreenService.stop();
                });
        };

        vm.modifyRole = function (nominator, role) {
            if (role === vm.IN_BOARD_ROLE) {
                nominator.RoleId = nominator.IsInBoard ? vm.IN_BOARD_ROLE : 0;
                nominator.IsNominator = false;
            } else if (role === vm.NOMINATOR_ROLE) {
                nominator.RoleId = nominator.IsNominator ? vm.NOMINATOR_ROLE : 0;
                nominator.IsInBoard = false;
            } else {
                nominator.RoleId = 0;
                nominator.IsInBoard = false;
                nominator.IsNominator = false;
            }

            lockScreenService.start();

            var nominators = [];

            nominators.push(nominator);

            EOQModel.updateEOQNominatorSettingById(vm.period.EOQYId, nominators)["finally"](function () {
                lockScreenService.stop();
            });
        };

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOQCtrl.period;
            }
        });

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.permission;
        }, function (newVal) {
            if (newVal) {
                vm.permission = $scope.updateEOQCtrl.permission;
            }
        });

        initialize();
    }
})(angular.module("assessmentModule"));