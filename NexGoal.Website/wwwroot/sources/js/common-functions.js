module.exports = {
    axiosError: function (error) {
        if (error.response) {
            var url;

            switch (error.response.status) {
                case 401:
                    url = 'auth/login';
                    break;

                case 404:
                    url = 'error/PageNotFound';
                    break;

                case 405:
                    url = 'error/AccessDenied';
                    break;

                case 500:
                    url = 'error/InternalError';
                    break;
            }

            window.location = document.getElementById('RootURL').value + url;
        }
    },
    getRootUrl: function () {
        return document.getElementById('RootURL').value;
    }
};