import React, { Fragment } from 'react';
import { ToastContainer, Slide } from 'react-toastify';

const ToastLayout = props => (
    <Fragment>
        <ToastContainer
            position="top-center"
            autoClose={3000}
            hideProgressBar={false}
            transition={Slide}
            closeOnClick={false}
            draggable={false} />
        {props.children}
    </Fragment>
);

export default ToastLayout;