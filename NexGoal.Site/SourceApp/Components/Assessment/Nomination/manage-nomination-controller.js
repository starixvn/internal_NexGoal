(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("ManageNominationController", manageNominationController);

    manageNominationController.$inject = ["EOQModel"];

    function manageNominationController(EOQModel) {
        var vm = this;

        vm.permission = 0;

        vm.dataTab = {
            eoq: true,
            eoy: false
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
})(angular.module("assessmentModule"));