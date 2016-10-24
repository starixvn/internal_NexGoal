'use strict';
/*global appModule, moment*/

appModule.factory('utilitiesFactory',
['$filter', 'enumerationsFactory', function ($filter, enumerationsFactory) {
    var utilities = {};
    var limitNumber = 100,
        dateTimeFormatString = "MM/DD/YYYY HH:mm",
        dateFormat = "dd MMM, yyyy",
        timeFormat = 'HH:mm',
        SEMI_ANNUAL_TARGET_TYPE = 3;
    var focusEvaluationLevel = enumerationsFactory.focusEvaluationLevel;

    function getLatestPeriodsIndex(periods) {
        var listIndex = [];
        var maxDate = new Date(1, 1, 1);

        angular.forEach(periods, function (period, index) {
            if (period.TargetType && period.TargetType.TargetTypeId === SEMI_ANNUAL_TARGET_TYPE && period.IsEndProcess) {
                var periodEndDate = new Date(period.EndDate);

                if (periodEndDate.setHours(0, 0, 0, 0) > maxDate.setHours(0, 0, 0, 0)) {
                    listIndex = [];
                    maxDate = angular.copy(periodEndDate);
                    listIndex.push(index);
                } else if (periodEndDate.setHours(0, 0, 0, 0) === maxDate.setHours(0, 0, 0, 0)) {
                    listIndex.push(index);
                }
            }
        });

        return listIndex;
    }

    utilities.maxStar = 5;

    utilities.getFocusEvaluationLevel = function (value) {
        var level = {
            Value: 0,
            Name: ""
        };
        var itemIndex = focusEvaluationLevel.map(function (item) { return item.Value; }).indexOf(value);

        if (itemIndex !== -1) {
            level = angular.copy(focusEvaluationLevel[itemIndex]);
        }

        return level;
    };

    utilities.getClassByNominationStatus = function (statusId) {
        var statusClass = "";

        switch (statusId) {
            case 1:
                statusClass = "nominated";
                break;
            case 2:
                statusClass = "shortlisted";
                break;
            case 3:
                statusClass = "winner";
                break;
        }

        return statusClass;
    };

    utilities.groupingTeams = function (teams) {
        var result = [], parents = [];
        var startGrouping, endGrouping;

        angular.forEach(teams, function (team) {
            if (team.DepartmentId > 0 && !team.IsGroup && team.Code !== "Nexcel") {
                if (parents.indexOf(team.DepartmentId) === -1) {
                    startGrouping = { Name: '<strong>' + team.Name + '</strong>', msGroup: true };

                    if (parents.length === 0) {
                        result.push(startGrouping);
                    } else {
                        endGrouping = { msGroup: false };
                        result.push(endGrouping);
                        result.push(startGrouping);
                    }

                    parents.push(team.Name);
                }

                angular.forEach(team.Childrens, function (child) {
                    result.push(child);
                });
            }
        });

        endGrouping = { msGroup: false };
        result.push(endGrouping);

        return result;
    };

    utilities.checkInProcessPeriod = function (periods) {
        var hasProcessPeriod = false;

        angular.forEach(periods, function (period) {
            if (period.TargetType && period.TargetType.TargetTypeId === SEMI_ANNUAL_TARGET_TYPE && !period.IsEndProcess) {
                hasProcessPeriod = true;
            }
        });

        return hasProcessPeriod;
    };

    utilities.checkLastestPeriods = function (periods) {
        var listIndex = getLatestPeriodsIndex(periods);

        angular.forEach(listIndex, function (singleIndex) {
            var period = periods[singleIndex];

            if (period) {
                period.ticked = true;
            }
        });
    };

    utilities.groupingPeriods = function (periods) {
        var result = [], years = [];
        var startGrouping, endGrouping;

        angular.forEach(periods, function (period) {
            if (period.Year) {
                if (!period.IsEndProcess) {
                    period.ticked = true;
                }

                if (years.indexOf(period.Year) === -1) {
                    startGrouping = { Name: '<strong>' + period.Year + '</strong>', msGroup: true };
                    if (years.length === 0) {
                        result.push(startGrouping);
                    } else {
                        endGrouping = { msGroup: false };
                        result.push(endGrouping);
                        result.push(startGrouping);
                    }

                    years.push(period.Year);
                    result.push(period);
                } else {
                    result.push(period);
                }
            }
        });

        endGrouping = { msGroup: false };
        result.push(endGrouping);

        if (periods.length > 0 && result.length) {
            var isInProcess = false;

            angular.forEach(periods, function (period) {
                if (!period.IsEndProcess) {
                    isInProcess = true;
                }

                if (!period.Year) {
                    period.ticked = false;
                    result.unshift(period);
                }
            });

            if (!isInProcess) {
                var period = periods[0];

                angular.forEach(result, function (item) {
                    if (item.PeriodId && item.PeriodId === period.PeriodId) {
                        item.ticked = true;
                    }
                });
            }

        } else {
            angular.forEach(periods, function (period) {
                if (!period.Year) {
                    period.ticked = false;
                    result.unshift(period);
                }
            });

            if (result.length > 0) {
                result[0].ticked = true;
            }
        }

        return result;
    };

    utilities.numberEntryList = function (vm) {
        vm.entries = [5, 10, 15, 20, 50, 100, 200];
        vm.pageSize = 20;
    };

    utilities.getDateFormat = function (vm) {
        vm.dateFormat = dateFormat;
    };

    utilities.getTimeFormat = function (vm) {
        vm.timeFormat = timeFormat;
    };

    /*
    ** event read more/less text.
    */
    utilities.maxLengthDescription = function (vm) {
        vm.maxLengthDescription = limitNumber;
    };

    utilities.readMore = function (item) {
        item.numLimit = 10000;
        item.showReadMore = true;
    };

    utilities.readLess = function (item) {
        item.numLimit = limitNumber;
        item.showReadMore = false;
    };

    utilities.pageOption = {
        pageIndex: 0,
        itemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 500]
    };

    utilities.dateTimeFormatString = function () {
        return dateTimeFormatString;
    };

    utilities.getDateTime = function (dateObject) {
        var resultDate = '';

        if (dateObject) {
            resultDate = moment(dateObject).format(dateTimeFormatString);
        }

        return resultDate;
    };

    utilities.getInt = function (number) {
        var result = number;

        if (number) {
            result = parseInt(number, 10);
        }

        return result;
    };

    utilities.openPopupStartDate = function ($event, vm) {
        $event.preventDefault();
        $event.stopPropagation();

        vm.calendar.startDateOpened = true;
        vm.calendar.endDateOpened = false;
    };

    utilities.openPopupEndDate = function ($event, vm) {
        $event.preventDefault();
        $event.stopPropagation();

        vm.calendar.endDateOpened = true;
        vm.calendar.startDateOpened = false;
    };

    utilities.searchOnPage = function (vm, originalData) {
        /*
        * Check if item match search text.
        */
        var searchMatch = function (haystack, needle) {
            if (!needle) {
                return true;
            }

            return haystack.toLowerCase().indexOf(needle.toLowerCase()) !== -1;
        };

        /*
        * Check all item recursion.
        */
        var checkAllItemRecursion = function (item, searchKey) {
            for (var attr in item) {
                var childItem = item[attr];

                if (childItem !== null && /^[a-zA-Z0-9]*$/.test(attr) &&
                        attr.toLowerCase().search('id') === -1 && attr.toLowerCase().search('length') === -1 &&
                        attr.toLowerCase().search('index') === -1 && attr.toLowerCase().search('hash') === -1 &&
                        attr.toLowerCase().search('time') === -1 && attr.toLowerCase().search('date') === -1 &&
                        attr.toLowerCase().search('modifiedby') === -1 && attr.toLowerCase().search('createdby') === -1) {
                    if (typeof (childItem) === 'object') {
                        var exist = checkAllItemRecursion(childItem, searchKey);

                        if (exist) {
                            return exist;
                        }
                    }
                    else {
                        if (searchMatch(item[attr].toString(), searchKey)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        };

        vm.search = function () {
            vm.filteredItems = [];

            if (originalData && originalData.length > 0) {
                vm.filteredItems = $filter('filter')(originalData, function (item) {
                    return checkAllItemRecursion(item, vm.searchKey);
                });
            }
        };

        vm.search();
    };

    utilities.formatString = function () {
        var theString = arguments[0];
        for (var i = 1; i < arguments.length; i++) {
            var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
            theString = theString.replace(regEx, arguments[i]);
        }

        return theString;
    };

    utilities.getSelectedDataByProperty = function (data, propName) {
        var selectedItems = [];

        angular.forEach(data, function (item) {
            var selectedValue = propName ? item[propName] : item;

            if (item.ticked) {
                selectedItems.push(selectedValue);
            }
        });

        var result = null;

        if (selectedItems.length > 0) {
            result = propName ? selectedItems.join(',') : selectedItems;
        }

        return result;
    };

    utilities.getSelectedData = function (data, propName) {
        var selectedProperties = [],
            allProperties = [];

        angular.forEach(data, function (item) {
            var selectedValue = propName ? item[propName] : item;

            if (item.ticked) {
                selectedProperties.push(selectedValue);
            }

            allProperties.push(selectedValue);
        });

        var result = null;

        if (selectedProperties.length > 0) {
            result = propName ? selectedProperties.join(',') : selectedProperties;
        } else if (allProperties.length > 0) {
            result = propName ? allProperties.join(',') : allProperties;
        }

        return result;
    };

    utilities.setSelectAll = function (data) {
        angular.forEach(data, function (item) {
            if (item.msGroup === undefined) {
                item.ticked = true;
            }
        });
    };

    utilities.setSelectedItem = function (data, selectedName, selectedItems) {
        if (selectedItems && selectedItems.length > 0) {
            var array = selectedItems.split(',');
            angular.forEach(array, function (id) {
                id = parseInt(id, 10);
                angular.forEach(data, function (item) {
                    if (item[selectedName] === id) {
                        item.ticked = true;
                    }
                });
            });
        }
    };

    utilities.parseFloat = function (input) {
        var result = input ? parseFloat(input).toFixed(2) : 0;

        return parseFloat(result);
    };

    utilities.splitNumber = function (number, numberDecimal) {
        number = number ? parseFloat(number) : 0;
        var result = number;

        if (!numberDecimal) {
            numberDecimal = 2;
        }

        if ((number % 1) !== 0) {
            var index = number.toString().indexOf('.'),
                decimal = number.toString().slice(index + 1);

            number = number.toString().slice(0, index) + '.' + decimal.substring(0, numberDecimal);
            result = parseFloat(number);
        }

        return result;
    };

    utilities.getEmployeeIdFromAutoSearch = function getEmployeeId(searchEmployee, employee) {
        if (searchEmployee != "" && angular.isUndefined(employee.EmployeeId)) {
            return 0;
        }

        return employee.EmployeeId;
    };

    return utilities;
}]);