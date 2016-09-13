(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("EOQModel", eoqModel);

    eoqModel.$inject = ["$modal", "eoqFactory", "nominationFactory", "errorFactory", "notificationFactory", "utilitiesFactory"];

    function eoqModel($modal, eoqFactory, nominationFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var vm = {};

        vm.periods = [];
        vm.period = {};
        vm.nominatorSettings = [];
        vm.permission = 0;
        vm.eoqQuestions = [];
        vm.eoqNominations = [];
        vm.nomination = {};

        vm.createEOQ = function (period) {
            return eoqFactory.createEOQ(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.createEOQNomination = function (eoqyId, employeeId, eoqQuestions) {
            return nominationFactory.createEOQNomination(eoqyId, employeeId, eoqQuestions).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEOQ = function (period) {
            return eoqFactory.updateEOQ(period).success(function (response) {
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

        vm.updateEOQNomination = function (nominationId, eoqQuestions) {
            return nominationFactory.updateNomination(nominationId, eoqQuestions).success(function (response) {
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

        vm.getEOQ = function (year, statusId) {
            return eoqFactory.getEOQ(year, statusId).success(function (periods) {
                vm.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOQById = function (eoqId) {
            return eoqFactory.getEOQById(eoqId).success(function (period) {
                vm.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOQNominatorSettingById = function (eoqyId, query, groupIds, roleId, showAllEmployee) {
            return eoqFactory.getEOQNominatorSettingById(eoqyId, query, groupIds, roleId, showAllEmployee).success(function (nominatorSettings) {
                vm.nominatorSettings = nominatorSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEOQNominatorSettingById = function (eoqyId, nominators) {
            return eoqFactory.updateEOQNominatorSettingById(eoqyId, nominators).success(function (response) {
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

        vm.getEOQQuestion = function () {
            return nominationFactory.getEOQQuestion().success(function (eoqQuestions) {
                vm.eoqQuestions = eoqQuestions;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOQNominationByNominatorId = function (eoqyId, roleId, query, teamIds, statusId) {
            return nominationFactory.getNominationByNominatorId(eoqyId, roleId, query, teamIds, statusId).success(function (eoqNominations) {
                vm.eoqNominations = eoqNominations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getCurrentEOQByNominatorId = function () {
            return nominationFactory.getCurrentEOQByNominatorId().success(function (period) {
                vm.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOQNominationById = function (nominationId) {
            return nominationFactory.getNominationById(nominationId).success(function (nomination) {
                vm.nomination = nomination;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupCreateEOQ = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modalCreateEOQ.html',
                controller: "CreateEOQController as createEOQCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center period-create-popup"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupDeleteEOQ = function (vm, period) {
            var modalInstance = $modal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return period.QuarterToString + " of " + period.Year;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                return eoqFactory.deleteEOQ(period.EOQYId).success(function (response) {
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
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modalCreateEOQNonimation.html',
                controller: "CreateEOQNominationController as createEOQNominationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center popup-lg",
                resolve: {
                    eoqNominationPeriod: function () {
                        return angular.copy(period);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        vm.openPopupViewNomination = function (nominationId) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modalViewEOQNomination.html',
                controller: "ViewEOQNominationController as viewEOQNominationCtrl",
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
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modalUpdateEOQNomination.html',
                controller: "UpdateEOQNominationController as updateEOQNominationCtrl",
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

        vm.validatePeriod = function (period) {
            if (!period) {
                return false;
            } else {
                if (!period.Quarter) {
                    notificationFactory.error("QuarterRequired");
                    return false;
                }

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
            }

            return true;
        };

        vm.validateNominee = function (nominee) {
            if (!nominee) {
                return false;
            } else {
                if (!nominee.employeeId || nominee.employeeId === 0) {
                    notificationFactory.error("NomineeRequired");
                    return false;
                }

                var validComments = true;

                if (nominee.eoqQuestions) {
                    nominee.eoqQuestions.forEach(function (question) {
                        if (!question.Comment || question.Comment.length === 0) {
                            validComments = false;
                            return;
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
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modelEOQInfomation.html',
                controller: "EOQModalInformationController as eoqModalInfoCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupDeleteEOQNomination = function (vm, nomination) {
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
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                return nominationFactory.deleteNomination(nomination.Nominations[0].NominationId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.eoqNominations.indexOf(nomination);
                        vm.eoqNominations.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.eoqNominations);
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

        vm.updateStatusEOQNomination = function (eoqyId, nomineeId, statusId) {
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

        return vm;
    }
})(angular.module("assessmentModule"));