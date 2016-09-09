(function (assessmentModule) {
	"use strict";

	assessmentModule.controller("ViewEOQNominationController", viewEOQNominationController);

	viewEOQNominationController.$inject = ["$modalInstance", "EOQModel", "lockScreenService", "nominationId"];

	function viewEOQNominationController($modalInstance, EOQModel, lockScreenService, nominationId) {
		var vm = this;

		vm.nomination = {};

		function initialize() {
			lockScreenService.start();

			EOQModel.getEOQNominationById(nominationId).then(function () {
			    vm.nomination = EOQModel.nomination;
			})["finally"](function () {
			    lockScreenService.stop();
			});
		}

		vm.cancel = function () {
			$modalInstance.dismiss("cancel");
		};

		initialize();
	}
})(angular.module("assessmentModule"));