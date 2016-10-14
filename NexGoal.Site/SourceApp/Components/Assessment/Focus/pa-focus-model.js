(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("PAFocusModel", paFocusModel);

    paFocusModel.$inject = ["$modal", "paFocusFactory", "errorFactory", "notificationFactory", "$translate"];

    function paFocusModel($modal, paFocusFactory, errorFactory, notificationFactory, $translate) {
        var vm = {};

        vm.permission = [];
        vm.process = [];
        vm.processSettings = [];
        vm.evaluations = [];
        vm.PAFocusPermission = {};

        function buildData(evaluations, employeeId) {
            var teams = [];

            angular.forEach(evaluations, function (evaluation) {
                var groupName = evaluation.Employee.Group.GroupName;
                var groupId = evaluation.Employee.Group.GroupId;
                if (teams.length === 0) {
                    teams.push({ TeamId: groupId, Name: groupName, MaxBonus: 100, TotalBonus: 0, Members: [] });
                } else {
                    var isExsited = false;
                    angular.forEach(teams, function (team) {
                        if (team.Name === groupName) {
                            isExsited = true;
                        }
                    });
                    if (!isExsited) {
                        teams.push({ TeamId: groupId, Name: groupName, MaxBonus: 100, TotalBonus: 0, Members: [] });
                    }
                }
            });

            angular.forEach(teams, function (team) {
                var isDisplay = false;
                angular.forEach(evaluations, function (evaluation) {
                    var groupName = evaluation.Employee.Group.GroupName;
                    if (groupName === team.Name) {
                        if (employeeId === undefined) {
                            evaluation.IsDisplay = true;
                            isDisplay = true;
                        }
                        else {
                            if (evaluation.Employee.EmployeeId === employeeId) {
                                evaluation.IsDisplay = true;
                                isDisplay = true;
                            } else {
                                evaluation.IsDisplay = false;
                            }
                        }

                        evaluation.NextDescriptionObjective = evaluation.NextDescriptionObjective ? evaluation.NextDescriptionObjective : "";
                        evaluation.LastCompletedPercentObjective = evaluation.LastCompletedPercentObjective ? evaluation.LastCompletedPercentObjective : "";

                        team.Members.push(evaluation);
                        team.TotalBonus += evaluation.BonusGiven;
                    }
                });
                team.IsDisplay = isDisplay;
            });

            return teams;
        }

        function unGroupEvaluationByTeam(teams) {
            var evaluations = [];

            angular.forEach(teams, function (team) {
                angular.forEach(team.Members, function (member) {
                    if (!member.IsReadOnly) {
                        evaluations.push(member);
                    }
                });
            });

            return evaluations;
        }

        vm.getPAFocusProcess = function (periodId) {
            return paFocusFactory.getPAFocusProcess(periodId).success(function (process) {
                vm.process = process;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getPermission = function (periodId) {
            return paFocusFactory.getPermission(periodId).success(function (permission) {
                vm.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getPAProcessSettingByPeriodId = function (periodId) {
            return paFocusFactory.getPAProcessSettingByPeriodId(periodId).success(function (processSettings) {
                vm.processSettings = processSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getPAFocus = function (periodId, groupIds, employeeId) {
            return paFocusFactory.getPAFocus(periodId, groupIds, employeeId).success(function (evaluations) {
                var groups = evaluations;
                vm.evaluations = buildData(groups, employeeId);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updatePAFocus = function (groups) {
            var evaluations = unGroupEvaluationByTeam(groups);

            return paFocusFactory.updatePAFocus(evaluations).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updatePAProcessSettingByPeriodId = function (periodId, processes) {
            return paFocusFactory.updatePAProcessSettingByPeriodId(periodId, processes).success(function (response) {
                if (!response.IsSuccessful) {
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupLostData = function () {
            vm.modalInstance = $modal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center",
                resolve: {
                    name: function () {
                        return "";
                    },
                    action: function () {
                        return "";
                    },
                    message: function () {
                        return "All unsaved changes will be lost. Do you want to proceed?";
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.OpenPopupContinue = function (groups) {
            $translate("TotalBonusGivenLessThanMaxMessage").then(function (responseMessage) {
                var modalInstance = $modal.open({
                    templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                    controller: "ModalConfirmController as modalConfirmCtrl",
                    backdrop: 'static',
                    resolve: {
                        name: function () {
                            return "";
                        },
                        action: function () {
                            return "";
                        },
                        message: function () {
                            return responseMessage;
                        }
                    },
                    windowClass: "modal-confirm-center"
                });

                modalInstance.result.then(function () {
                    vm.updatePAFocus(groups);
                })['finally'](function () {
                    modalInstance = '';
                });
            });
        };

        vm.openPopupInformation = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalInformation.html',
                controller: "PAModalInformationController as paModalInformationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupCurrentObjective = function (member) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalLastObjective.html',
                controller: "PAModalLastObjectiveController as paModalLastObjectiveCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center",
                resolve: {
                    member: function () {
                        return angular.copy(member);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.openPopupObjective = function (member) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalObjective.html',
                controller: "PAModalObjectiveController as paModalObjectiveCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center",
                resolve: {
                    member: function () {
                        return angular.copy(member);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        return vm;
    }
})(angular.module("assessmentModule"));