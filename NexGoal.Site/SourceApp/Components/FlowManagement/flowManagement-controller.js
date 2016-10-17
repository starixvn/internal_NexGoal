'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
["$location", 'FlowMngModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'baseUrl', "lockScreenService",
function ($location, FlowMngModel, CommonModel, utilitiesFactory, enumerationsFactory, baseUrl, lockScreenService) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.flowTemplates = FlowMngModel.flowTemplates;
    vm.permission = FlowMngModel.permission;

    vm.policyEnum = enumerationsFactory.policy;

    vm.filter = {
        targetTypeId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.currentPage = 1;

    var getFlowTemplate = function (targetTypeId, isActive) {
        lockScreenService.start();

        FlowMngModel.getFlowTemplate(targetTypeId, isActive).then(function () {
            vm.flowTemplates = FlowMngModel.flowTemplates;
            utilitiesFactory.searchOnPage(vm, vm.flowTemplates);
        })["finally"](function () {
            lockScreenService.stop();
        });
    };

    vm.createFlowTemplate = function () {
        FlowMngModel.openPopupCreateFlowTemplate().then(function () {
            vm.query(vm.filter);
        });
    };

    vm.updateFlowTemplate = function (flowTemplate) {
        $location.path(baseUrl + 'Flow/Update/' + flowTemplate.FlowTemplateId);
    };

    vm.updateStatusFlowGroup = function (flowTemplate) {
        FlowMngModel.openPopupUpdateStatusFlowTemplate(flowTemplate);
    };

    vm.deleteFlowTemplate = function (flowTemplate) {
        FlowMngModel.openPopupDeleteFlowTemplate(vm, flowTemplate);
    };

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getFlowTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.initialize = function () {
        FlowMngModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowMngModel.permission;
        });

        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        getFlowTemplate(vm.filter.targetTypeId, vm.filter.isActive);
    };

    vm.initialize();

    /// Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);

flowMngModule.controller("UpdateFlowTemplateController",
['$routeParams', 'FlowMngModel', 'CommonModel', 'enumerationsFactory', 'cacheService', 'historyFactory',
function ($routeParams, FlowMngModel, CommonModel, enumerationsFactory, cacheService, historyFactory) {
    var vm = this;

    vm.flowTemplateId = parseInt($routeParams.id, 10);
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.remindDay = CommonModel.remindDay;
    vm.permission = FlowMngModel.permission;

    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.tabDepartmentSelected = function () {
        cacheService.setProperty(1);
    };

    vm.tabPersonalSelected = function () {
        cacheService.setProperty(2);
    };

    vm.tabResultSettingSelected = function () {
        cacheService.setProperty(3);
    };

    vm.initialize = function () {
        FlowMngModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowMngModel.permission;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        FlowMngModel.getFlowTemplateById(vm.flowTemplateId).then(function () {
            vm.flowTemplate = FlowMngModel.flowTemplate;
            vm.flowTemplate.TargetTypeId = FlowMngModel.flowTemplate.TargetType.TargetTypeId;
        });
    };

    vm.backToList = function () {
        historyFactory.back();
    };

    vm.initialize();
}]);

flowMngModule.controller("GeneralInformationController",
['$scope', '$routeParams', 'FlowMngModel', 'CommonModel',
function ($scope, $routeParams, FlowMngModel, CommonModel) {
    var vm = this;

    vm.flowTemplateId = parseInt($routeParams.id, 10);
    vm.targetTypes = CommonModel.targetTypes;

    $scope.$watch(function () { return $scope.updateCtrl.flowTemplate; },
        function (newVal) {
            if (newVal) {
                vm.flowTemplate = $scope.updateCtrl.flowTemplate;

                CommonModel.getTargetType(true).then(function () {
                    vm.targetTypes = CommonModel.targetTypes;

                    var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.flowTemplate.TargetTypeId);
                    if (index === -1) {
                        vm.targetTypes.push(vm.flowTemplate.TargetType);
                    }
                });
            }
        });

    vm.save = function (flowTemplate) {
        if (FlowMngModel.validateFlowTemplate(flowTemplate)) {
            FlowMngModel.updateFlowTemplate(flowTemplate);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);