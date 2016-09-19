(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ManageNominationController', manageNominationController);

    manageNominationController.$inject = ['$routeParams', 'EOQModel', 'historyFactory'];

    function manageNominationController($routeParams, EOQModel, historyFactory) {
        var vm = this;
        var secondTab = $routeParams.secondTab ? true : false;

        vm.permission = 0;

        vm.dataTab = {};

        function initTab() {
            var dataTab = historyFactory.getState();

            return dataTab ? dataTab : {
                eoq: !secondTab,
                eoy: secondTab
            };
        }

        function initialize() {
            vm.dataTab = initTab();

            EOQModel.getUserPermissionOnManageNomination().then(function () {
                vm.permission = EOQModel.permission;
            });
        }

        vm.tabEOQSelected = function () {
            vm.dataTab.eoq = true;
            vm.dataTab.eoy = false;

            historyFactory.saveState(vm.dataTab);
        };

        vm.tabEOYSelected = function () {
            vm.dataTab.eoq = false;
            vm.dataTab.eoy = true;

            historyFactory.saveState(vm.dataTab);
        };

        initialize();
    }
})(angular.module('assessmentModule'));