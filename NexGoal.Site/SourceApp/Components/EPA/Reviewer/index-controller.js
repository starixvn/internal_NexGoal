'use strict';
/*global epaModule*/

epaModule.controller('IndexReviewerController',
['$translate', '$location', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', 'enumerationsFactory',
    'utilitiesFactory', 'notificationFactory', 'historyFactory', 'baseUrl', 'lockScreenService',
function ($translate, $location, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel, enumerationsFactory,
    utilitiesFactory, notificationFactory, historyFactory, baseUrl, lockScreenService) {
    var vm = this;
    var isFirstLoad = true;
    var isBackToList = false;

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
        if (filter) {
            isBackToList = true;
        } else {
            filter = {
                groupIds: '',
                employeeIds: '',
                periodIds: ''
            };
        }

        return filter;
    }

    function getMemberByReviewer(periodIds, groupIds) {
        ReviewerManagementModel.getMemberByReviewer(periodIds, groupIds).then(function () {
            if (periodIds === null || groupIds === null) {
                vm.members = [];
            } else {
                vm.members = ReviewerManagementModel.members;
                if (isBackToList) {
                    utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
                } else {
                    utilitiesFactory.setSelectAll(vm.members, "EmployeeId");

                    vm.filter.employeeIds = utilitiesFactory.getSelectedDataByProperty(vm.members, "EmployeeId");
                }
            }

            if (isFirstLoad) {
                isFirstLoad = false;
                vm.query();
            }
        });
    }

    function getGroupsByPeriods(periodIds) {
        if (periodIds === null) {
            vm.groupOptions = [];
            vm.members = [];
        }
        else {
            ReviewerManagementModel.getGroupByReviewer(periodIds).then(function () {
                vm.groupOptions = ReviewerManagementModel.groups;

                if (isBackToList) {
                    utilitiesFactory.setSelectedItem(vm.groupOptions, "GroupId", vm.filter.groupIds);
                } else {
                    utilitiesFactory.setSelectAll(vm.groupOptions, "GroupId");

                    vm.filter.groupIds = utilitiesFactory.getSelectedDataByProperty(vm.groupOptions, "GroupId");
                }

                getMemberByReviewer(periodIds, vm.filter.groupIds);
            });
        }
    }

    function resetSorting(field) {
        vm.sortBy = {
            name: field,
            ascending: false
        };
    }

    function sortByTeam(epaInfo1, epaInfo2) {
        if (epaInfo1.Employee.GroupCode < epaInfo2.Employee.GroupCode) {
            return -1;
        }

        if (epaInfo1.Employee.GroupCode > epaInfo2.Employee.GroupCode) {
            return 1;
        }

        return 0;
    }

    function sortByFinalScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.CurrentProcess.TotalScore ? parseFloat(epaInfo1.Process.CurrentProcess.TotalScore) : 0,
            tempb = epaInfo2.Process.CurrentProcess.TotalScore ? parseFloat(epaInfo2.Process.CurrentProcess.TotalScore) : 0;

        return tempa - tempb;
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        return tempa - tempb;
    }

    function sortByEvaluatorScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.AverageScoreByEvaluators,
            tempb = epaInfo2.Process.AverageScoreByEvaluators;

        return tempa - tempb;
    }

    function getEPAByReviewer(periodIds, groupIds, employeeIds, selectedEmployees) {
        vm.isEndProcessAll = false;
        resetSorting();
        lockScreenService.start();
        ReviewerManagementModel.getEPAByReviewer(periodIds, groupIds, employeeIds).then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (memberEPA.Process.Evaluators[0].IsEndProcess && memberEPA.Process.CurrentProcess.TotalScore.length === 0) {
                    memberEPA.Process.CurrentProcess.TotalScore =
                        utilitiesFactory.parseFloat(memberEPA.Process.AverageScoreByEvaluators + memberEPA.ToeicPolicyScore);
                }

                memberEPA.Process.Self.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.Self.TotalScore);
                memberEPA.Process.CurrentProcess.TotalScore =
                    memberEPA.Process.CurrentProcess.TotalScore !== ''
                    ? utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.TotalScore)
                    : utilitiesFactory.parseFloat(memberEPA.Process.AverageScoreByEvaluators + memberEPA.ToeicPolicyScore);

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
        })["finally"](function () {
            lockScreenService.stop();
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
            if (isBackToList) {
                utilitiesFactory.setSelectedItem(vm.periods, "PeriodId", vm.filter.periodIds);
            } else {
                if ($routeParams.periodId && parseInt($routeParams.periodId, 10) > 0) {
                    angular.forEach(vm.periods, function (period) {
                        if (period.PeriodId === parseInt($routeParams.periodId, 10)) {
                            period.ticked = true;
                        } else {
                            period.ticked = false;
                        }
                    });
                } else if (!utilitiesFactory.checkInProcessPeriod(vm.periods)) {
                    utilitiesFactory.checkLastestPeriods(vm.periods);
                }

                vm.filter.periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, "PeriodId");
            }

            getGroupsByPeriods(vm.filter.periodIds);
        });
    }

    vm.handleOnPeriodChange = function () {
        var periodIds = utilitiesFactory.getSelectedDataByProperty(vm.periods, "PeriodId");
        isBackToList = false;
        if (periodIds !== vm.filter.periodIds) {
            vm.filter.periodIds = periodIds;

            getGroupsByPeriods(periodIds);
        }
    };

    vm.handleOnGroupChange = function () {
        var groupIds = utilitiesFactory.getSelectedDataByProperty(vm.groupOptions, "GroupId");
        isBackToList = false;
        if (groupIds !== vm.filter.groupIds) {
            vm.filter.groupIds = groupIds;

            getMemberByReviewer(vm.filter.periodIds, vm.filter.groupIds);
        }
    };

    vm.selectedCls = function (field) {
        var cssClass = '';
        if (vm.sortBy.name === field && vm.sortBy.ascending) {
            cssClass = 'sorting-asc';
        } else if (vm.sortBy.name === field && !vm.sortBy.ascending) {
            cssClass = 'sorting-desc';
        }

        return cssClass;
    };

    vm.changeSorting = function (field) {
        switch (field) {
            case 'Team':
                if (vm.sortBy.name === field && vm.sortBy.ascending) {
                    vm.memberEPAs.reverse();
                } else {
                    vm.memberEPAs.sort(sortByTeam);
                }
                break;
            case 'FinalScore':
                if (vm.sortBy.name === field && vm.sortBy.ascending) {
                    vm.memberEPAs.reverse();
                } else {
                    vm.memberEPAs.sort(sortByFinalScore);
                }
                break;
            case 'SelfScore':
                if (vm.sortBy.name === field && vm.sortBy.ascending) {
                    vm.memberEPAs.reverse();
                } else {
                    vm.memberEPAs.sort(sortBySelfScore);
                }

                break;
            case 'EvaluatorScore':
                if (vm.sortBy.name === field && vm.sortBy.ascending) {
                    vm.memberEPAs.reverse();
                } else {
                    vm.memberEPAs.sort(sortByEvaluatorScore);
                }
                break;
        }

        vm.sortBy.name = field;
        vm.sortBy.ascending = !vm.sortBy.ascending;
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
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore,
        adjustScore = memberEPA.Process.CurrentProcess.AdjustScore.length === 0 ? 0 : memberEPA.Process.CurrentProcess.AdjustScore;

        if (adjustScore > maxAdditionScore) {
            notificationFactory.error('AdjustScoreInvalid');
            memberEPA.Process.CurrentProcess.AdjustScore = maxAdditionScore;
        } else if (adjustScore < maxSubtractScore) {
            notificationFactory.error('AdjustScoreInvalid');
            memberEPA.Process.CurrentProcess.AdjustScore = maxSubtractScore;
        }

        ReviewerManagementModel.calculateScore(memberEPA, true);
    };

    vm.adjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("NoEmployeeSelectedWarning");
        } else {
            ReviewerManagementModel.openPopupAdjustScore(vm, selectedEmployees, true)
            .then(function (adjust) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(selectedEmployees, adjust);
                ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("NoEmployeeSelectedWarning");
        } else {
            var isValidComment = ReviewerManagementModel.validateComments(selectedEmployees);
            if (!isValidComment) {
                notificationFactory.error('CommentRequire');
            } else {
                ReviewerManagementModel.calculateAdjustScore(selectedEmployees);
                ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees)['finally'](function () {
                    getEPAByReviewer(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds, selectedEmployees);
                });
            }
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, true)
        .then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].Process.CurrentProcess.Comment = commentUpdate;
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    vm.query = function () {
        vm.filter.employeeIds = utilitiesFactory.getSelectedDataByProperty(vm.members, 'EmployeeId');
        if (vm.filter.periodIds === null || vm.filter.groupIds === null || vm.filter.employeeIds === null) {
            vm.memberEPAs = [];
        } else {
            getEPAByReviewer(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds);
            vm.selectedAll = false;
        }
    };

    initialize();
}]);