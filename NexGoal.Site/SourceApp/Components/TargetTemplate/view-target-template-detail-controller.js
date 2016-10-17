(function (targetTemplateModule) {
    "use strict";

    targetTemplateModule.controller("ViewTargetTemplateDetailController", viewTargetTemplateDetailController);

    viewTargetTemplateDetailController.$inject = ["$modalInstance", "TargetTemplateModel", "utilitiesFactory", "targetTemplateId"];

    function viewTargetTemplateDetailController($modalInstance, TargetTemplateModel, utilitiesFactory, targetTemplateId) {
        var vm = this;

        vm.targetTemplateId = targetTemplateId;
        vm.targetTemplate = {};
        vm.kpiList = [];

        utilitiesFactory.getDateFormat(vm);

        function initialize() {
            TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
                vm.targetTemplate = TargetTemplateModel.targetTemplate;
            });

            TargetTemplateModel.getKPIOfTargetTemplate(vm.targetTemplateId, true).then(function () {
                vm.kpiList = TargetTemplateModel.kpiList;
            });
        }

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("targetTemplateModule"));