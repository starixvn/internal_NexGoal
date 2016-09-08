(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("PAFocusModel", paFocusModel);

    paFocusModel.$inject = ["$modal", "paFocusFactory", "errorFactory", "notificationFactory"];

    function paFocusModel($modal, paFocusFactory, errorFactory, notificationFactory) {
        var vm = {};

        vm.processSettings = [];
        vm.evaluations = [];
        vm.PAFocusPermission = {};

        function groupEvaluationByTeam(evaluations) {
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
                angular.forEach(evaluations, function (evaluation) {
                    var groupName = evaluation.Employee.Group.GroupName;
                    if (groupName === team.Name) {
                        team.Members.push(evaluation);
                        team.TotalBonus += evaluation.BonusGiven;
                    }
                });
            });

            return teams;
        }


        function unGroupEvaluationByTeam(team) {
            var evaluations = [];

            angular.forEach(team.Members, function (member) {
                evaluations.push(member);
            });
            return evaluations;
        }

        vm.getPAFocusFullPermission = function () {
            return paFocusFactory.getPAFocusFullPermission().success(function (PAFocusPermission) {
                vm.PAFocusPermission = PAFocusPermission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.validatePAFocusForm = function (group) {
            var isValid = true;


            if (group.TotalBonus !== group.MaxBonus) {
                isValid = false;
                notificationFactory.error("TotalBonusGivenInvalidMessage");
                return;
            }

            return isValid;
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
                vm.evaluations = groupEvaluationByTeam(groups);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updatePAFocus = function (group) {
            var evaluations = unGroupEvaluationByTeam(group);
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

        vm.openPopupInformation = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalInformation.html',
                controller: "PAModalInformationController as paModalInformationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
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