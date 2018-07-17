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
    },
    objectToFormData: function (obj, formData, namespace) {
        var fd = new FormData();
        var formKey;

        for (var property in obj) {
            if (obj.hasOwnProperty(property) && obj[property]) {
                if (namespace) {
                    formKey = namespace + '[' + property + ']';
                } else {
                    formKey = property;
                }

                if (obj[property] instanceof Date) {
                    fd.append(formKey, obj[property].toISOString());
                } else if (obj[property] instanceof Array) {
                    formKey = formKey + '[]'
                    obj[property].forEach(element => {
                        fd.append(formKey, obj[property]);
                    })
                } else if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
                    // if the property is an object, but not a File, use recursivity.
                    this.objectToFormData(obj[property], fd, formKey);
                } else {
                    // if it's a string or a File object
                    fd.append(formKey, obj[property]);
                }
            }
        }

        return fd;
    }
};