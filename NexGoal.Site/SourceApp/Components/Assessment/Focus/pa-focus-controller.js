(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "PAFocusModel", "utilitiesFactory"];

    function paFocusController(CommonModel, EmployeeModel, PAFocusModel, utilitiesFactory) {
        var vm = this;
        var oldGroupsData = [];
        vm.objectivesAchievementRanges = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
        vm.promotionsRanges = [
            {
                Value: true,
                Name: "Yes"
            },
            {
                Value: false,
                Name: "No"
            }
        ];
        vm.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null,
            employeeId: null
        };
        vm.searchEmployee = "";

        function initializeData() {
            var groupOne = {};

            groupOne.TeamId = 1;
            groupOne.TeamName = "Olympus";
            groupOne.MaxBonus = 100;
            groupOne.TotalBonus = 0;

            groupOne.members = [
                {
                    TeamId: 1,
                    EmployeeId: 1,
                    NickName: "Kaden",
                    FullName: "Trịnh Đăng Khoa",
                    JobTitle: "Senior Software Engineer",
                    EPAScore: 84,
                    ObjectivesAchievement: 50,
                    Contribution: 1,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 10
                },
                {
                    TeamId: 1,
                    EmployeeId: 2,
                    NickName: "Jacob",
                    FullName: "Phan Đình Hải Sơn",
                    JobTitle: "Staff Software Engineer / Team Leader",
                    EPAScore: 85,
                    ObjectivesAchievement: 20,
                    Contribution: 3,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 10
                },
                {
                    TeamId: 1,
                    EmployeeId: 3,
                    NickName: "Leo",
                    FullName: "Võ Quang Lễ",
                    JobTitle: "Software Engineer",
                    EPAScore: 86,
                    ObjectivesAchievement: 40,
                    Contribution: 1,
                    BonusRanking: 1,
                    Promotion: true,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 10
                },
                {
                    TeamId: 1,
                    EmployeeId: 4,
                    NickName: "Robert",
                    FullName: "Trần Văn Khoa",
                    JobTitle: "Senior Software Engineer",
                    EPAScore: 84,
                    ObjectivesAchievement: 100,
                    Contribution: 5,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 20
                }
            ];

            vm.groups.push(groupOne);

            var groupTwo = {};

            groupTwo.TeamName = "Nexus";
            groupTwo.MaxBonus = 100;
            groupTwo.TotalBonus = 0;
            groupTwo.TeamId = 2;

            groupTwo.members = [
                {
                    TeamId: 2,
                    EmployeeId: 5,
                    NickName: "Quinn",
                    FullName: "Võ Kim Quang",
                    JobTitle: "Software Engineer / Team Leader",
                    EPAScore: 84,
                    ObjectivesAchievement: 10,
                    Contribution: 2,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 20
                },
                {
                    TeamId: 2,
                    EmployeeId: 6,
                    NickName: "Razor",
                    FullName: "Nguyễn Văn Mạnh",
                    JobTitle: "Software Engineer",
                    EPAScore: 85,
                    ObjectivesAchievement: 70,
                    Contribution: 4,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 20
                },
                {
                    TeamId: 2,
                    EmployeeId: 7,
                    NickName: "Tim",
                    FullName: "Trịnh Đăng Thắng",
                    JobTitle: "Asst. Software Engineer",
                    EPAScore: 86,
                    ObjectivesAchievement: 80,
                    Contribution: 5,
                    BonusRanking: 1,
                    Promotion: true,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 20
                },
                {
                    TeamId: 2,
                    EmployeeId: 8,
                    NickName: "Larry",
                    FullName: "Trần Văn Hiền",
                    JobTitle: "Software Engineer",
                    EPAScore: 84,
                    ObjectivesAchievement: 60,
                    Contribution: 1,
                    BonusRanking: 1,
                    Promotion: false,
                    Objectives: "Lorem ispum dolor sit amet, consectetur",
                    BonusGiven: 10
                }
            ];

            vm.groups.push(groupTwo);
        }

        function initialize() {
            CommonModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = CommonModel.periodOptions;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }
            });

            CommonModel.getDepartment(null, true).then(function () {
                vm.teams = utilitiesFactory.groupingTeams(CommonModel.departments);

                utilitiesFactory.setSelectAll(vm.teams);
            });

            initializeData();

            angular.forEach(vm.groups, function (group) {
                group.TotalBonus = 0;

                angular.forEach(group.members, function (member) {
                    group.TotalBonus += member.BonusGiven;
                });

                calculateBonusRanking(group.TeamId);
            });

            oldGroupsData = angular.copy(vm.groups);
        }

        function calculateTeamTotalBonus(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    group.TotalBonus = 0;

                    angular.forEach(group.members, function (member) {
                        group.TotalBonus += member.BonusGiven;
                    });
                }
            });
        }

        function calculateBonusRanking(teamId) {
            angular.forEach(vm.groups, function (group) {
                if (group.TeamId === teamId) {
                    var members = angular.copy(group.members);

                    members.sort(function (memberA, memberB) {
                        return memberB.BonusGiven - memberA.BonusGiven;
                    });

                    angular.forEach(members, function (member, index) {
                        var previousMember = members[index - 1];

                        if (previousMember) {
                            member.BonusRanking = previousMember.BonusGiven === member.BonusGiven ?
                                previousMember.BonusRanking :
                                previousMember.BonusRanking + 1;
                        } else {
                            member.BonusRanking = index + 1;
                        }
                    });

                    angular.forEach(group.members, function (member) {
                        var rankingMemberIndex = members.map(function (item) { return item.EmployeeId; }).indexOf(member.EmployeeId);

                        member.BonusRanking = members[rankingMemberIndex].BonusRanking;
                    });
                }
            });
        }

        vm.getEmployee = function () {
            return EmployeeModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employeeId) {
                    vm.filter.employeeId = null;
                }
            }
        };

        vm.openPopupObjective = function (member) {
            PAFocusModel.openPopupObjective(member).then(function (objectives) {
                member.Objectives = objectives;
            });
        };

        vm.inputBonus = function (member) {
            if (member.BonusGiven === "" || member.BonusGiven < 0) {
                member.BonusGiven = 0;
            }

            calculateTeamTotalBonus(member.TeamId);
            calculateBonusRanking(member.TeamId);
        };

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');

            console.log(vm.filter);

            alert("Implement Query Later!");
        };

        vm.reset = function () {
            vm.groups = angular.copy(oldGroupsData);
        };

        vm.save = function () {
            if (PAFocusModel.validatePAFocusForm(vm.groups)) {
                alert("Implement Save Later!");
            }
        };

        initialize();
    }
})(angular.module("assessmentModule"));