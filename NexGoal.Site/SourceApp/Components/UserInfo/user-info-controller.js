(function (userInfoModule) {
    "use strict";

    userInfoModule.controller("UserInfoController", userInfoController);

    userInfoController.$inject = ["CommonModel"];

    function userInfoController(commonModel) {
        var vm = this;

        vm.userInfo = {};

        commonModel.getEmployeeById(null).then(function () {
            vm.userInfo = commonModel.employee;
        });
    };

})(angular.module("userInfoModule"));