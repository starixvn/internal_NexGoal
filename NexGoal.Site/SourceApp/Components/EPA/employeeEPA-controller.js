'use strict';
/*global epaModule*/

epaModule.controller('ViewEPADetailController',
['$window', '$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', "SelfEvalutorModel",
    'ViolationModel', 'NominationModel', 'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl', 'lockScreenService',
function ($window, $timeout, $translate, $routeParams, EmployeeEPAModel, CommonModel, SelfEvalutorModel,
ViolationModel, NominationModel, utilitiesFactory, enumerationsFactory, historyFactory, baseUrl, lockScreenService) {
    var vm = this;

    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.violations = [];
    vm.epaId = parseInt($routeParams.id, 10);
    vm.exportPDF = $routeParams.exportpdf;
    vm.standardGrades = CommonModel.standardGrades;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.back = function () {
        historyFactory.back();
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.exportPDF = function (epaId) {
        var iframe = $('body iframe');
        var url = baseUrl + 'ExportPDF/ProbationDetail?epaId=' + epaId;

        if (iframe.length === 0) {
            iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
            iframe.appendTo('body');
        }

        iframe.attr('src', url);
    };

    function getIntegrationInformation() {
        if (vm.epaInfo.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            SelfEvalutorModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });

            SelfEvalutorModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.employeeProjects = SelfEvalutorModel.employeeProjects;
            });

        } else {
            SelfEvalutorModel.getHRMSummaryOnProbation(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });
        }
    }

    function getEPAInfoById() {
        lockScreenService.start();
        EmployeeEPAModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;

            // self
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

            // evaluator
            angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });

            // reviewer
            angular.forEach(vm.epaInfo.Process.Reviewers, function (reviewer) {
                reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
            });

            if (vm.epaInfo.Period.IsEndPeriod && vm.epaInfo.FinalScore.length > 0) {
                vm.epaInfo.FinalScore = utilitiesFactory.parseFloat(vm.epaInfo.FinalScore);
                vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.epaInfo.FinalScore, vm.standardGrades);
            } else {
                vm.epaInfo.FinalScore = null;
            }

            getIntegrationInformation();

            ViolationModel.getViolationsByPeriod(vm.epaInfo.Period.PeriodId, vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.violations = ViolationModel.violationsByPeriod;
            });

            NominationModel.getNominationByPeriodId(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.employeeNominations = NominationModel.employeeNominations;
            });
        })["finally"](function () {
            lockScreenService.stop();
        });
    }

    function initialize() {
        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoById();
        });
    }

    initialize();
}]);

epaModule.controller('ModalViewCommentController',
['$modalInstance', 'comment',
function ($modalInstance, comment) {
    var vm = this;

    vm.comment = comment;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalAdjustScoreController',
['$translate', '$modalInstance', 'EmployeeEPAModel', 'ReviewerManagementModel', 'utilitiesFactory', 'notificationFactory',
    'selectedEmployees', 'isReviewer', 'minScore', 'maxScore',
function ($translate, $modalInstance, EmployeeEPAModel, ReviewerManagementModel, utilitiesFactory, notificationFactory,
    selectedEmployees, isReviewer, minScore, maxScore) {
    var vm = this;

    vm.adjust = {
        Score: '',
        Comment: ''
    };

    vm.selectedEmployees = selectedEmployees;
    vm.isReviewer = isReviewer;
    vm.scoreInputValidMessage = "";

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });
    }

    vm.ok = function () {
        if (!vm.adjust.Score) {
            notificationFactory.error("ScoreRequire");
        } else if (!vm.adjust.Comment || vm.adjust.Comment.length === 0) {
            notificationFactory.error("CommentRequire");
        }
        else {
            $modalInstance.close(vm.adjust);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    vm.changedScore = function () {
        if (vm.adjust.Score < minScore) {
            vm.adjust.Score = minScore;
        }

        if (vm.adjust.Score > maxScore) {
            vm.adjust.Score = maxScore;
        }
    };

    initialize();
}]);

epaModule.controller('ModalUpdateCommentController',
['$modalInstance', 'EmployeeEPAModel', 'ReviewerManagementModel', 'utilitiesFactory', 'memberEPA', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, ReviewerManagementModel, utilitiesFactory, memberEPA, comment, isEdit, isReviewer) {
    var vm = this;

    vm.comment = comment;
    vm.isEdit = isEdit;
    vm.isReviewer = isReviewer;
    vm.memberEPA = memberEPA;

    vm.ok = function () {
        var model = [], scoreAdjusted = utilitiesFactory.parseFloat(vm.memberEPA.Process.CurrentProcess.AdjustScoreOriginal),
            averageScore = vm.memberEPA.Process.AverageScoreByEvaluators,
            totalScore;

        if (!vm.isReviewer && vm.memberEPA.Process.Reviewers.length > 0) {
            averageScore = vm.memberEPA.Process.AverageScoreByReviewers;
        }

        if (vm.memberEPA.HasDisciplineScore) {
            averageScore -= vm.memberEPA.DisciplineScore;
        }

        totalScore = averageScore + scoreAdjusted;
        vm.memberEPA.Process.CurrentProcess.TotalScore = totalScore;
        vm.memberEPA.Process.CurrentProcess.AdjustScore = scoreAdjusted;
        vm.memberEPA.Process.CurrentProcess.Comment = vm.comment;
        model.push(vm.memberEPA);

        $modalInstance.close(vm.comment);
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalNotificationController',
['$modalInstance', 'message',
function ($modalInstance, message) {
    var vm = this;

    vm.message = message;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller("ModalRemindController",
['$modalInstance',
 function ($modalInstance) {
     var vm = this;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);