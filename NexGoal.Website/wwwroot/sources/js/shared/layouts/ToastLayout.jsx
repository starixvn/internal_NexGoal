import React, { Fragment } from 'react';
import { ToastContainer } from 'react-toastify';

const ToastLayout = props => (
    <Fragment>
        <ToastContainer
            position="top-center"
            autoClose={3000}
            hideProgressBar={false}
            closeOnClick={false}
            draggable={false} />
        {props.children}
    </Fragment>
);

export default ToastLayout;