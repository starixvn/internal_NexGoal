(function (flowMngModule) {
    "use strict";

    flowMngModule.controller("CreateFlowTemplateController", createFlowTemplateController);

    createFlowTemplateController.$inject = ['$scope', '$modalInstance', 'FlowMngModel', 'CommonModel'];

    function createFlowTemplateController($scope, $modalInstance, FlowMngModel, CommonModel) {
        var vm = this;

        vm.targetTypes = CommonModel.targetTypes;
        vm.flowTemplate = {
            Name: '',
            TargetTypeId: '',
            IsActive: true,
            Description: ''
        };

        function initalize() {
            CommonModel.getTargetType(true).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss('cancel');
            });
        }

        vm.save = function () {
            if (FlowMngModel.validateFlowTemplate(vm.flowTemplate)) {
                FlowMngModel.createFlowTemplate(vm.flowTemplate, $modalInstance);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };

        initalize();
    }
})(angular.module("flowMngModule"));