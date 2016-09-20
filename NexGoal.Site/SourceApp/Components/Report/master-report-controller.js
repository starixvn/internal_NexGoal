(function (reportModule) {

    reportModule.controller("MasterReportController", masterReportController);

    masterReportController.$inject = ["$scope", "ReportModel", "EmployeeModel", "CommonModel", "utilitiesFactory", "lockScreenService"];

    function masterReportController($scope, ReportModel, EmployeeModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.employeeReports = [];
        vm.searchEmployee = "";
        vm.employee = null;
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: '',
            teamIds: '',
            employeeId: 0
        };

        function generateFakeData() {
            var data = [];
            var forrest = {
                ReviewerAdjustment: 3,
                EvaluatorComment: "Evaluator Comment",
                PAFocus: {
                    FinalScore: "85.55",
                    Contribution: 1,
                    BonusGiven: 25,
                    BonusRanking: 2,
                    IsReadyPromotion: false,
                    LastCompletedPercentObjective: 80,
                    NextDescriptionObjective: "Next Description Objective",
                    Employee: {
                        EmployeeId: 1,
                        NickName: "Forrest",
                        FullName: "Nguyễn Văn Lâm",
                        JobTitle: {
                            JobTitleName: "Software Engineer"
                        },
                        Group: {
                            GroupId: 1,
                            GroupName: "Nexus"
                        }
                    }
                },
                Nominations: [
                    {
                        Quarter: 1,
                        Year: 2016,
                        Status: 1
                    },
                    {
                        Quarter: 2,
                        Year: 2016,
                        Status: 2
                    },
                    {
                        Quarter: 3,
                        Year: 2016,
                        Status: 3
                    },
                    {
                        Quarter: "",
                        Year: 2016,
                        Status: 3
                    }
                ],
                EmployeeWorkings: [
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project A's Comment"
                    },
                    {
                        ProjectName: "Project B",
                        DecryptedScore: 4,
                        Comment: "Project B's Comment"
                    },
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project B's Comment"
                    }
                ]
            };
            var tim = {
                ReviewerAdjustment: 3,
                EvaluatorComment: "Evaluator Comment",
                PAFocus: {
                    FinalScore: 85,
                    Contribution: 1,
                    BonusGiven: 25,
                    BonusRanking: 2,
                    IsReadyPromotion: false,
                    LastCompletedPercentObjective: 80,
                    NextDescriptionObjective: "Next Description Objective",
                    Employee: {
                        EmployeeId: 2,
                        NickName: "Tim",
                        FullName: "Trịnh Đăng Thắng",
                        JobTitle: {
                            JobTitleName: "Software Engineer"
                        },
                        Group: {
                            GroupId: 1,
                            GroupName: "Nexus"
                        }
                    }
                },
                Nominations: [
                    {
                        Quarter: 1,
                        Year: 2016,
                        Status: 2
                    },
                    {
                        Quarter: 2,
                        Year: 2016,
                        Status: 3
                    },
                    {
                        Quarter: 3,
                        Year: 2016,
                        Status: 1
                    },
                    {
                        Quarter: "",
                        Year: 2016,
                        Status: 2
                    }
                ],
                EmployeeWorkings: [
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project A's Comment"
                    },
                    {
                        ProjectName: "Project B",
                        DecryptedScore: 4,
                        Comment: "Project B's Comment"
                    },
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project B's Comment"
                    }
                ]
            };
            var quinn = {
                ReviewerAdjustment: 3,
                EvaluatorComment: "Evaluator Comment",
                PAFocus: {
                    FinalScore: 85,
                    Contribution: 1,
                    BonusGiven: 25,
                    BonusRanking: 2,
                    IsReadyPromotion: false,
                    LastCompletedPercentObjective: 80,
                    NextDescriptionObjective: "Next Description Objective",
                    Employee: {
                        EmployeeId: 3,
                        NickName: "Quinn",
                        FullName: "Võ Kim Quang",
                        JobTitle: {
                            JobTitleName: "Software Engineer / Team Leader"
                        },
                        Group: {
                            GroupId: 1,
                            GroupName: "Nexus"
                        }
                    }
                },
                Nominations: [],
                EmployeeWorkings: [
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project A's Comment"
                    },
                    {
                        ProjectName: "Project B",
                        DecryptedScore: 4,
                        Comment: "Project B's Comment"
                    },
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project B's Comment"
                    }
                ]
            };

            var jacob = {
                ReviewerAdjustment: -1,
                EvaluatorComment: "Evaluator Comment",
                PAFocus: {
                    FinalScore: 85,
                    Contribution: 1,
                    BonusGiven: 25,
                    BonusRanking: 2,
                    IsReadyPromotion: false,
                    LastCompletedPercentObjective: 80,
                    NextDescriptionObjective: "Next Description Objective",
                    Employee: {
                        EmployeeId: 4,
                        NickName: "Jacob",
                        FullName: "Phan Đình Hải Sơn",
                        JobTitle: {
                            JobTitleName: "Staff Software Engineer / Team Leader"
                        },
                        Group: {
                            GroupId: 2,
                            GroupName: "Olympus"
                        }
                    }
                },
                Nominations: [],
                EmployeeWorkings: [
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project A's Comment"
                    },
                    {
                        ProjectName: "Project B",
                        DecryptedScore: 4,
                        Comment: "Project B's Comment"
                    },
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project B's Comment"
                    }
                ]
            };
            var kaden = {
                ReviewerAdjustment: -1,
                EvaluatorComment: "Evaluator Comment",
                PAFocus: {
                    FinalScore: 85,
                    Contribution: 1,
                    BonusGiven: 25,
                    BonusRanking: 2,
                    IsReadyPromotion: false,
                    LastCompletedPercentObjective: 80,
                    NextDescriptionObjective: "Next Description Objective",
                    Employee: {
                        EmployeeId: 5,
                        NickName: "Kaden",
                        FullName: "Trịnh Đăng Khoa",
                        JobTitle: {
                            JobTitleName: "Software Engineer"
                        },
                        Group: {
                            GroupId: 2,
                            GroupName: "Olympus"
                        }
                    }
                },
                Nominations: [],
                EmployeeWorkings: [
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project A's Comment"
                    },
                    {
                        ProjectName: "Project B",
                        DecryptedScore: 4,
                        Comment: "Project B's Comment"
                    },
                    {
                        ProjectName: "Project A",
                        DecryptedScore: 5,
                        Comment: "Project B's Comment"
                    }
                ]
            };

            data.push(forrest);
            data.push(tim);
            data.push(quinn);
            data.push(jacob);
            data.push(kaden);

            return data;
        }

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);

                vm.teams = angular.copy(treeDepartment);
                utilitiesFactory.setSelectAll(vm.teams);
            });

            ReportModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = ReportModel.periods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });

            vm.employeeReports = generateFakeData();
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unselectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                vm.employee = null;
                vm.filter.employeeId = 0;
            }
        };

        initialize();
    }
})(angular.module("reportModule"));