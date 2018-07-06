module.exports = {
    axiosError: function (error) {
        if (error.response) {
            var url;

            switch (error.response.status) {
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

            window.location = document.getElementById('RootURL').value + url;
        }
    },
    getRootUrl: function () {
        return document.getElementById('RootURL').value;
    }
};