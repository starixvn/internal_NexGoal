module.exports = {
    handleAjaxError: function (xhr, textStatus, errorThrown) {
        var url;

        switch (xhr.status) {
            case 401:
                url = 'auth/login';
                break;

            case 405:
                url = 'error/405';
                break;

            case 500:
                url = 'error/500';
                break;
        }

        window.location = $('#RootURL').val() + url;
    },
    getRootUrl: function () {
        return $('#RootURL').val();
    }
};