'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
['$translate', '$location', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', 'enumerationsFactory',
    'utilitiesFactory', 'notificationFactory', 'historyFactory', 'baseUrl',
function ($translate, $location, EmployeeEPAModel, ReviewerManagementModel, CommonModel, enumerationsFactory,
    utilitiesFactory, notificationFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.permission = 0;
    vm.memberEPAs = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.averageScore = 0;
    vm.isEndProcessAll = false;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.monthlyFilterSelected = false;
    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();

        return filter ? filter : {
            groupId: '',
            isLocal: '',
            isMonthly: false,
            employeeIds: ''
        };
    }

    vm.getMember = function () {
        ReviewerManagementModel.getMemberByReviewer(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly).then(function () {
            vm.members = ReviewerManagementModel.members;
            utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
        });
    };

    vm.selectedCls = function (field) {
        var cssClass = '';

        switch (field) {
            case 'SelfScore':
                if (vm.sortBy.selfScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.selfScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'EvaluatorScore':
                if (vm.sortBy.evaluatorScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.evaluatorScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
        }

        return cssClass;
    };

    function resetSorting() {
        vm.sortBy = {
            selfScore: {
                ascending: false, descending: false
            },
            evaluatorScore: {
                ascending: false, descending: false
            }
        };
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        if (epaInfo1.HasMonthlyEPA && epaInfo1.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempa = epaInfo1.MonthlyEPA.AverageSelfScoreConverted;
        }

        if (epaInfo2.HasMonthlyEPA && epaInfo2.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempb = epaInfo2.MonthlyEPA.AverageSelfScoreConverted;
        }

        return tempa - tempb;
    }

    function sortByEvaluatorScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.AverageScoreByEvaluators,
            tempb = epaInfo2.Process.AverageScoreByEvaluators;

        if (epaInfo1.HasMonthlyEPA && epaInfo1.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempa = epaInfo1.MonthlyEPA.AverageEvaluatorScoreConverted;
        }

        if (epaInfo2.HasMonthlyEPA && epaInfo2.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempb = epaInfo2.MonthlyEPA.AverageEvaluatorScoreConverted;
        }

        return tempa - tempb;
    }

    vm.changeSorting = function (field) {
        switch (field) {
            case 'SelfScore':
                vm.sortBy.evaluatorScore = {
                    ascending: false,
                    descending: false
                };
                if (!vm.sortBy.selfScore.ascending) {
                    vm.sortBy.selfScore.ascending = true;
                    vm.sortBy.selfScore.descending = false;
                    vm.memberEPAs.sort(sortBySelfScore);
                } else {
                    vm.sortBy.selfScore.ascending = false;
                    vm.sortBy.selfScore.descending = true;
                    vm.memberEPAs.reverse();
                }
                break;
            case 'EvaluatorScore':
                vm.sortBy.selfScore = {
                    ascending: false,
                    descending: false
                };
                if (!vm.sortBy.evaluatorScore.ascending) {
                    vm.sortBy.evaluatorScore.ascending = true;
                    vm.sortBy.evaluatorScore.descending = false;
                    vm.memberEPAs.sort(sortByEvaluatorScore);
                } else {
                    vm.sortBy.evaluatorScore.ascending = false;
                    vm.sortBy.evaluatorScore.descending = true;
                    vm.memberEPAs.reverse();
                }
                break;
        }
    };

    vm.viewEPADetail = function (memberEPA) {
        if (ReviewerManagementModel.checkAdjustScoreChanged(vm.memberEPAs)) {
            ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                $location.path(baseUrl + 'Reviewer/Detail/' + memberEPA.EPAId);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        } else {
            if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                $location.path(baseUrl + 'Reviewer/SemiDetail/' + memberEPA.EPAId);
            } else {
                $location.path(baseUrl + 'Reviewer/Detail/' + memberEPA.EPAId);
            }
        }
    };

    vm.viewEPAResult = function (memberEPA) {
        if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            $location.path(baseUrl + 'EPA/SemiDetail/' + memberEPA.EPAId);
        } else {
            $location.path(baseUrl + 'EPA/Detail/' + memberEPA.EPAId);
        }
    };

    vm.viewOtherViolationDetail = function (memberEPA) {
        CommonModel.openPopupViewOtherViolationDetail(memberEPA.EPAId);
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.memberEPAs, function (memberEPA) {
            if (!memberEPA.Process.CurrentProcess.IsEndProcess) {
                memberEPA.Selected = selectedAll;
            } else {
                memberEPA.Selected = false;
            }
        });
    };

    vm.getNotificationInputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore;

        return EmployeeEPAModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore;

        if (!EmployeeEPAModel.validateScoreInput(memberEPA.Process.CurrentProcess.AdjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            memberEPA.Process.CurrentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(memberEPA, true);
    };

    vm.adjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        } else {
            ReviewerManagementModel.openPopupAdjustScore(vm, selectedEmployees, true)
            .then(function (adjust) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(selectedEmployees, adjust, true);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeSaveWarning");
        } else {
            ReviewerManagementModel.calculateAdjustScore(selectedEmployees, true);
            ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees)['finally'](function () {
                getEPAByReviewer(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly, vm.filter.employeeIds, selectedEmployees);
            });
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, true)
        .then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].Process.CurrentProcess.TotalScore = vm.memberEPAs[index].Process.CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].Process.CurrentProcess.AdjustScore = vm.memberEPAs[index].Process.CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].Process.CurrentProcess.Comment = commentUpdate;
                vm.memberEPAs[index].Process.CurrentProcess.Evaluated = true;

                ReviewerManagementModel.calculateScore(vm.memberEPAs[index], true);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    function getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, selectedEmployees) {
        vm.isEndProcessAll = false;
        resetSorting();

        ReviewerManagementModel.getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds).then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var averageScore = 0, countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (memberEPA.Process.Evaluators[0].IsEndProcess && !memberEPA.Process.CurrentProcess.TotalScore) {
                    memberEPA.Process.CurrentProcess.TotalScore = memberEPA.Process.AverageScoreByEvaluators;
                }

                memberEPA.Process.Self.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.Self.TotalScore);
                memberEPA.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.TotalScore);
                memberEPA.FinalScore = memberEPA.Process.CurrentProcess.TotalScore + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.Process.CurrentProcess.MaxPerformanceScore);

                if (memberEPA.LastFinalScore.length > 0) {
                    memberEPA.LastFinalScore = utilitiesFactory.parseFloat(memberEPA.LastFinalScore);
                } else {
                    memberEPA.LastFinalScore = null;
                }

                if (memberEPA.Process.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }
                averageScore += memberEPA.Process.AverageScoreByEvaluators;
            });

            if (countEndProcess === vm.memberEPAs.length) {
                vm.isEndProcessAll = true;
            }
            vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.memberEPAs.length);

            if (selectedEmployees && selectedEmployees.length > 0) {
                ReviewerManagementModel.updateSelectedEmployees(vm.memberEPAs, selectedEmployees);
            }

            $(".rating-loading").rating();
            if (isMonthly) {
                vm.monthlyFilterSelected = true;
            } else {
                vm.monthlyFilterSelected = false;
            }
        });

        historyFactory.saveState(vm.filter);
    }

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(filter.groupId, filter.isLocal, filter.isMonthly, filter.employeeIds);
    };

    function initialize() {
        vm.filter = initFilter();
        resetSorting();

        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('AdjustScoreInvalid').then(function (message) {
            vm.adjustScoreInvalidMessage = message;
        });

        ReviewerManagementModel.getUserPermissionOnReviewer().then(function () {
            vm.permission = ReviewerManagementModel.permission;
        });

        ReviewerManagementModel.getGroupByReviewer().then(function () {
            vm.groupOptions = ReviewerManagementModel.groups;
        });

        vm.getMember();

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAByReviewer(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly, vm.filter.employeeIds);
        });
    }

    initialize();
}]);

epaModule.controller('MemberEPAInfoOfReviewerController',
['$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', "SelfEvalutorModel",
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl', 'historyFactory',
function ($timeout, $translate, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel, SelfEvalutorModel,
    utilitiesFactory, enumerationsFactory, baseUrl, historyFactory) {
    var vm = this;

    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id, 10);
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    vm.dataSaved = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;
    vm.maxStar = utilitiesFactory.maxStar;

    vm.checkEndProcess = function (members) {
        var endProcess = true;

        angular.forEach(members, function (member) {
            if (!member.IsEndProcess) {
                endProcess = false;
            }
        });

        return endProcess;
    };

    vm.convertScoreToGrade = function (score) {
        return EmployeeEPAModel.convertScoreToGrade(score, vm.standardGrades);
    };

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.getNotificationInputScore = function (currentProcess) {
        var message = "";

        if (currentProcess) {
            var maxSubtractScore = currentProcess.MaxSubtractScore,
                maxAdditionScore = currentProcess.MaxAdditionScore;

            message = EmployeeEPAModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
        }

        return message;
    };

    vm.inputScore = function (currentProcess) {
        var maxSubtractScore = currentProcess.MaxSubtractScore,
            maxAdditionScore = currentProcess.MaxAdditionScore;

        if (!EmployeeEPAModel.validateScoreInput(currentProcess.AdjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            currentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(vm.epaInfo, true);
        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(currentProcess.TotalScore, vm.standardGrades);
    };

    vm.save = function (epaInfo) {
        var totalcore = epaInfo.Process.CurrentProcess.TotalScore;

        if (ReviewerManagementModel.validateEvaluation(totalcore)) {
            var model = new Array(epaInfo);

            ReviewerManagementModel.adjustScoreByReviewer(vm, model);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    function CalculateEvaluatorScore(evaluators) {
        var averageScore = 0;

        if (evaluators.length > 0) {
            var score = 0;
            var length = 0;
            angular.forEach(evaluators, function (evaluator) {
                if (evaluator.ScoreEncode) {
                    score += evaluator.Score;
                    length++;
                }
            });

            averageScore = score / length;
        }

        return utilitiesFactory.splitNumber(averageScore, 2);
    }

    function getEPAInfoByReviewer() {
        ReviewerManagementModel.getEPAInfoByReviewer(vm.epaId).then(function () {
            vm.epaInfo = ReviewerManagementModel.epaInfo;
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

            var totalScoreOfEvaluator = 0;
            for (var index = 0; index < vm.epaInfo.KPIGroups.length; index++) {
                var kpiGroup = vm.epaInfo.KPIGroups[index];
                var selfScore = 0;
                var evaluatorScore = 0;

                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    kpi.EvaluatorScore = CalculateEvaluatorScore(kpi.EvaluationByEvaluators);
                    selfScore += kpi.EvaluationBySelf.Score;
                    evaluatorScore += kpi.EvaluatorScore;
                });

                kpiGroup.SelfScore = selfScore;
                kpiGroup.EvaluatorScore = evaluatorScore;
                totalScoreOfEvaluator += evaluatorScore;
            }

            // self
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

            // evaluator
            angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });
            vm.epaInfo.Process.AverageScoreByEvaluators = totalScoreOfEvaluator;

            // reviewer
            vm.epaInfo.Process.CurrentProcess.MaxSubtractScore = -totalScoreOfEvaluator;
            vm.epaInfo.Process.CurrentProcess.MaxAdditionScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore - totalScoreOfEvaluator);
            vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.Process.CurrentProcess.TotalScore !== "" ?
                                                            utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore) :
                                                            utilitiesFactory.parseFloat(vm.epaInfo.Process.AverageScoreByEvaluators);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.epaInfo.Process.CurrentProcess.TotalScore, vm.standardGrades);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            ReviewerManagementModel.getUserPermissionOnReviewer().then(function () {
                vm.permission = ReviewerManagementModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                }
            });

            SelfEvalutorModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });

            SelfEvalutorModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.employeeProjects = SelfEvalutorModel.employeeProjects;
            });
        });
    }

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByReviewer();
        });
    }

    initialize();
}]);