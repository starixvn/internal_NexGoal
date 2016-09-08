'use strict';
/*global epaModule*/

epaModule.controller('IndexEmployeeController',
["$location", 'SelfEvalutorModel', 'CommonModel', 'enumerationsFactory', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl',
function ($location, SelfEvalutorModel, CommonModel, enumerationsFactory, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
    var vm = this;

    vm.myEPAs = SelfEvalutorModel.myEPAs;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.periods = [];
    vm.filters = {};

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            periodIds: null
        };
    }

    vm.query = function () {
        vm.filters.periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, 'PeriodId');

        if (vm.filters.periodIds === null) {
            vm.myEPAs = [];
        } else {
            lockScreenService.start();

            SelfEvalutorModel.getEPAByEmployee(vm.filters.periodIds).then(function () {
                vm.myEPAs = SelfEvalutorModel.myEPAs;
            })["finally"](function () {
                lockScreenService.stop();
            });
        }

        historyFactory.saveState(vm.filter);
    };

    vm.viewEPADetail = function (epa) {
        $location.path(baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId);
    };

    vm.viewEPAResult = function (epa) {
        if (epa.HasMonthlyEPA && epa.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            $location.path(baseUrl + 'EPA/SemiDetail/' + epa.EPAId);
        } else {
            $location.path(baseUrl + 'EPA/Detail/' + epa.EPAId);
        }
    };

    function initialize() {
        vm.filter = initFilter();

        CommonModel.getAllPeriodByEmployee().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(CommonModel.periodOptions);
            vm.query();
        });
    }

    initialize();
}]);
