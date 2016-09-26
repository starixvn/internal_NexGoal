'use strict';
/*global epaModule*/

epaModule.factory('ReviewerManagementModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var reviewerMngModel = this;
    var maxAdditionScore = utilitiesFactory.maxAdditionScore;
    var maxSubtractScore = utilitiesFactory.maxSubtractScore;

    reviewerMngModel.epaInfo = {};
    reviewerMngModel.groups = [];
    reviewerMngModel.members = [];
    reviewerMngModel.memberEPAs = [];
    reviewerMngModel.permission = 0;
    reviewerMngModel.periodOptions = [];

    reviewerMngModel.getAllPeriodByReviewer = function () {
        return employeeEPAFactory.getAllPeriodByReviewer().success(function (periods) {
            reviewerMngModel.periodOptions = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // common
    reviewerMngModel.getSelectedEmployee = function (memberEPAs) {
        var result = [];

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                result.push(memberEPA);
            }
        });

        return result;
    };

    reviewerMngModel.openPopupAdjustScore = function (vm, selectedEmployees, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalAdjustScore.html',
            controller: "ModalAdjustScoreController as modalAdjustScoreCtrl",
            backdrop: 'static',
            resolve: {
                selectedEmployees: function () {
                    return angular.copy(selectedEmployees);
                },
                isReviewer: function () {
                    return isReviewer;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.openPopupUpdateComment = function (vm, memberEPA, comment, isEdit, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalUpdateComment.html',
            controller: "ModalUpdateCommentController as modalUpdateCommentCtrl",
            backdrop: 'static',
            resolve: {
                memberEPA: function () {
                    return angular.copy(memberEPA);
                },
                comment: function () {
                    return comment;
                },
                isEdit: function () {
                    return isEdit;
                },
                isReviewer: function () {
                    return isReviewer;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.checkAdjustScoreChanged = function (memberEPAs) {
        var hasChange = false;

        for (var i = 0; i < memberEPAs.length; i++) {
            var adjustScoreOriginal = utilitiesFactory.parseFloat(memberEPAs[i].Process.CurrentProcess.AdjustScoreOriginal),
                adjustScore = utilitiesFactory.parseFloat(memberEPAs[i].Process.CurrentProcess.AdjustScore);

            if (adjustScoreOriginal !== adjustScore) {
                hasChange = true;

                break;
            }
        }

        return hasChange;
    };

    reviewerMngModel.openPopupRemindSaveChange = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalRemindSaveChange.html',
            controller: "ModalRemindController as modalRemindCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.validateEvaluation = function (score) {
        if (!score || parseFloat(score) < 0) {
            notificationFactory.error("ScoreRequire");
            return false;
        }

        return true;
    };

    reviewerMngModel.calculateScore = function (memberEPA, isReviewer) {
        var scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.AdjustScore),
            averageScore = memberEPA.Process.AverageScoreByEvaluators,
            totalScore = 0;

        if (memberEPA.HasMonthlyEPA) {
            averageScore = memberEPA.MonthlyEPA.AverageReviewerScoreConverted;
        }

        if (!isReviewer && memberEPA.Process.Reviewers.length > 0) {
            averageScore = memberEPA.Process.AverageScoreByReviewers;
        }

        totalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);

        if (totalScore > memberEPA.Process.CurrentProcess.MaxTotalScore) {
            scoreAdjusted = memberEPA.Process.CurrentProcess.MaxTotalScore - averageScore;
        } else if (totalScore < 0) {
            scoreAdjusted = -averageScore;
        }

        memberEPA.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);
        memberEPA.FinalScore = averageScore + scoreAdjusted + memberEPA.DisciplineScore;
        memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.Process.CurrentProcess.MaxPerformanceScore);
    };

    reviewerMngModel.calculatePerformanceScore = function (vm, isReviewer) {
        var totalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore),
            averageScore = vm.epaInfo.Process.AverageScoreByEvaluators;

        if (vm.epaInfo.HasMonthlyEPA) {
            averageScore = vm.epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
        }

        if (!isReviewer && vm.epaInfo.Process.Reviewers.length > 0) {
            averageScore = vm.epaInfo.Process.AverageScoreByReviewers;
        }

        vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

        vm.epaInfo.Process.CurrentProcess.AdjustScore = utilitiesFactory.parseFloat(totalScore - averageScore);
        vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal = vm.epaInfo.Process.CurrentProcess.AdjustScore;
        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    reviewerMngModel.validateComment = function (memberEPA) {
        var isValid = true;

        if (!memberEPA.Process.CurrentProcess.Comment || memberEPA.Process.CurrentProcess.Comment.length === 0) {
            isValid = false;
            memberEPA.Process.CurrentProcess.cssClass = "note";
        } else {
            memberEPA.Process.CurrentProcess.cssClass = "";
        }

        return isValid;
    };

    reviewerMngModel.validateComments = function (memberEPAs) {
        var isValid = true;
        angular.forEach(memberEPAs, function (memberEPA) {
            isValid = isValid ? reviewerMngModel.validateComment(memberEPA) : isValid;
        });

        return isValid;
    };

    reviewerMngModel.calculateAdjustScore = function (memberEPAs, isReviewer) {
        angular.forEach(memberEPAs, function (memberEPA) {
            var scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.AdjustScore),
                averageScore = memberEPA.Process.AverageScoreByEvaluators,
                totalScore = 0;

            if (!isReviewer && memberEPA.Process.Reviewers.length > 0) {
                averageScore = memberEPA.Process.AverageScoreByReviewers;
            }

            totalScore = utilitiesFactory.parseFloat(scoreAdjusted + averageScore);

            if (totalScore > memberEPA.Process.CurrentProcess.MaxTotalScore) {
                scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.MaxTotalScore - averageScore);
            } else if (totalScore < 0) {
                scoreAdjusted = -averageScore;
            }

            memberEPA.Process.CurrentProcess.AdjustScore = scoreAdjusted;
        });
    };

    reviewerMngModel.calculateAdjustScoreMultiple = function (memberEPAs, adjust, isReviewer) {
        var scoreAdjusted, averageScore, totalScore;
        var isValidAdjustScore = true;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
                    maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore,
                    adjustScore = adjust.Score === "" ? 0 : adjust.Score;

                if (adjustScore > maxAdditionScore) {
                    memberEPA.Process.CurrentProcess.AdjustScore = maxAdditionScore;
                } else if (adjustScore < maxSubtractScore) {
                    memberEPA.Process.CurrentProcess.AdjustScore = maxSubtractScore;
                } else {
                    memberEPA.Process.CurrentProcess.AdjustScore = adjustScore;
                }

                reviewerMngModel.calculateScore(memberEPA, true);

                memberEPA.Process.CurrentProcess.Comment = adjust.Comment;
            }
        });
    };

    // reviewer
    reviewerMngModel.getUserPermissionOnReviewer = function () {
        return employeeEPAFactory.getUserPermissionOnReviewer()
        .success(function (permission) {
            reviewerMngModel.permission = parseInt(permission, 10);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getGroupByReviewer = function (periodIds) {
        return employeeEPAFactory.getGroupByReviewer(periodIds)
        .success(function (groups) {
            reviewerMngModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getMemberByReviewer = function (periodIds, groupIds) {
        return employeeEPAFactory.getMemberByReviewer(periodIds, groupIds)
        .success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByReviewer = function (periodIds, groupIds, employeeIds) {
        return employeeEPAFactory.getEPAByReviewer(periodIds, groupIds, employeeIds)
        .success(function (memberEPAs) {
            reviewerMngModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAInfoByReviewer = function (epaId) {
        return employeeEPAFactory.getEPAInfoByReviewer(epaId)
        .success(function (epaInfo) {
            reviewerMngModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.adjustScoreByReviewer = function (vm, memberEPAs) {
        return employeeEPAFactory.adjustScoreByReviewer(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                vm.dataSaved = true;
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.updateSelectedEmployees = function (memberEPAs, selectedEmployees) {
        angular.forEach(memberEPAs, function (memberEPA) {
            if (!memberEPA.Process.CurrentProcess.IsEndProcess) {
                angular.forEach(selectedEmployees, function (selectedEmployee) {
                    if (memberEPA.EPAId === selectedEmployee.EPAId) {
                        memberEPA.Selected = true;
                    }
                });
            }
        });
    };

    return reviewerMngModel;
}]);