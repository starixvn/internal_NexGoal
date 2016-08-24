'use strict';
/*global epaModule*/

epaModule.controller('IndexEvaluatorController',
["$location", 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', "lockScreenService",
    'enumerationsFactory', 'historyFactory', 'baseUrl',
function ($location, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, lockScreenService,
    enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.memberEPAs = [];
    vm.members = [];
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.filter = {};

    vm.handleOnPeriodClose = function () {
        var periodIds = utilitiesFactory.getSelectedData(vm.periods, "PeriodId");

        if (periodIds !== vm.filter.periodIds) {
            vm.filter.periodIds = periodIds;

            getGroupsByPeriods(periodIds);
        }
    };

    vm.handleOnGroupClose = function () {
        var groupIds = utilitiesFactory.getSelectedData(vm.groupOptions, "GroupId");

        if (groupIds !== vm.filter.groupIds) {
            vm.filter.groupIds = groupIds;

            getMemberByEvaluator(vm.filter.periodIds, vm.filter.groupIds);
        }
    };

    vm.query = function () {
        vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');

        getEPAByEvaluator(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        $location.path(baseUrl + 'Evaluator/Detail/' + memberEPA.EPAId);
    };

    vm.viewEPAResult = function (memberEPA) {
        $location.path(baseUrl + 'EPA/Detail/' + memberEPA.EPAId);
    };

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupIds: '',
            employeeIds: '',
            periodIds: ''
        };
    }

    function getEPAByEvaluator(periodIds, groupIds, employeeIds) {
        lockScreenService.start();

        SelfEvalutorModel.getEPAByEvaluator(periodIds, groupIds, employeeIds).then(function () {
            vm.memberEPAs = SelfEvalutorModel.memberEPAs;
        })["finally"](function () {
            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

    function getMemberByEvaluator(periodIds, groupIds) {
        SelfEvalutorModel.getMemberByEvaluator(periodIds, groupIds).then(function () {
            vm.members = SelfEvalutorModel.members;

            utilitiesFactory.setSelectAll(vm.members, "EmployeeId");

            vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, "EmployeeId");
        });
    }

    function getGroupsByPeriods(periodIds) {
        SelfEvalutorModel.getGroupByEvaluator(periodIds).then(function () {
            vm.groupOptions = SelfEvalutorModel.groups;

            utilitiesFactory.setSelectAll(vm.groupOptions, "GroupId");

            vm.filter.groupIds = utilitiesFactory.getSelectedData(vm.groupOptions, "GroupId");

            getMemberByEvaluator(vm.filter.periodIds, vm.filter.groupIds);
        });
    }

    function initialize() {
        vm.filter = initFilter();

        CommonModel.getAllPeriodByEvaluator().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(CommonModel.periodOptions);

            vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, "PeriodId");

            getGroupsByPeriods(vm.filter.periodIds);

            vm.query();
        });
    }

    initialize();
}]);
