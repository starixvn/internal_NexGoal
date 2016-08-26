'use strict';
/*global epaModule*/

epaModule.controller('IndexReviewerController',
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
    vm.groupOptions = [];
    vm.periods = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.isEndProcessAll = false;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();

        return filter ? filter : {
            groupIds: '',
            employeeIds: '',
            periodIds: ''
        };
    }

    function getMemberByReviewer(periodIds, groupIds) {
        ReviewerManagementModel.getMemberByReviewer(periodIds, groupIds).then(function () {
            vm.members = ReviewerManagementModel.members;

            utilitiesFactory.setSelectAll(vm.members, "EmployeeId");

            vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, "EmployeeId");
        });
    }

    function getGroupsByPeriods(periodIds) {
        ReviewerManagementModel.getGroupByReviewer(periodIds).then(function () {
            vm.groupOptions = ReviewerManagementModel.groups;

            utilitiesFactory.setSelectAll(vm.groupOptions, "GroupId");

            vm.filter.groupIds = utilitiesFactory.getSelectedData(vm.groupOptions, "GroupId");

            getMemberByReviewer(vm.filter.periodIds, vm.filter.groupIds);
        });
    }

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

    function CalculateAverageEvaluator(Evaluators) {
        var totalScore = 0, count = 0;

        angular.forEach(Evaluators, function (evaluator) {
            if (evaluator.TotalScore) {
                totalScore += utilitiesFactory.parseFloat(evaluator.TotalScore);
                count++;
            }
        })

        return count > 0 ? utilitiesFactory.splitNumber(totalScore / count, 2) : 0;
    }

    function getEPAByReviewer(periodIds, groupIds, employeeIds, selectedEmployees) {
        vm.isEndProcessAll = false;
        resetSorting();

        ReviewerManagementModel.getEPAByReviewer(periodIds, groupIds, employeeIds).then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var averageScoreEvaluators = CalculateAverageEvaluator(memberEPA.Process.Evaluators);

                memberEPA.Process.AverageScoreByEvaluators = averageScoreEvaluators;
                if (memberEPA.Process.Evaluators[0].IsEndProcess && !memberEPA.Process.CurrentProcess.TotalScore) {
                    memberEPA.Process.CurrentProcess.TotalScore = memberEPA.Process.AverageScoreByEvaluators;
                }

                memberEPA.Process.CurrentProcess.MaxSubtractScore = -10;
                memberEPA.Process.CurrentProcess.MaxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore > 10 ?
                    10 :
                    memberEPA.Process.CurrentProcess.MaxAdditionScore;

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
            });

            if (countEndProcess === vm.memberEPAs.length) {
                vm.isEndProcessAll = true;
            }

            if (selectedEmployees && selectedEmployees.length > 0) {
                ReviewerManagementModel.updateSelectedEmployees(vm.memberEPAs, selectedEmployees);
            }

            $(".rating-loading").rating();
        });

        historyFactory.saveState(vm.filter);
    }

    function initialize() {
        vm.filter = initFilter();
        resetSorting();

        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        ReviewerManagementModel.getUserPermissionOnReviewer().then(function () {
            vm.permission = ReviewerManagementModel.permission;
        });

        ReviewerManagementModel.getAllPeriodByReviewer().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(ReviewerManagementModel.periodOptions);

            vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, "PeriodId");

            getGroupsByPeriods(vm.filter.periodIds);

            vm.query();
        });
    }

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

            getMemberByReviewer(vm.filter.periodIds, vm.filter.groupIds);
        }
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
            $location.path(baseUrl + 'Reviewer/Detail/' + memberEPA.EPAId);
        }
    };

    vm.viewEPAResult = function (memberEPA) {
        $location.path(baseUrl + 'EPA/Detail/' + memberEPA.EPAId);
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

    vm.getNotificationInputScore = function () {
        return vm.scoreInputValidMessage;
    };

    vm.inputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore;
        var adjustScore = memberEPA.Process.CurrentProcess.AdjustScore;

        if (adjustScore > maxAdditionScore || adjustScore < maxSubtractScore) {
            notificationFactory.error('AdjustScoreInvalid');

            if (adjustScore > maxAdditionScore) {
                memberEPA.Process.CurrentProcess.AdjustScore = maxAdditionScore;
            } else if (adjustScore < maxSubtractScore) {
                memberEPA.Process.CurrentProcess.AdjustScore = maxSubtractScore;
            }
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
                ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees);
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
            var isValidComment = ReviewerManagementModel.validateComments(selectedEmployees);
            if (!isValidComment) {
                notificationFactory.error('CommentRequire');
            } else {
                ReviewerManagementModel.calculateAdjustScore(selectedEmployees, true);
                ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees)['finally'](function () {
                    getEPAByReviewer(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds);
                });
            }
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

    vm.query = function () {
        vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds);
    };

    initialize();
}]);
