(function (assessmentModule) {
	"use strict";

	assessmentModule.controller("PAModalLastObjectiveController", paModalLastObjectiveController);

	paModalLastObjectiveController.$inject = ["$modalInstance", "notificationFactory", "member"];

	function paModalLastObjectiveController($modalInstance, notificationFactory, member) {
		var vm = this;

		vm.employeeId = member.EmployeeId;
		vm.LastCompletedPercentObjective = member.LastCompletedPercentObjective;
		vm.IsReadOnly = member.IsReadOnly;

		vm.ok = function () {
		    if (!vm.LastCompletedPercentObjective) {
				notificationFactory.error("ObjectivesRequired");
			} else {
				$modalInstance.close(vm.LastCompletedPercentObjective);
			}
		};

		vm.cancel = function () {
			$modalInstance.dismiss("cancel");
		};
	}
})(angular.module("assessmentModule"));