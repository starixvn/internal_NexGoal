(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("EOQAccessController", eoqAccessController);

    eoqAccessController.$inject = ["EOQModel", "CommonModel", "enumerationsFactory", "utilitiesFactory", "lockScreenService"];

    function eoqAccessController(EOQModel, CommonModel, enumerationsFactory, utilitiesFactory,
        lockScreenService) {
        var vm = this;

        vm.IN_BOARD_ROLE = 2;
        vm.NOMINATOR_ROLE = 1;
        vm.period = {};
        vm.teams = [];
        vm.permission = 0;
        vm.filter = {
            query: "",
            teamIds: "",
            statusId: ""
        };
        vm.eoqNominations = [];

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            EOQModel.getCurrentEOQByNominatorId().then(function () {
                vm.period = EOQModel.period;

                if (EOQModel.period) {
                    vm.query();
                }
            });
        }

        vm.query = function () {
            lockScreenService.start();

            vm.filter.teamIds = utilitiesFactory.getSelectedDataByProperty(vm.teams, "DepartmentId");

            EOQModel.getEOQNominationByNominatorId(
                vm.period.Period.EOQYId,
                vm.period.RoleId,
                vm.filter.query,
                vm.filter.teamIds,
                vm.filter.statusId).then(function () {
                    vm.eoqNominations = EOQModel.eoqNominations;
                })["finally"](function () {
                    lockScreenService.stop();
                });
        };

        vm.addNominee = function () {
            EOQModel.openPopupAddNominee(vm.period).then(function (nominee) {
                vm.modalInstance = null;

                lockScreenService.start();

                EOQModel.createEOQNomination(vm.period.Period.EOQYId, nominee.employeeId, nominee.eoqQuestions).then(function () {
                    vm.query();
                })["finally"](function () {
                    lockScreenService.stop();
                });
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));