(function (menuModule) {
    "use strict";

    menuModule.factory("menuModel", menuModel);

    menuModel.$inject = ["menuFactory", "errorFactory"];

    function menuModel(menuFactory, errorFactory) {
        var model = {};

        model.menus = {};

        model.getMenus = function () {
            return menuFactory.getMenus()
            .success(function (menus) {
                model.menus = groupMenu(menus);
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        var groupMenu = function (menus) {
            var groupedMenus = [];

            angular.forEach(menus, function (key) {
                var parentMenu;

                if (!key.ParentId) {
                    parentMenu = key;
                    parentMenu.MenuItems = [];

                    angular.forEach(menus, function (otherKey) {
                        if (otherKey.ParentId === parentMenu.MenuId) {
                            parentMenu.MenuItems.push(otherKey);
                        }
                    });

                    groupedMenus.push(parentMenu);
                }
            });

            return groupedMenus;
        };

        return model;
    }
})(angular.module("menuModule"));