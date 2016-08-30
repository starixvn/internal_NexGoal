(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAFocusController", paFocusController);

    paFocusController.$inject = ["CommonModel", "EmployeeModel", "utilitiesFactory"];

    function paFocusController(CommonModel, EmployeeModel, utilitiesFactory) {
        var vm = this;

        vm.groups = [];
        vm.periods = [];
        vm.teams = [];
        vm.filter = {
            periodId: null,
            teamIds: null,
            employeeId: null
        };
        vm.searchEmployee = "";

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

        vm.query = function () {
            vm.filter.teamIds = utilitiesFactory.getSelectedData(vm.teams, 'DepartmentId');
            //vm.filter.employeeId = vm.searchEmployee;

            console.log(vm.filter);

            alert("Implement Query Later");
        }

        function initializeData() {
            var groupOne = {};

            groupOne.TeamName = "Olympus";
            groupOne.TotalBonus = 100;

            groupOne.members = [
                {
                    NickName: "Kaden",
                    FullName: "Trịnh Đăng Khoa",
                    JobTitle: "Senior Software Engineer",
                    EPAScore: 84
                },
                {
                    NickName: "Jacob",
                    FullName: "Phan Đình Hải Sơn",
                    JobTitle: "Staff Software Engineer / Team Leader",
                    EPAScore: 85
                },
                {
                    NickName: "Leo",
                    FullName: "Võ Quang Lễ",
                    JobTitle: "Software Engineer",
                    EPAScore: 86
                },
                {
                    NickName: "Robert",
                    FullName: "Trần Văn Khoa",
                    JobTitle: "Senior Software Engineer",
                    EPAScore: 84
                }
            ];

            vm.groups.push(groupOne);

            var groupTwo = {};

            groupTwo.TeamName = "Nexus";
            groupTwo.TotalBonus = 100;

            groupTwo.members = [
                {
                    NickName: "Quinn",
                    FullName: "Võ Kim Quang",
                    JobTitle: "Software Engineer / Team Leader",
                    EPAScore: 84
                },
                {
                    NickName: "Razor",
                    FullName: "Nguyễn Văn Mạnh",
                    JobTitle: "Software Engineer",
                    EPAScore: 85
                },
                {
                    NickName: "Tim",
                    FullName: "Trịnh Đăng Thắng",
                    JobTitle: "Asst. Software Engineer",
                    EPAScore: 86
                },
                {
                    NickName: "Larry",
                    FullName: "Trần Văn Hiền",
                    JobTitle: "Software Engineer",
                    EPAScore: 84
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
        }

        initialize();
    }
})(angular.module("assessmentModule"));