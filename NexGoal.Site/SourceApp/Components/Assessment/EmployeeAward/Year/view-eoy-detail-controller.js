(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ViewEOYDetailController", viewEOYDetailController);

    viewEOYDetailController.$inject = ["$routeParams", "$filter", "$location", "EOYModel", "CommonModel",
        "historyFactory", "utilitiesFactory", "lockScreenService", "baseUrl"];

    function viewEOYDetailController($routeParams, $filter, $location, EOYModel, CommonModel,
        historyFactory, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;
        var firstRolesManagementLoaded = true;
        var IN_BOARD_ROLE = 2;
        var NOMINATOR_ROLE = 1;

        vm.period = null;
        vm.eoqyId = parseInt($routeParams.id, 10);
        vm.dataTab = {
            basicInformation: true,
            rolesManagement: false
        };
        vm.filter = {
            query: '',
            teamIds: '',
            roleId: '',
            showAllEmployee: false
        };
        vm.teams = [];
        vm.nominatorSettings = [];
        vm.searchKey = '';
        vm.filteredItems = [];
        vm.currentPage = 1;

        function initialize() {
            utilitiesFactory.getDateFormat(vm);

            lockScreenService.start();

            EOYModel.getEOYById(vm.eoqyId).then(function () {
                vm.period = EOYModel.period;

                if (vm.period) {
                    vm.period.RawStartDate = $filter('date')(vm.period.StartDate, vm.dateFormat);
                    vm.period.RawEndDate = $filter('date')(vm.period.EndDate, vm.dateFormat);
                }
            })['finally'](function () {
                lockScreenService.stop();
            });
        }

        function initRolesManagementTab() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);

                vm.query();
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.tabBasicInformationSelected = function () {
            vm.dataTab.basicInformation = true;
            vm.dataTab.rolesManagement = false;
        };

        vm.tabRolesManagementSelected = function () {
            vm.dataTab.basicInformation = false;
            vm.dataTab.rolesManagement = true;

            if (firstRolesManagementLoaded) {
                firstRolesManagementLoaded = false;
                initRolesManagementTab();
            }
        };

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, 'DepartmentId');

            EOYModel.getEOYNominatorSettingById(
                vm.period.EOQYId,
                vm.filter.query,
                vm.filter.teamIds,
                vm.filter.roleId,
                vm.filter.showAllEmployee).then(function () {
                    vm.nominatorSettings = EOYModel.nominatorSettings;

                    angular.forEach(vm.nominatorSettings, function (nominator) {
                        nominator.IsInBoard = nominator.RoleId === IN_BOARD_ROLE;
                        nominator.IsNominator = nominator.RoleId === NOMINATOR_ROLE;
                    });

                    vm.searchKey = '';
                    vm.currentPage = 1;

                    utilitiesFactory.searchOnPage(vm, vm.nominatorSettings);
                })['finally'](function () {
                    lockScreenService.stop();
                });
        };

        vm.backToList = function () {
            historyFactory.setBackStatus(true);
            $location.path(baseUrl + 'ManageNomination/Index').search({ secondTab: true });
        };

        initialize();
    }
})(angular.module("assessmentModule"));