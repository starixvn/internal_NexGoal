(function (targetTemplateModule) {
    "use strict";

    targetTemplateModule.controller("ViewTargetTemplateDetailController", viewTargetTemplateDetailController);

    viewTargetTemplateDetailController.$inject = ["$scope", "$modalInstance", "TargetTemplateModel", "utilitiesFactory", "targetTemplateId"];

    function viewTargetTemplateDetailController($scope, $modalInstance, TargetTemplateModel, utilitiesFactory, targetTemplateId) {
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

            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initialize();
    }
})(angular.module("targetTemplateModule"));