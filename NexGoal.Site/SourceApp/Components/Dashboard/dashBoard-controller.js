"use strict";

(function (dashboardModule) {
    dashboardModule.controller("DashboardController", dashboardController);

    dashboardController.$inject = ["$interval", "$location", 'DashboardModel', 'CommonModel',
        'utilitiesFactory', 'enumerationsFactory', 'lockScreenService', 'baseUrl', 'EmployeeEPAModel'];

    function dashboardController($interval, $location, DashboardModel, CommonModel,
        utilitiesFactory, enumerationsFactory, lockScreenService, baseUrl, EmployeeEPAModel) {
        var vm = this;

        var stopInterval;
        vm.showScore = false;

        vm.epaStatusEnum = enumerationsFactory.epaStatus;
        vm.targetTypeEnum = enumerationsFactory.targetType;
        vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
        vm.selfEvaluatorPermission = 0;
        vm.policy = enumerationsFactory.policy;
        vm.processType = enumerationsFactory.processType;
        vm.nominationRole = {
            Nominator: 1,
            Committee: 2
        };

        vm.getUserPermissionOnEmployee = function () {
            EmployeeEPAModel.getUserPermissionOnEmployee().then(function () {
                vm.selfEvaluatorPermission = EmployeeEPAModel.selftEvaluatorPermission;
            });
        };

        vm.showHideScore = function () {
            vm.showScore = !vm.showScore;

            if (vm.showScore) {
                stopInterval = $interval(function () {
                    vm.showScore = false;
                    $interval.cancel(stopInterval);
                }, 30000);
            }
        };

        vm.getClassProgress = function (statusEndProcess) {
            if (statusEndProcess === vm.statusEndProcessEnum.InProgress) {
                return 'in-process';
            }
            else if (statusEndProcess === vm.statusEndProcessEnum.EndProcess) {
                return 'end-process';
            }
            else {
                return 'open';
            }
        };

        vm.viewEPADetail = function (epa) {
            $location.path(baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId);
        };

        vm.viewEPAResult = function (epa) {
            $location.path(baseUrl + 'EPA/Detail/' + epa.EPAId);
        };

        vm.viewOtherViolationDetail = function (epa) {
            CommonModel.openPopupViewOtherViolationDetail(epa.EPAId);
        };

        vm.redirectToPAFormDetail = function (epaInfo) {
            var path = '';

            if (epaInfo.ProcessId === vm.processType.Evaluator) {
                path = 'Evaluator/Detail/';
            } else {
                path = 'Reviewer/Detail/';
            }

            $location.path(baseUrl + path + epaInfo.EPAId);
        };

        vm.redirectToPAForm = function (epaInfo) {
            var path = '';

            if (epaInfo.ProcessId === vm.processType.Evaluator) {
                path = 'Evaluator/Index';
            } else {
                path = 'Reviewer/Index';
            }

            $location.path(baseUrl + path).search({ periodId: epaInfo.PeriodId });
        };

        vm.redirectToNomination = function (nomination) {
            var path = '';

            if (nomination.IsQuarter) {
                path = 'EOQ/Index';
            } else {
                path = 'EOY/Index';
            }

            $location.path(baseUrl + path);
        };

        vm.redirectToPAFocus = function () {
            $location.path(baseUrl + 'PAFocus/Index');
        };

        function initialize() {
            lockScreenService.start();

            vm.getUserPermissionOnEmployee();

            DashboardModel.getDashboard().then(function () {
                var dashboardInfo = DashboardModel.dashboardInfo;

                vm.evaluationProcesses = [];
                vm.myEPAs = dashboardInfo.MyEPAs;
                vm.toDoList = dashboardInfo.ToDoList;

                angular.forEach(vm.myEPAs, function (myEPA) {
                    if (myEPA.Period.IsEndPeriod && myEPA.FinalScore.length > 0) {
                        myEPA.FinalScore = utilitiesFactory.parseFloat(myEPA.FinalScore);
                    } else {
                        myEPA.FinalScore = null;
                        myEPA.Grade = null;
                        vm.evaluationProcesses.push(myEPA);
                    }
                });
            })["finally"](function () {
                lockScreenService.stop();
            });
        }

        initialize();
    }
})(angular.module("dashboardModule"));