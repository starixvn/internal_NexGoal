(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOYController', updateEOYController);

    updateEOYController.$inject = ['$routeParams', '$location', 'EOYModel', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function updateEOYController($routeParams, $location, EOYModel, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.period = null;
        vm.permission = 0;
        vm.eoqyId = parseInt($routeParams.id, 10);
        vm.viewMode = $routeParams.viewSetting ? true : false;

        vm.dataTab = {
            basicInformation: !vm.viewMode,
            rolesManagement: vm.viewMode
        };

        vm.tabBasicInformationSelected = function () {
            vm.dataTab.basicInformation = true;
            vm.dataTab.rolesManagement = false;
        };

        vm.tabRolesManagementSelected = function () {
            vm.dataTab.basicInformation = false;
            vm.dataTab.rolesManagement = true;
        };

        vm.backToList = function () {
            historyFactory.setBackStatus(true);
            $location.path(baseUrl + 'ManageNomination/Index').search({ secondTab: true });
        };

        function initialize() {
            lockScreenService.start();

            EOYModel.getUserPermissionOnManageNomination().then(function () {
                vm.permission = EOYModel.permission;
            });

            EOYModel.getEOYById(vm.eoqyId).then(function () {
                vm.period = EOYModel.period;
            })['finally'](function () {
                lockScreenService.stop();
            });
        }

        initialize();
    }
})(angular.module('assessmentModule'));