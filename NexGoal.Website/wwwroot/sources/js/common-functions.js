import React from 'react';

const _rootUrl = document.getElementById('RootURL').value ? document.getElementById('RootURL').value : '/';

export const resolveClientUrl = (relativePath) => {
    return _rootUrl + relativePath;
};

export const axiosError = (error) => {
    debugger;
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

        window.location = _rootUrl + url;
    }
};

export const noDataIndication = () => <span className="text-danger">No information available!</span>;