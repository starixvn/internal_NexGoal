(function (menuModule) {
    "use strict";

    menuModule.factory("menuFactory", menuFactory);

    menuFactory.$inject = ["$http", "baseUrl"];

    function menuFactory($http, baseUrl) {
        return {
            getMenus: getMenus
        };

        function getMenus() {
            return $http.get(baseUrl + 'api/Menu/GetMenusByEmployeeId');
        }
    }
})(angular.module("menuModule"));