'use strict';
/*global roleModule*/

roleModule.controller('RoleController',
['RoleModel', 'CommonModel', "lockScreenService",
function (RoleModel, CommonModel, lockScreenService) {
    var vm = this;

    vm.systemRoleOptions = CommonModel.systemRoles;
    vm.roleFunctions = RoleModel.roleFunctions;

    vm.filter = {
        roleId: ''
    };

    /*
    ** Get system roles.
    */
    CommonModel.getSystemRoles(true, false).then(function () {
        vm.systemRoleOptions = CommonModel.systemRoles;

        if (vm.systemRoleOptions && vm.systemRoleOptions.length > 0) {
            vm.filter.roleId = vm.systemRoleOptions[0].RoleId;
            getFunctionsOfRole(vm.filter.roleId);
        }
    });

    /*
    ** Get functions of role.
    */
    var getFunctionsOfRole = function (roleId) {
        lockScreenService.start();

        RoleModel.getRoleFunctionByRoleId(roleId).then(function () {
            vm.roleFunctions = RoleModel.roleFunctions;
            if (roleId === 1) {
                vm.isSystemAdmin = true;
            }
            else {
                vm.isSystemAdmin = false;
            }
        })["finally"](function () {
            lockScreenService.stop();
        });
    };

    /*
    ** Query.
    */
    vm.query = function (filter) {
        getFunctionsOfRole(filter.roleId);
    };

    /*
    ** Assign function to role.
    */
    vm.save = function (roleFunction) {
        lockScreenService.start();

        RoleModel.assignFunctionToRole(roleFunction)["finally"](function () {
            lockScreenService.stop();
        });
    };
} ]);