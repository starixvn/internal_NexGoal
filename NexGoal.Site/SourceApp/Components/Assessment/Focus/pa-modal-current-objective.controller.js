(function (assessmentModule) {
	"use strict";

	assessmentModule.controller("PaModalCurrentObjectiveController", paModalObjectiveController);

	paModalCurrentObjectiveController.$inject = ["$modalInstance", "notificationFactory", "member"];

	function paModalCurrentObjectiveController($modalInstance, notificationFactory, member) {
		var vm = this;

		vm.employeeId = member.EmployeeId;
		vm.CurrentObjective = member.CurrentObjective;
		vm.IsReadOnly = member.IsReadOnly;

		vm.ok = function () {
			if (!vm.CurrentObjective) {
				notificationFactory.error("ObjectivesRequired");
			} else {
				$modalInstance.close(vm.CurrentObjective);
			}
		};

		vm.cancel = function () {
			$modalInstance.dismiss("cancel");
		};
	}
})(angular.module("assessmentModule"));