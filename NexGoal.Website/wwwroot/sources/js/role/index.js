import commonFunctions from '../common-functions.js';

var assignPermissionToRole = (function ($) {
    var $permissionTable,
        $formFilter,
        $submitForm,
        $btnSubmit,
        attrRead = 'criteria\\.funcRead',
        attrHost = 'criteria\\.funcHost';

    function updateParent(type) {
        var $this = $(this),
            pid = $this.data('pid');

        if (pid) {
            var checked = $this.prop('checked'),
                parent = $('#func' + type + pid);

            if (checked && !parent.prop('checked')) {
                parent.prop('checked', true);
            }

            if (!checked) {
                var siblings = $('[name=criteria\\.func' + type + '][data-pid=' + parent.data('fid') + ']:checked');

                if (siblings.length === 0) {
                    parent.prop('checked', false);

                    if (type === 'Read') {
                        parent.closest('tr').find('[name=' + attrHost + ']').prop('checked', false);
                    }
                }
            }

            updateParent.call(parent.get(0), type);
        }
    }

    function registerEventCheckRoleFunc() {
        $permissionTable.find('input[type=checkbox]:not(:disabled)').on('change', function () {
            var $this = $(this),
                functionId = $this.data('fid'),
                isChecked = $this.prop('checked'),
                row = $this.closest('tr');

            switch ($this.attr('name')) {
                case 'criteria.funcHost':
                    // host checked -> read checked
                    if (isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=' + attrRead + ']').prop('checked', true).get(0),
                            'Read');
                        $permissionTable.find('[type=checkbox][name=' + attrRead + '][data-pid=' + functionId + ']:not(:disabled)')
                            .attr('checked', true).change();
                    }

                    // parent changed -> children changed
                    $permissionTable.find('[type=checkbox][name=' + attrHost + '][data-pid=' + functionId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Host');
                    break;

                case 'criteria.funcRead':
                    // read unchecked -> host unchecked
                    if (!isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=' + attrHost + ']:not(:disabled)').prop('checked', false).get(0),
                            'Host');
                    }

                    // parent changed -> children changed
                    $permissionTable.find('[type=checkbox][name=' + attrRead + '][data-pid=' + functionId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Read');
                    break;
            }
        });
    }

    function registerEventChangeRole() {
        $('#RoleId').on('change', function () {
            $formFilter.submit();
        });
    }

    function registerEventSubmit() {
        $btnSubmit.on('click', function () {
            var roleId = $('#RoleId').val(),
                model = [],
                els = [],
                totalItems;

            els = $permissionTable.find('input[type=checkbox][name=' + attrRead + ']:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i < totalItems; i++) {
                var el = $(els[i]),
                    functionId = el.data('fid'),
                    isRead = el.prop('checked'),
                    isHost = $permissionTable
                        .find('input[type=checkbox][name=' + attrHost + '][data-fid=' + functionId + ']:not(:disabled)').prop('checked');

                model.push({ FunctionId: functionId, Policy: isHost ? 2 : (isRead ? 1 : 0) });
            }

            if (totalItems > 0) {
                var token = $('input[name="__RequestVerificationToken"]', $submitForm).val();

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.getRootUrl() + 'Role/AssignPermission',
                    data: {
                        __RequestVerificationToken: token,
                        roleId: roleId,
                        permissions: model
                    },
                    success: function (response) {
                        if (response.isSuccessful) {
                            $('.msg-success').removeClass('hidden');

                            setTimeout(function () {
                                $('.msg-success').addClass('hidden');
                            }, 3000);
                        } else {
                            $('.msg-error').text(response.Message).removeClass('hidden');

                            setTimeout(function () {
                                $('.msg-error').text('').addClass('hidden');
                            }, 3000);
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEvents() {
        registerEventChangeRole();
        registerEventCheckRoleFunc();
        registerEventSubmit();
    }

    function initModule() {
        $permissionTable = $('#permissionTable');
        $formFilter = $('#formFilter');
        $submitForm = $('#submitForm');
        $btnSubmit = $('#btnSubmit');

        registerEvents();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    assignPermissionToRole.initModule();
});