(function (userInfoModule) {
    "use strict";

    userInfoModule.controller("UserInfoController", userInfoController);

    userInfoController.$inject = ["CommonModel"];

    function userInfoController(commonModel) {
        var vm = this;

        vm.userInfo = {};

        commonModel.getUserInfo().then(function () {
            vm.userInfo = commonModel.userInfo;
        });
    }
})(angular.module("userInfoModule"));