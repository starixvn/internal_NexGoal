(function (assessmentModule) {
    'use strict';

    assessmentModule.controller('ManageNominationController', manageNominationController);

    manageNominationController.$inject = ['$routeParams', 'EOQModel'];

    function manageNominationController($routeParams, EOQModel) {
        var vm = this;
        var secondTab = $routeParams.secondTab ? true : false;

        vm.permission = 0;

        vm.dataTab = {
            eoq: !secondTab,
            eoy: secondTab
        };

        function initialize() {
            EOQModel.getUserPermissionOnManageNomination().then(function () {
                vm.permission = EOQModel.permission;
            });
        }

        vm.tabEOQSelected = function () {
            vm.dataTab.eoq = true;
            vm.dataTab.eoy = false;
        };

        vm.tabEOYSelected = function () {
            vm.dataTab.eoq = false;
            vm.dataTab.eoy = true;
        };

        initialize();
    }
})(angular.module('assessmentModule'));