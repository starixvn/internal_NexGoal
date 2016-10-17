(function (targetTemplateModule) {
    "use strict";

    targetTemplateModule.controller("CreateTargetTemplateController", createTargetTemplateController);

    createTargetTemplateController.$inject = ['$modalInstance', 'TargetTemplateModel', 'CommonModel'];

    function createTargetTemplateController($modalInstance, TargetTemplateModel, CommonModel) {
        var vm = this;

        vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
        vm.ratingTypes = TargetTemplateModel.ratingTypes;

        vm.targetTemplate = {
            Name: '',
            TargetTypeId: '',
            ScoreRatingTypeId: 3,
            RatingTypeId: 1,
            IsActive: true,
            Description: ''
        };
        vm.targetTypes = CommonModel.targetTypes;

        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        TargetTemplateModel.getScoreRatingType(null, true).then(function () {
            vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
        });

        TargetTemplateModel.getRatingType().then(function () {
            vm.ratingTypes = TargetTemplateModel.ratingTypes;
        });

        vm.save = function () {
            if (TargetTemplateModel.validateTargetTemplate(vm.targetTemplate)) {
                TargetTemplateModel.createTargetTemplate(vm.targetTemplate, $modalInstance);
            }
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }
})(angular.module("targetTemplateModule"));