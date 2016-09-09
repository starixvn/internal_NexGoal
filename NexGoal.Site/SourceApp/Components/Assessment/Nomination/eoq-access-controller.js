(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("EOQAccessController", eoqAccessController);

    eoqAccessController.$inject = ["EOQModel", "CommonModel", "enumerationsFactory", "utilitiesFactory", "lockScreenService"];

    function eoqAccessController(EOQModel, CommonModel, enumerationsFactory, utilitiesFactory,
        lockScreenService) {
        var vm = this;

        vm.period = {};
        vm.teams = [];
        vm.permission = 0;
        vm.filter = {
            query: "",
            teamIds: "",
            statusId: ""
        };

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            EOQModel.getCurrentEOQByNominatorId().then(function () {
                vm.period = EOQModel.period;
            });
        }

        vm.query = function () {
            alert("Implement Query Soon!")
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