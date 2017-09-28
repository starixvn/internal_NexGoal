(function () {
    'use strict';

    angular
        .module('reportModule')
        .controller('NominationReportController', nominationReportController);

    nominationReportController.$inject = ['historyFactory'];

    function nominationReportController(historyFactory) {
        /* jshint validthis:true */
        var vm = this;
        vm.dataTab = {};

        function initTab() {
            var dataTab = historyFactory.getState();

            return dataTab ? dataTab : {
                eoq: false,
                eoy: false
            };
        }

        function initialize() {
            vm.dataTab = initTab();
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
})();
