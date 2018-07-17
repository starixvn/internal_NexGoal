import React, { Component, Fragment } from 'react';
import { ToastContainer } from 'react-toastify';

export default class ToastLayout extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Fragment>
                <ToastContainer
                    position="top-center"
                    autoClose={3000}
                    hideProgressBar={false}
                    closeOnClick={false}
                    draggable={false} />
                {this.props.children}
            </Fragment>
        );
    }
}