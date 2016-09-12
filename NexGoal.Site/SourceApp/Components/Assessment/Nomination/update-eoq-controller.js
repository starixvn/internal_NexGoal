(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('UpdateEOQController', updateEOQController);

    updateEOQController.$inject = ['$routeParams', 'EOQModel', 'historyFactory', 'lockScreenService'];

    function updateEOQController($routeParams, EOQModel, historyFactory, lockScreenService) {
        var vm = this;

        vm.period = null;
        vm.permission = 0;
        vm.eoqId = parseInt($routeParams.id, 10);
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
            historyFactory.back();
        };

        function initialize() {
            lockScreenService.start();

            EOQModel.getUserPermissionOnManageNomination().then(function () {
                vm.permission = EOQModel.permission;
            });

            EOQModel.getEOQById(vm.eoqId).then(function () {
                vm.period = EOQModel.period;
            })['finally'](function () {
                lockScreenService.stop();
            });
        }

        initialize();
    }
})(angular.module('assessmentModule'));