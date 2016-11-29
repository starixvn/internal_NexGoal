(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("EOYModel", eoyModel);

    eoyModel.$inject = ["$translate", "$modal", "eoyFactory", "nominationFactory",
        "errorFactory", "notificationFactory", "utilitiesFactory"];

    function eoyModel($translate, $modal, eoyFactory, nominationFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var vm = {};

        vm.confirmCancelFormMessage = "";
        vm.periods = [];
        vm.period = {};
        vm.nominatorSettings = [];
        vm.permission = 0;
        vm.eoyQuestions = [];
        vm.eoyNominations = [];
        vm.nomination = {};

        function initialize() {
            $translate("ConfirmCancelFormMessage").then(function (message) {
                vm.confirmCancelFormMessage = message;
            });
        }

        vm.isQuestionModified = function (origin, modified) {
            var isModified = false;

            if (origin.length !== modified.length) {
                isModified = true;
            } else {
                for (var i = 0; i < origin.length; i++) {
                    if (origin[i].Comment !== modified[i].Comment) {
                        isModified = true;
                        break;
                    }
                }
            }

            return isModified;
        };

        vm.createEOY = function (period, modalInstance) {
            return eoyFactory.createEOY(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                    modalInstance.close();
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.createEOYNomination = function (eoqyId, employeeId, eoyQuestions, modalInstance) {
            return nominationFactory.createEOYNomination(eoqyId, employeeId, eoyQuestions).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreateNominationSuccessMessage");
                    modalInstance.close();
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEOY = function (period) {
            return eoyFactory.updateEOY(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEOYNomination = function (nominationId, eoyQuestions, modalInstance) {
            return nominationFactory.updateNomination(nominationId, eoyQuestions).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    modalInstance.close();
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOY = function (year, statusId) {
            return eoyFactory.getEOY(year, statusId).success(function (periods) {
                vm.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOYById = function (eoyId) {
            return eoyFactory.getEOYById(eoyId).success(function (period) {
                vm.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOYNominatorSettingById = function (eoqyId, query, groupIds, roleId, showAllEmployee) {
            return eoyFactory.getEOYNominatorSettingById(eoqyId, query, groupIds, roleId, showAllEmployee).success(function (nominatorSettings) {
                vm.nominatorSettings = nominatorSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEOYNominatorSettingById = function (eoqyId, nominators) {
            return eoyFactory.updateEOYNominatorSettingById(eoqyId, nominators).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getUserPermissionOnManageNomination = function () {
            return nominationFactory.getUserPermissionOnManageNomination().success(function (permission) {
                vm.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getUserPermissionOnNomination = function () {
            return nominationFactory.getUserPermissionOnNomination().success(function (permission) {
                vm.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOYQuestion = function () {
            return nominationFactory.getEOYQuestion().success(function (eoyQuestions) {
                vm.eoyQuestions = eoyQuestions;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOYNominationByNominatorId = function (eoqyId, query, teamIds, statusId) {
            return nominationFactory.getNominationByNominatorId(eoqyId, query, teamIds, statusId).success(function (eoyNominations) {
                vm.eoyNominations = eoyNominations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getCurrentEOYByNominatorId = function () {
            return nominationFactory.getCurrentEOYByNominatorId().success(function (period) {
                vm.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOYNominationById = function (nominationId) {
            return nominationFactory.getEOYNominationById(nominationId).success(function (nomination) {
                vm.nomination = nomination;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupCreateEOY = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EmployeeAward/Year/Templates/modalCreateEOY.html',
                controller: "CreateEOYController as createEOYCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-md"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupDeleteEOY = function (vm, period) {
            var modalInstance = $modal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return "Employee of the Year " + period.Year;
                    },
                    action: function () {
                        return 'delete';
                    },
                    message: function () {
                        return "";
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                return eoyFactory.deleteEOY(period.EOQYId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.periods.indexOf(period);
                        vm.periods.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.periods);
                        notificationFactory.success("DeletedSuccessfully");
                    }
                    else {
                        notificationFactory.error("", response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })["finally"](function () {
                modalInstance = null;
            });
        };

        vm.openPopupAddNominee = function (period) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EmployeeAward/Year/Templates/modalCreateEOYNonimation.html',
                controller: "CreateEOYNominationController as createEOYNominationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-lg",
                resolve: {
                    eoyNominationPeriod: function () {
                        return angular.copy(period);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.openPopupViewNomination = function (nominationId) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EmployeeAward/Year/Templates/modalViewEOYNomination.html',
                controller: "ViewEOYNominationController as viewEOYNominationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-lg",
                resolve: {
                    nominationId: function () {
                        return angular.copy(nominationId);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.openPopupUpdateNomination = function (nominationId) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EmployeeAward/Year/Templates/modalUpdateEOYNomination.html',
                controller: "UpdateEOYNominationController as updateEOYNominationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-lg",
                resolve: {
                    nominationId: function () {
                        return angular.copy(nominationId);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };

        vm.openPopupStartDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.startDateOpened = true;
            vm.calendar.endDateOpened = false;
        };

        vm.openPopupEndDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.startDateOpened = false;
            vm.calendar.endDateOpened = true;
        };

        vm.openPopupRemindDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.endDateOpened = false;
            vm.calendar.remindDateOpened = true;
        };

        vm.validatePeriod = function (period) {
            if (!period) {
                return false;
            } else {
                if (!period.Year) {
                    notificationFactory.error("YearRequired");
                    return false;
                }

                if (!period.RawStartDate) {
                    notificationFactory.error("StartDateRequired");
                    return false;
                }

                if (!period.RawEndDate) {
                    notificationFactory.error("EndDateRequired");
                    return false;
                }

                if (new Date(period.RawStartDate).setHours(0, 0, 0, 0) > new Date(period.RawEndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
                if (new Date(period.RawRemindDate).setHours(0, 0, 0, 0) < new Date(period.RawStartDate).setHours(0, 0, 0, 0) ||
                   new Date(period.RawRemindDate).setHours(0, 0, 0, 0) > new Date(period.RawEndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("RemindDateBetweenStartAndEndDate");
                    return false;
                }
            }

            return true;
        };

        vm.validateNominee = function (nominee) {
            if (!nominee) {
                return false;
            } else {
                if (!nominee.employeeId || nominee.employeeId === 0) {
                    notificationFactory.error("NoEmployeeSelectedWarning");
                    return false;
                }

                var validComments = true;

                if (nominee.eoyQuestions) {
                    angular.forEach(nominee.eoyQuestions, function (question) {
                        if (!question.Comment || question.Comment.length === 0) {
                            question.IsValid = false;
                            validComments = false;
                        } else {
                            question.IsValid = true;
                        }
                    });
                }

                if (!validComments) {
                    notificationFactory.error("NominationCommentRequired");
                    return false;
                }
            }

            return true;
        };

        vm.openPopupInformation = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EmployeeAward/Year/Templates/modalEOYInfomation.html',
                controller: "EOYModalInformationController as eoyModalInfoCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-lg"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupDeleteEOYNomination = function (vm, nomination) {
            var modalInstance = $modal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return nomination.Nominee.FullName + ' (' + nomination.Nominee.NickName + ')';
                    },
                    action: function () {
                        return 'delete';
                    },
                    message: function () {
                        return "";
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                return nominationFactory.deleteNomination(nomination.Nominations[0].NominationId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.eoyNominations.indexOf(nomination);
                        vm.eoyNominations.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.eoyNominations);
                        notificationFactory.success("DeletedSuccessfully");
                    }
                    else {
                        notificationFactory.error("", response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })["finally"](function () {
                modalInstance = null;
            });
        };

        vm.updateStatusEOYNomination = function (eoqyId, nomineeId, statusId) {
            return nominationFactory.updateStatusNomination(eoqyId, nomineeId, statusId).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                }
                else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        initialize();

        return vm;
    }
})(angular.module("assessmentModule"));