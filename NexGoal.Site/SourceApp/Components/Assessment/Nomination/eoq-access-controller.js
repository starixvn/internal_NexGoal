(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("EOQAccessController", eoqAccessController);

    eoqAccessController.$inject = ["EOQModel", "CommonModel", "enumerationsFactory", "utilitiesFactory",
        "lockScreenService"];

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

        vm.addNominee = function () {
            EOQModel.openPopupAddNominee().then(function () {
                vm.modalInstance = null;
            });
        };

        initialize();
    }
})(angular.module("assessmentModule"));