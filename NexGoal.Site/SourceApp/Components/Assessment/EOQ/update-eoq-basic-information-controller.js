(function (assessmentModule) {

    assessmentModule.controller("UpdateEOQBasicInformationController", updateEOQBasicInformationController);

    updateEOQBasicInformationController.$inject = ["$scope","EOQModel"];

    function updateEOQBasicInformationController($scope, EOQModel) {
        var vm = this;

        vm.period = {};

        vm.openPopupStartDate = function (event) {
            EOQModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            EOQModel.openPopupEndDate(event);
        };

        $scope.$watch(function () {
            return $scope.updateEOQCtrl.period;
        }, function (newVal) {
            if (newVal) {
                vm.period = $scope.updateEOQCtrl.period;
            }
        });

        function initialze() {
            vm.period = $scope.updateEOQCtrl.period;
            vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
            vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
            vm.calender = EOQModel.calendar;

            console.log(vm.period);
        }

        initialze();
    }
})(angular.module("assessmentModule"));