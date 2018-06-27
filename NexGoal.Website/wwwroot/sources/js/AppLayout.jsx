import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

class AppLayout extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Fragment>
                {this.props.children}
            </Fragment>
        );
    }
}

export default AppLayout;