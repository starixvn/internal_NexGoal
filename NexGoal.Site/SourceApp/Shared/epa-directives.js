'use strict';
/*global appModule*/

// remove class button, add class form-control to multi-select
appModule.directive('addClassToRoleFilter', ['$compile', function ($compile) {
    return {
        restrict: 'E',
        link: function (scope, iElement) {
            var button = iElement.find("button.multiSelectButton");

            button.removeClass('button').addClass('form-control');
            $compile(button)(scope);
        }
    };
}]);

appModule.directive('searchAutoComplete', ['$timeout', 'errorFactory', function ($timeout, errorFactory) {
    return {
        scope: {
            employee: "=",
            searchEmployees: "&",
            employeeStyle: '@'
        },
        link: function (scope, iElement) {
            iElement.autocomplete({
                source: function (request, response) {
                    scope.searchEmployees().success(function (employees) {
                        var data = [];
                        if (employees.length > 0) {
                            angular.forEach(employees, function (employee) {
                                data.push(
                                {
                                    value: employee.NickName + ' (' + employee.FullName + ')',
                                    id: employee.EmployeeId,
                                    employee: {
                                        EmployeeId: employee.EmployeeId,
                                        Name: employee.NickName,
                                        HasMonthlyEPA: employee.HasMonthlyEPA,
                                        TeamName: employee.Group.GroupName,
                                        FullName: employee.FullName
                                    }
                                });
                            });

                            response(data);

                            if (scope.employeeStyle) {
                                if (data.length > 0) {
                                    angular.element(".ui-autocomplete").attr("style", scope.employeeStyle);
                                } else {
                                    angular.element(".ui-autocomplete").attr("style", "display: none;");
                                }
                            }
                        } else {
                            scope.employee = {
                                EmployeeId: 0,
                                Name: scope.searchEmployees,
                                HasMonthlyEPA: false,
                                TeamName: ""
                            };
                        }
                    }).error(function (response, status) {
                        errorFactory.errorHandle(status);
                    });
                },
                select: function (event, ui) {
                    scope.employee = ui.item.employee;

                    $timeout(function () {
                        //iElement.trigger('input');
                    }, 0);
                },
                appendTo: iElement.parent(),
                delay: 500,
                minLength: 2
            });
        }
    };
}]);

appModule.directive('calendarImage', function () {
    return {
        restrict: 'AE',
        replace: 'true',
        template: '<img class="calendar-image" src="/Content/Site/Components/images/icon-calendar.svg"></img>'
    };
});

appModule.directive('percentage', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {
                if (inputValue === undefined) {
                    return '';
                }

                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput < 0 || transformedInput > 100) {
                    modelCtrl.$setViewValue('');
                    modelCtrl.$render();
                }
                else {
                    if (transformedInput !== inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }
                }

                return transformedInput;
            });
        }
    };
});

appModule.directive('numberDot', [function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ctrl) {
            ctrl.$parsers.push(function (inputValue) {
                if (inputValue) {
                    var digits = inputValue.replace(/[^\d+\.?\d+$]/, '');

                    if (digits !== inputValue) {
                        ctrl.$setViewValue(digits);
                        ctrl.$render();
                    }

                    return digits;
                }

                return '';
            });
        }
    };
}]);

appModule.directive('datepickerPopup', function () {
    return {
        restrict: 'EAC',
        require: 'ngModel',
        link: function (scope, element, attr, controller) {
            //remove the default formatter from the input directive to prevent conflict
            controller.$formatters.shift();
        }
    };
});

appModule.directive('numberOnly', ['$filter', function ($filter) {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ctrl) {
            ctrl.$formatters.unshift(function () {
                var value = ctrl.$modelValue;

                if (value !== "") {
                    value = $filter(attr.numberOnly)(value);
                }

                return value;
            });

            ctrl.$parsers.push(function (inputValue) {
                if (inputValue) {
                    var digits = inputValue.replace(/[^0-9]/g, '');

                    if (digits !== '') {
                        digits = $filter(attr.numberOnly)(digits);
                    }

                    if (digits !== inputValue) {
                        ctrl.$setViewValue(digits);
                        ctrl.$render();
                    }

                    return digits;
                }

                return '';
            });
        }
    };
}]);

appModule.directive('optionsClass', ['$parse', function ($parse) {
    return {
        require: 'select',
        link: function (scope, elem, attrs, ngSelect) {
            var optionsSourceStr = attrs.ngOptions.split(' ').pop(),
                getOptionsClass = $parse(attrs.optionsClass);

            scope.$watch(optionsSourceStr, function (items) {
                angular.forEach(items, function (item, index) {
                    var classes = getOptionsClass(item),
                        option = elem.find('option[value=' + index + ']');
                    angular.forEach(classes, function (add, className) {
                        if (add) {
                            angular.element(option).addClass(className);
                        }
                    });
                });
            });
        }
    };
}]);