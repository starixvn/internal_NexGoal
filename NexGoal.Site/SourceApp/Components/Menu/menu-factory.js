(function (menuModule) {
    "use strict";

    menuModule.factory("menuFactory", menuFactory);

    menuFactory.$inject = ["$http", "baseUrl"];

    function menuFactory($http, baseUrl) {
        return {
            getMenus: getMenus
        };
    }

    function getMenus() {
        // return $http.get(baseUrl + "UrlGoesHere");

        //var a = baseUrl;

        return fakeData();
    }


    function fakeData() {
        return [
            {
                "MenuId": "1",
                "MenuName": "Home",
                "MenuLink": "/",
                "ParentId": "0",
                "Function": "1",
                "Icon": "fa-home",
                "MenuItems": []
            },
            {
                "MenuId": "2",
                "MenuName": "EPA",
                "MenuLink": "/",
                "ParentId": "0",
                "Function": "2",
                "Icon": "fa-cog",
                "MenuItems": [
                    {
                        "MenuId": "3",
                        "MenuName": "Self-Evaluation",
                        "MenuLink": "/SelfEvaluation/Index",
                        "ParentId": "2",
                        "Function": "3",
                        "Icon": ""
                    },
                    {
                        "MenuId": "4",
                        "MenuName": "Evaluator",
                        "MenuLink": "/Evaluator/Index",
                        "ParentId": "2",
                        "Function": "4",
                        "Icon": ""
                    },
                    {
                        "MenuId": "5",
                        "MenuName": "Evaluator By KPI",
                        "MenuLink": "/RatingByKPI/Index",
                        "ParentId": "2",
                        "Function": "5",
                        "Icon": ""
                    }
                ]
            },
            {
                "MenuId": "6",
                "MenuName": "Report",
                "MenuLink": "/",
                "ParentId": "0",
                "Function": "6",
                "Icon": "fa-table",
                "MenuItems": [
                    {
                        "MenuId": "7",
                        "MenuName": "EPA Summary Report",
                        "MenuLink": "/Report/EPASummaryReport",
                        "ParentId": "6",
                        "Function": "7",
                        "Icon": ""
                    },
                    {
                        "MenuId": "8",
                        "MenuName": "EPA Detail Report",
                        "MenuLink": "/Report/EPADetailReport",
                        "ParentId": "6",
                        "Function": "8",
                        "Icon": ""
                    },
                    {
                        "MenuId": "9",
                        "MenuName": "Monthly EPA Summary Report",
                        "MenuLink": "/Report/MonthlyEPADetailReport",
                        "ParentId": "6",
                        "Function": "9",
                        "Icon": ""
                    }
                ]
            }
        ];
    }

})(angular.module("menuModule"));