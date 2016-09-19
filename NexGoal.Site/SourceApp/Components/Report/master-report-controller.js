(function (reportModule) {
    function masterReportController($scope, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.teams = [];
        vm.filter = {
            periodId: '',
            teamIds: '',
            employeeId: ''
        };
    }
})(angular.module("reportModule"));