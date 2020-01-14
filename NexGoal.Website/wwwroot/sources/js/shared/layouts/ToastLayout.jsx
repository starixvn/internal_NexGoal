import React, { Fragment, PureComponent } from 'react';
import { ToastContainer, Slide } from 'react-toastify';

export default class ToastLayout extends PureComponent {
    render() {
        return (
            <Fragment>
                <ToastContainer
                    position="top-center"
                    autoClose={3000}
                    hideProgressBar={false}
                    transition={Slide}
                    closeOnClick={false}
                    draggable={false}
                />
                {this.props.children}
            </Fragment>
        );
    }
}