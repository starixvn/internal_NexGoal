(function () {
    'use strict';

    angular
        .module('reportModule')
        .controller('EOQReportController', eoqReportController);

    eoqReportController.$inject = ['$scope', 'ReportModel', 'EOQModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function eoqReportController($scope, ReportModel, EOQModel, CommonModel, utilitiesFactory, lockScreenService) {
        /* jshint validthis:true */
        var vm = this;
        vm.filter = {
            eoqyId: -1,
            query: '',
            teamIds: '',
            statusId: ''
        };
        vm.isFirstLoad = true;
        vm.nominationReports = [];
        vm.periods = [];
        vm.query = query;
        vm.viewNomination = viewNomination;

        $scope.$watch(function () { return $scope.nominationReportCtrl.dataTab.eoq; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
                vm.isFirstLoad = false;
            }
        });

        function initialize() {
            EOQModel.getEOQ().then(function () {
                vm.periods = EOQModel.periods;
                vm.filter.eoqyId = vm.periods.length > 0 ? vm.periods[0].EOQYId : -1;

                query();
            });

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });
        }

        function query() {
            lockScreenService.start();
            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");

            ReportModel.getNominationReport(vm.filter.eoqyId, vm.filter.query, vm.filter.teamIds, vm.filter.statusId).then(function () {
                vm.nominationReports = ReportModel.nominationReports;
            })["finally"](function () {
                lockScreenService.stop();
            });
        }

        function viewNomination(nomination) {
            EOQModel.openPopupViewNomination(nomination.NominationId).then(function () {
                vm.modalInstance = null;
            });
        }
    }
})();